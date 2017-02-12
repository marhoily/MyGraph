using System;
using System.ComponentModel;

namespace Tests.RealTracker
{
    internal sealed class Watcher : IDisposable
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly Action _valueChanged;
        public object Value { get; private set; }

        public Watcher(string propertyName, Action valueChanged)
        {
            _propertyName = propertyName;
            _valueChanged = valueChanged;
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
            _valueChanged?.Invoke();
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