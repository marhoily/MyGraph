// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Tests.SecondTry
{
    internal interface IObservable : IDisposable
    {
        object Fact { get; }
        event Action FactChanged;
    }
    internal sealed class VarObservable : IObservable
    {
        private readonly IObservable _premiseSource;
        private INotifyPropertyChanged _premise;
        private readonly string _propertyName;
        public object Fact { get; private set; }
        public event Action FactChanged;

        public VarObservable(IObservable premiseSource, string propertyName)
        {
            _premiseSource = premiseSource;
            _propertyName = propertyName;
            _premiseSource.FactChanged += OnPremiseChanged;
            OnPremiseChanged();
        }

        private void ChangePremise(INotifyPropertyChanged premise)
        {
            if (ReferenceEquals(_premise, premise))
                return;
            if (_premise != null)
            {
                _premise.PropertyChanged -= OnPropertyChanged;
            }
            else
            {
                1.ToString();
            }
            _premise = premise;
            if (_premise != null)
            {
                UpdateValue();
                _premise.PropertyChanged += OnPropertyChanged;
            }
            else
            {
                1.ToString();
            }
        }
        private void OnPremiseChanged()
        {
            ChangePremise(_premiseSource.Fact as INotifyPropertyChanged);
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
            var value = _premise
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(_premise);

            if (ReferenceEquals(Fact, value))
                return;
            Fact = value;
            FactChanged?.Invoke();
        }

        public void Dispose()
        {
            if (_premise != null)
            {
                _premise.PropertyChanged -= OnPropertyChanged;
            }
            else
            {
                1.ToString();
            }
            if (_premiseSource != null)
            {
                _premiseSource.FactChanged -= OnPremiseChanged;
            }
            else
            {
                1.ToString();
            }
        }
    }
    internal sealed class ConstObservable : IObservable
    {
        private readonly INotifyPropertyChanged _premise;
        private readonly string _propertyName;

        public object Fact => _premise
            .GetType()
            .GetProperty(_propertyName)
            .GetValue(_premise);
        public event Action FactChanged;

        public ConstObservable([NotNull]INotifyPropertyChanged premise, string propertyName)
        {
            _propertyName = propertyName;
            _premise = premise;
            _premise.PropertyChanged += OnPropertyChanged;
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
            {
                FactChanged?.Invoke();
            }
            else
            {
                1.ToString();
            }
        }

        public void Dispose()
        {
            if (_premise != null)
            {
                _premise.PropertyChanged -= OnPropertyChanged;
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
            var observable = new ConstObservable(source, propertyName);
            if (handler != null) observable.FactChanged += handler;
            return observable;
        }
        public static IObservable Observe(this IObservable source, string propertyName, Action handler = null)
        {
            var observable = new VarObservable(source, propertyName);
            if (handler != null) observable.FactChanged += handler;
            return observable;
        }
    }

}