using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MyGraph;
using Xunit;
using static Tests.NpcSamples;

namespace Tests
{
    public sealed class ObservablesTest
    {
        private static S[] Chain(char start, int count)
        {
            var proto = Enumerable.Range(0, count)
                .Select(i => new S(new string((char)(start + i), 1), null))
                .ToArray();
            foreach (var p in proto.Zip(proto.Skip(1), (a, b) => new { a, b }))
                p.a.X = p.b;
            return proto;
        }
        private readonly S _z = new S("z", null);
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
            var o1 = _z.Observe<string>(nameof(S.Name), _log.Add);
            _z.ToString().Should().Be("z*");
            var o2 = _z.Observe<string>(nameof(S.Name), _log.Add);
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
            var observed = chain[0].Track(s => s.X.X.X);
            observed.Subscribe(s => _log.Add(s?.ToString() ?? "<null>"));
            observed.Value.Should().Be(null);

            chain[2].X = _z;
            observed.Value.Should().Be(_z);
            PopLog().Should().Equal("z");
            chain[0].ToString().Should().Be("a*b*c*z");

            chain[1].X = null;
            chain[0].ToString().Should().Be("a*b*");
            observed.Value.Should().Be(null);
            PopLog().Should().Equal("<null>");

            var replacement = Chain(start: 'd', count: 3);
            chain[0].X = replacement[0]; 
            observed.Value.ToString().Should().Be("f");
            PopLog().Should().Equal("f");

            chain.Select(i => i.ToString()).Should().Equal("a*d*e*f", "b", "cz");
            replacement.Select(i => i.ToString()).Should().Equal("d*e*f", "e*f", "f");
            observed.Dispose();
            chain.Select(i => i.ToString()).Should().Equal("adef", "b", "cz");
            replacement.Select(i => i.ToString()).Should().Equal("def", "ef", "f");
        }
    }
}