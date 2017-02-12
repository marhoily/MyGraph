using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Tests.FirstTry
{
    internal interface IObservable : IDisposable
    {
        object Value { get; }
        event Action Changed;
    }

    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    internal sealed class Npc : IObservable
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        public object Value { get; private set; }
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
            var value = _source
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(_source);

            if (ReferenceEquals(Value, value))
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
    internal sealed class Compound : IObservable
    {
        private readonly IObservable _premiseSource;
        private readonly Npc _npc;
        public object Value { get; private set; }
        public event Action Changed;

        public Compound(IObservable premiseSource, string propertyName)
        {
            _premiseSource = premiseSource;
            _npc = new Npc(propertyName, changeHandler: UpdateValue);
            _premiseSource.Changed += OnPremiseChanged;
            OnPremiseChanged();
        }

        private void ChangePremise(INotifyPropertyChanged premise)
        {
            _npc.ChangeSource(premise);
        }

        private void OnPremiseChanged()
        {
            ChangePremise(_premiseSource.Value as INotifyPropertyChanged);
        }

        private void UpdateValue()
        {
            if (ReferenceEquals(Value, _npc.Value))
                return;
            Value = _npc.Value;
            Changed?.Invoke();
        }

        public void Dispose()
        {
            _npc.Dispose();
            if (_premiseSource != null)
            {
                _premiseSource.Changed -= OnPremiseChanged;
            }
            else
            {
                1.ToString();
            }
        }
    }

    internal static class Ext
    {
        public static IObservable Observe(this INotifyPropertyChanged source, string propertyName, Action handler = null)
        {
            var observable = new Npc(propertyName, source, handler);
            return observable;
        }
        public static IObservable Observe(this IObservable source, string propertyName, Action handler = null)
        {
            var observable = new Compound(source, propertyName);
            if (handler != null) observable.Changed += handler;
            return observable;
        }
    }
}