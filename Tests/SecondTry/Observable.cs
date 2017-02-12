// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;
using MyGraph;

namespace Tests.SecondTry
{
    internal sealed class Watcher
    {
        private readonly string _propertyName;

        public Watcher(string propertyName)
        {
            _propertyName = propertyName;
        }

        public object Look(object premise) => premise
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(premise);

        public Action Track(INotifyPropertyChanged premise, Action onChange)
            => premise.Track(_propertyName, onChange);
    }
    internal sealed class Observable : IDisposable
    {
        private readonly Observable _premiseSource;
        private INotifyPropertyChanged _premise;
        private readonly Watcher _watcher;
        public object Fact { get; private set; }
        private Action _disposePremiseTracking;
        public event Action FactChanged;

        public Observable(INotifyPropertyChanged premise, Watcher watcher)
        {
            _watcher = watcher;
            ChangePremise(premise);
        }
        public Observable(Observable premiseSource, Watcher watcher)
        {
            _premiseSource = premiseSource;
            _watcher = watcher;
            _premiseSource.FactChanged += OnPremiseChanged;
            OnPremiseChanged();
        }

        private void ChangePremise(INotifyPropertyChanged premise)
        {
            if (ReferenceEquals(_premise, premise))
                return;
            if (_premise != null)
            {
                _disposePremiseTracking?.Invoke();
                _disposePremiseTracking = null;
            }
            else
            {
                1.ToString();
            }
            _premise = premise;
            if (_premise != null)
            {
                UpdateValue();
                _disposePremiseTracking = _watcher.Track(_premise, UpdateValue);
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
        
        private void UpdateValue()
        {
            var value = _watcher.Look(_premise);
            if (ReferenceEquals(Fact, value))
                return;
            Fact = value;
            FactChanged?.Invoke();
        }

        public void Dispose()
        {
            _disposePremiseTracking?.Invoke();
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
    internal static class Ext
    {
        public static Observable Observe(this INotifyPropertyChanged source, string propertyName, Action handler = null)
        {
            var observable = new Observable(source, new Watcher(propertyName));
            if (handler != null) observable.FactChanged += handler;
            return observable;
        }
        public static Observable Observe(this Observable source, string propertyName, Action handler = null)
        {
            var observable = new Observable(source, new Watcher(propertyName));
            if (handler != null) observable.FactChanged += handler;
            return observable;
        }
    }

}