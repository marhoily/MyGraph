using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Tests.FirstTry
{
    internal interface IObservable<out T> : IDisposable
    {
        T Value { get; }
        event Action Changed;
    }

    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    internal sealed class Npc<T> : IObservable<T>
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        public T Value { get; private set; }
        public event Action Changed;

        public Npc(string propertyName, INotifyPropertyChanged source = null, Action changeHandler = null)
        {
            _propertyName = propertyName;
            ChangeSource(source);
            Changed = changeHandler;
        }

        public void ChangeSource(INotifyPropertyChanged source)
        {
            if (ReferenceEquals(_source, source))
                return;
            if (_source != null)
            {
                _source.PropertyChanged -= OnPropertyChanged;
            }
            else
            {
                1.ToString();
            }
            _source = source;
            if (_source != null)
            {
                UpdateValue();
                _source.PropertyChanged += OnPropertyChanged;
            }
            else
            {
                1.ToString();
            }
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
            {
                UpdateValue();
            }
            else
            {
                1.ToString();
            }
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
            Changed?.Invoke();
        }

        public void Dispose()
        {
            if (_source != null)
            {
                _source.PropertyChanged -= OnPropertyChanged;
            }
            else
            {
                1.ToString();
            }
        }
    }
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    internal sealed class Observable<T> : IObservable<T>
    {
        private readonly IObservable<INotifyPropertyChanged> _source;
        private readonly Npc<T> _npc;
        public T Value => _npc.Value;
        public event Action Changed;

        public Observable(IObservable<INotifyPropertyChanged> source, string propertyName)
        {
            _source = source;
            _npc = new Npc<T>(propertyName, changeHandler: () => Changed?.Invoke());
            _source.Changed += OnSourceChanged;
            OnSourceChanged();
        }

        private void OnSourceChanged()
        {
            _npc.ChangeSource(_source.Value);
        }

        public void Dispose()
        {
            _npc.Dispose();
            if (_source != null)
            {
                _source.Changed -= OnSourceChanged;
            }
            else
            {
                1.ToString();
            }
        }
    }

    internal static class Ext
    {
        public static IObservable<T> Observe<T>(this INotifyPropertyChanged source, string propertyName, Action handler = null)
        {
            return new Npc<T>(propertyName, source, handler);
        }
        public static IObservable<T> Observe<T>(this IObservable<INotifyPropertyChanged> source, string propertyName, Action handler = null)
        {
            var observable = new Observable<T>(source, propertyName);
            if (handler != null) observable.Changed += handler;
            return observable;
        }
    }
}