using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FluentAssertions;
using MyGraph;
using Xunit;

namespace Tests
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public sealed class GetPropertyNamesTest
    {
        sealed class Target
        {
            public int A { get; } = 0;
            public int B { get; } = 0;
        }
        sealed class T2
        {
            public int B { get; } = 0;
        }

        private static List<string> Check(Expression<Action<Target, T2>> exp) 
            => exp.GetUniquePropertyNames();

        [Fact]
        public void AccessNone()
        {
            Check((x, y) => 7.ToString()).Should().BeEmpty();
        }
        [Fact]
        public void AccessTwo()
        {
            Check((x, y) => Math.Sin(x.A + x.B)).Should().Equal("A", "B");
        }
        [Fact]
        public void AccessOnT2()
        {
            Check((x, y) => Math.Sin(x.A + y.B + "".Length)).Should().Equal("A");
        }
    }
}