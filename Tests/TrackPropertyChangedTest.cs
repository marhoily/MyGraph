using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MyGraph;
using Xunit;
using static Tests.NpcSamples;

namespace Tests
{
    public sealed class TrackPropertyChangedTest
    {
        private readonly S _a = new S("a", null);
        private readonly S _b = new S("b", null);
        private readonly List<string> _log = new List<string>();
       
        [Fact]
        public void Direct_Track_Should_Work()
        {
            var dispose = _a.Track(nameof(S.X), (S c) => _log.Add(c.Name));
            _a.X = _b;
            _log.Should().Equal("a");
            dispose();
            _a.X = null;
            _log.Should().Equal("a");
        }
        [Fact]
        public void GetPropertyName()
        {
            PropertiesExplorationExtensions.GetPropertyName(() => _a.X).Should().Be("X");
        }
        [Fact]
        public void Zip()
        {
            var ints = new [] {1,2,3};
            ints.Zip(ints.Skip(1), (x, y) => $"{x}-{y}")
                .Should().Equal("1-2", "2-3");
        }
    }
}