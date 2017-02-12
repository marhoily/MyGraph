using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using FluentAssertions;
using MyGraph;
using Xunit;

namespace Tests
{

    public sealed class TrackPropertyChangedTest
    {
        sealed class S : INotifyPropertyChanged
        {
            public string Name { get; }
            private S _x;
            public S X
            {
                get { return _x; }
                set
                {
                    if (Equals(value, _x)) return;
                    _x = value;
                    OnPropertyChanged();
                }
            }

            public S(string name, S x)
            {
                _x = x;
                Name = name;
            }

            public override string ToString()
            {
                return Name + new string(c: '*', count: _handlersList.Count) + X;
            }

            private readonly List<PropertyChangedEventHandler> _handlersList = new List<PropertyChangedEventHandler>();
            public event PropertyChangedEventHandler PropertyChanged
            {
                add { _handlersList.Add(value); }
                remove { _handlersList.Remove(value); }
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                foreach (var handler in _handlersList)
                    handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
            ExpressionExtensions.GetPropertyName(() => _a.X).Should().Be("X");
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
        [Fact]
        public void Observables()
        {
            _a.X = _b;
            var observed = _a
                .Observe(nameof(S.X), () => _log.Add("1"))
                .Observe(nameof(S.Name), () => _log.Add("2"));
            observed.Fact.Should().Be("b");
            _a.X = _c;
            observed.Fact.Should().Be("c");
            _log.Should().Equal("1", "2");
        }
    }
}