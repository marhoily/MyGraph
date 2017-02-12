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

    internal abstract class ResourceManager : IDisposable
    {
        public List<Action> Resources { get; } = new List<Action>();
        public void Dispose() => Resources.ForEach(dispose => dispose());
    }
    internal sealed class Npc<T> : ResourceManager, IObservable<T>
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly List<Action<T>> _changed = new List<Action<T>>();

        public Npc(string propertyName)
        {
            _propertyName = propertyName;
            Resources.Add(Unsubscribe);
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
            UpdateValue();
            if (_source != null)
            {
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
            var value = (T)_source?
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(_source);

            if (Equals(Value, value))
                return;
            Value = value;
            _changed.ForEach(handle => handle(value));
        }
        private void Unsubscribe()
        {
            if (_source != null)
                _source.PropertyChanged -= OnPropertyChanged;
        }
    }

    internal static class Ext
    {
        public static IObservable<T> Observe<T>(this INotifyPropertyChanged source, string propertyName, Action<T> handler = null)
        {
            return CreateNpc(source, propertyName, handler);
        }
        public static IObservable<T> Observe<T>(this IObservable<INotifyPropertyChanged> source, string propertyName, Action<T> handler = null)
        {
            var npc = CreateNpc(source.Value, propertyName, handler);
            npc.Resources.Add(source.Dispose);
            source.Subscribe(npc.ChangeSource);
            return npc;
        }

        private static Npc<T> CreateNpc<T>(INotifyPropertyChanged source, string propertyName, Action<T> handler)
        {
            var npc = new Npc<T>(propertyName);
            npc.ChangeSource(source);
            if (handler != null) npc.Subscribe(handler);
            return npc;
        }
    }
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
            var observed = chain[0]
                .Observe<S>(nameof(S.X), _ => _log.Add("1"))
                .Observe<S>(nameof(S.X), _ => _log.Add("2"))
                .Observe<S>(nameof(S.X), _ => _log.Add("3"));
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