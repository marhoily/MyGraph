using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MyGraph;
using Xunit;

namespace Tests
{
    public sealed class ObservablesTest
    {
        private static NpcSamples.S[] Chain(char start, int count)
        {
            var proto = Enumerable.Range(0, count)
                .Select(i => new NpcSamples.S(new string((char)(start + i), 1), null))
                .ToArray();
            foreach (var p in proto.Zip(proto.Skip(1), (a, b) => new { a, b }))
                p.a.X = p.b;
            return proto;
        }
        private readonly NpcSamples.S _z = new NpcSamples.S("z", null);
        private readonly List<string> _log = new List<string>();

        private List<string> PopLog()
        {
            var result = _log.ToList();
            _log.Clear();
            return result;
        }
        [Fact]
        public void CheckChain()
        {
            Chain(start: 'a', count: 5)[0].ToString().Should().Be("abcde");
        }
        [Fact]
        public void Npc_Should_Subscribe()
        {
            var o1 = _z.Observe<string>(nameof(NpcSamples.S.Name), _log.Add);
            _z.ToString().Should().Be("z*");
            var o2 = _z.Observe<string>(nameof(NpcSamples.S.Name), _log.Add);
            _z.ToString().Should().Be("z**");
            o1.Dispose();
            _z.ToString().Should().Be("z*");
            o2.Dispose();
            _z.ToString().Should().Be("z");
        }
        [Fact]
        public void Long_Chain()
        {
            var chain = Chain(start: 'a', count: 3);
            var observed = chain[0]
                .Observe<NpcSamples.S>(nameof(NpcSamples.S.X), _ => _log.Add("1"))
                .Observe<NpcSamples.S>(nameof(NpcSamples.S.X), _ => _log.Add("2"))
                .Observe<NpcSamples.S>(nameof(NpcSamples.S.X), _ => _log.Add("3"));
            observed.Value.Should().Be(null);

            chain[2].X = _z;
            observed.Value.Should().Be(_z);
            PopLog().Should().Equal("3");
            chain[0].ToString().Should().Be("a*b*c*z");

            chain[1].X = null;
            chain[0].ToString().Should().Be("a*b*");
            observed.Value.Should().Be(null);
            PopLog().Should().Equal("2", "3");

            var replacement = Chain(start: 'd', count: 3);
            chain[0].X = replacement[0]; 
            observed.Value.ToString().Should().Be("f");
            PopLog().Should().Equal("1",  "2", "3");

            chain.Select(i => i.ToString()).Should().Equal("a*d*e*f", "b", "cz");
            replacement.Select(i => i.ToString()).Should().Equal("d*e*f", "e*f", "f");
            observed.Dispose();
            chain.Select(i => i.ToString()).Should().Equal("adef", "b", "cz");
            replacement.Select(i => i.ToString()).Should().Equal("def", "ef", "f");
        }
    }
}