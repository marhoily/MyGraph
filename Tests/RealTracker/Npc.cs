using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Tests.SecondTry;

namespace Tests.RealTracker
{
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    internal sealed class Npc : IObservable
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        public event Action Changed;
        public object Value { get; private set; }

        public Npc(string propertyName, Action changeHandler)
        {
            _propertyName = propertyName;
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
}