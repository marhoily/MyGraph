using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MyGraph;
using Xunit;
using static Tests.NpcSamples;

namespace Tests.FirstTry
{
    internal interface IObservable<out T> : IDisposable
    {
        T Value { get; }
        event Action<T> Changed;
    }

    internal sealed class Npc<T> : IObservable<T>
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        public T Value { get; private set; }
        private readonly List<Action<T>> _changed = new List<Action<T>>();
        public event Action<T> Changed
        {
            add { _changed.Add(value); }
            remove { _changed.Remove(value); }
        }

        public Npc(string propertyName, INotifyPropertyChanged source = null, Action<T> changeHandler = null)
        {
            _propertyName = propertyName;
            ChangeSource(source);
            if (changeHandler != null) _changed.Add(changeHandler);
        }

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
        public T Value => _npc.Value;
        public event Action<T> Changed;

        public Observable(IObservable<INotifyPropertyChanged> source, string propertyName)
        {
            _source = source;
            _npc = new Npc<T>(propertyName, changeHandler: v => Changed?.Invoke(v));
            _source.Changed += _npc.ChangeSource;
            _npc.ChangeSource(_source.Value);
        }

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
            if (handler != null) observable.Changed += handler;
            return observable;
        }
    }
    public sealed class ObservablesTest
    {
        private readonly S _a = new S("a", null);
        private readonly S _b = new S("b", null);
        private readonly S _c = new S("c", null);
        private readonly List<string> _log = new List<string>();

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
        public void Observables()
        {
            _a.X = _b;
            var observed = _a
                .Observe<S>(nameof(S.X), _ => _log.Add("1"))
                .Observe<string>(nameof(S.Name), _ => _log.Add("2"));
            observed.Value.Should().Be("b");
            _a.X = _c;
            observed.Value.Should().Be("c");
            _log.Should().Equal("1", "2");
            observed.Dispose();
        }
    }
}