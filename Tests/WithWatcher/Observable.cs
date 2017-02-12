// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;
using MyGraph;

namespace Tests.WithWatcher
{
    internal sealed class Watcher
    {
        private readonly string _propertyName;
        private readonly Action _changeHandler;
        private Action _untrack;

        public Watcher(string propertyName, Action changeHandler)
        {
            _propertyName = propertyName;
            _changeHandler = changeHandler;
        }

        public object Look(object premise) => premise
            .GetType()
            .GetProperty(_propertyName)
            .GetValue(premise);

        public void Track(INotifyPropertyChanged premise)
        {
            _changeHandler();
            _untrack = premise.Track(_propertyName, _changeHandler);
        }

        public void Untrack()
        {
            _untrack?.Invoke();
            _untrack = null;
        }
    }

    internal sealed class Observable : IDisposable
    {
        private readonly Observable _premiseSource;
        private INotifyPropertyChanged _premise;
        private readonly Watcher _watcher;
        public object Fact { get; private set; }
        public event Action FactChanged;

        public Observable(INotifyPropertyChanged premise, string propertyName)
        {
            _watcher = new Watcher(propertyName, UpdateValue);
            ChangePremise(premise);
        }

        public Observable(Observable premiseSource, string propertyName)
        {
            _premiseSource = premiseSource;
            _watcher = new Watcher(propertyName, UpdateValue);
            _premiseSource.FactChanged += OnPremiseChanged;
            OnPremiseChanged();
        }

        private void ChangePremise(INotifyPropertyChanged premise)
        {
            if (ReferenceEquals(_premise, premise))
                return;
            if (_premise != null)
            {
                _watcher.Untrack();
            }
            else
            {
                1.ToString();
            }
            _premise = premise;
            if (_premise != null)
            {
                _watcher.Track(_premise);
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
            _watcher.Untrack();
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
            var observable = new Observable(source, propertyName);
            if (handler != null) observable.FactChanged += handler;
            return observable;
        }

        public static Observable Observe(this Observable source, string propertyName, Action handler = null)
        {
            var observable = new Observable(source, propertyName);
            if (handler != null) observable.FactChanged += handler;
            return observable;
        }
    }
}