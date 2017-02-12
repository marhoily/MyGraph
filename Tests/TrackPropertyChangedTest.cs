using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MyGraph;
using Tests.FirstTry;
using Xunit;
using static Tests.NpcSamples;

namespace Tests
{
    public sealed class TrackPropertyChangedTest
    {
        private readonly S _a = new S("a", null);
        private readonly S _b = new S("b", null);
        private readonly S _c = new S("c", null);
        private readonly List<string> _log = new List<string>();
        private Action _dispose;

        private void Track(Expression<Func<S>> exp) =>
            _dispose = exp.Track(c => _log.Add(c.Name));

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
            MyExpressionExtensions.GetPropertyName(() => _a.X).Should().Be("X");
        }
        [Fact]
        public void Zip()
        {
            var ints = new [] {1,2,3};
            ints.Zip(ints.Skip(1), (x, y) => $"{x}-{y}")
                .Should().Equal("1-2", "2-3");
        }
        [Fact]
        public void Basic_Scenario()
        {
            Track(() => _a.X);
            _a.X = _b;
            _log.Should().Equal("a");
            _dispose();
            _a.X = null;
            _log.Should().Equal("a");
        }
    }
}