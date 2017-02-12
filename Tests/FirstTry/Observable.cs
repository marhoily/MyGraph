using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using Xunit;
using static Tests.NpcSamples;

namespace Tests.FirstTry
{
    internal interface IObservable<out T> : IDisposable
    {
        T Value { get; }
        void Subscribe(Action<T> handler);
    }

    internal sealed class Npc<T> : IObservable<T>
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly List<Action<T>> _changed = new List<Action<T>>();

        public Npc(string propertyName, INotifyPropertyChanged source = null, Action<T> changeHandler = null)
        {
            _propertyName = propertyName;
            ChangeSource(source);
            if (changeHandler != null) _changed.Add(changeHandler);
        }

        public T Value { get; private set; }
        public void Subscribe(Action<T> handler) => _changed.Add(handler);

        public void ChangeSource(INotifyPropertyChanged source)
        {
            if (ReferenceEquals(_source, source))
                return;
            if (_source != null)
            {
                _source.PropertyChanged -= OnPropertyChanged;
            }
            _source = source;
            if (_source != null)
            {
                UpdateValue();
                _source.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
                UpdateValue();
        }
        private void UpdateValue()
        {
            var value = (T)_source
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(_source);

            if (Equals(Value, value))
                return;
            Value = value;
            _changed.ForEach(handle => handle(value));
        }

        public void Dispose()
        {
            if (_source != null)
                _source.PropertyChanged -= OnPropertyChanged;
        }
    }
    internal sealed class Observable<T> : IObservable<T>
    {
        private readonly IObservable<INotifyPropertyChanged> _source;
        private readonly Npc<T> _npc;
        private readonly List<Action<T>> _changed = new List<Action<T>>();

        public Observable(IObservable<INotifyPropertyChanged> source, string propertyName)
        {
            _source = source;
            _npc = new Npc<T>(propertyName,
                changeHandler: v => _changed.ForEach(h => h(v)));
            _source.Subscribe(_npc.ChangeSource);
            _npc.ChangeSource(_source.Value);
        }
        public T Value => _npc.Value;
        public void Subscribe(Action<T> handler) => _changed.Add(handler);

        public void Dispose()
        {
            _npc.Dispose();
            _source?.Dispose();
        }
    }

    internal static class Ext
    {
        public static IObservable<T> Observe<T>(this INotifyPropertyChanged source, string propertyName, Action<T> handler = null)
        {
            return new Npc<T>(propertyName, source, handler);
        }
        public static IObservable<T> Observe<T>(this IObservable<INotifyPropertyChanged> source, string propertyName, Action<T> handler = null)
        {
            var observable = new Observable<T>(source, propertyName);
            if (handler != null) observable.Subscribe(handler);
            return observable;
        }
    }
    public sealed class ObservablesTest
    {
        sealed class FakeObservable : IObservable<INotifyPropertyChanged>
        {
            public INotifyPropertyChanged Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    _handler?.Invoke(value);
                }
            }

            private Action<INotifyPropertyChanged> _handler;
            private INotifyPropertyChanged _value;

            public FakeObservable(INotifyPropertyChanged value)
            {
                Value = value;
            }
            void IObservable<INotifyPropertyChanged>.Subscribe(Action<INotifyPropertyChanged> handler)
            {
                _handler += handler;
            }

            void IDisposable.Dispose()
            {
            }
        }
        private static S[] Chain(char start, int count)
        {
            var proto = Enumerable.Range(0, count)
                .Select(i => new S(new string((char)(start + i), 1), null))
                .ToArray();
            foreach (var p in proto.Zip(proto.Skip(1), (a, b) => new { a, b }))
                p.a.X = p.b;
            return proto;
        }
        private readonly S _a = new S("a", null);
        private readonly S _b = new S("b", null);
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
            var o1 = _a.Observe<string>(nameof(S.Name), _log.Add);
            _a.ToString().Should().Be("a*");
            var o2 = _a.Observe<string>(nameof(S.Name), _log.Add);
            _a.ToString().Should().Be("a**");
            o1.Dispose();
            _a.ToString().Should().Be("a*");
            o2.Dispose();
            _a.ToString().Should().Be("a");
        }

        [Fact]
        public void Observable_Should_Dispose_Both_Npc_And_Source()
        {
            var o = _a
                .Observe<S>(nameof(S.X), s => _log.Add(s.ToString()))
                .Observe<string>(nameof(S.Name), _log.Add);
            _a.X = _b;
            _a.ToString().Should().Be("a*b*");
            o.Dispose();
            _a.ToString().Should().Be("ab");
        }

        [Fact]
        public void Observable_Value_Should_React_To_Changing_Source()
        {
            var chain = Chain(start: 'a', count: 3);
            var source = new FakeObservable(chain[0]);
            var observable = new Observable<S>(source, nameof(S.X));
            observable.Value.ToString().Should().Be("bc");
            source.Value = chain[1];
            observable.Value.ToString().Should().Be("c");
        }
        [Fact]
        public void Observable_Should_Notify_About_Changing_Source()
        {
            var chain = Chain(start: 'a', count: 3);
            var source = new FakeObservable(chain[0]);
            var observable = new Observable<S>(source, nameof(S.X));
            observable.Subscribe(s => _log.Add(s.ToString()));
            source.Value = chain[1];
            PopLog().Should().Equal("c");
        }
        [Fact]
        public void Observable_Value_Should_React_To_Replacing_Source_Property()
        {
            var chain = Chain(start: 'a', count: 3);
            var observable = new Observable<S>(
                new FakeObservable(chain[0]), nameof(S.X));
            observable.Value.ToString().Should().Be("bc");
            chain[0].X = chain[2];
            observable.Value.ToString().Should().Be("c");
        }
        [Fact]
        public void Observable_Should_Notify_About_Changing_Source_Property()
        {
            var chain = Chain(start: 'a', count: 3);
            var source = new FakeObservable(chain[0]);
            var observable = new Observable<S>(source, nameof(S.X));
            observable.Subscribe(s => _log.Add(s.ToString()));
            chain[0].X = chain[2];
            PopLog().Should().Equal("c");
        }

        // [Fact]
        public void Observables1()
        {
            var chain = Chain(start: 'a', count: 3);
            var observed = chain[0]
                .Observe<S>(nameof(S.X), _ => _log.Add("1"))
                .Observe<S>(nameof(S.X), _ => _log.Add("2"))
                .Observe<S>(nameof(S.X), _ => _log.Add("3"));
            observed.Value.Should().Be(null);

            chain[2].X = _z;
            observed.Value.Should().Be(_z);
            PopLog().Should().Equal("3");

            chain[1].X = null;
            observed.Value.Should().Be(null);
            PopLog().Should().Equal("2");

            observed.Dispose();
        }
    }
}