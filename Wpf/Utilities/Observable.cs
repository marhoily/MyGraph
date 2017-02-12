using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyGraph
{
    public interface IObservable<out T> : IDisposable
    {
        T Value { get; }
        void Subscribe(Action<T> handler);
    }

    public abstract class Disposables : IDisposable
    {
        public List<Action> Resources { get; } = new List<Action>();
        public void Dispose() => Resources.ForEach(dispose => dispose());
    }
    public sealed class Npc<T> : Disposables, IObservable<T>
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

    public static class Ext
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
}