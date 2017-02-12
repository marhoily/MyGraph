// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;
using Tests.RealTracker;

namespace Tests.WithWatcher
{
    internal sealed class Observable : IDisposable
    {
        private readonly Observable _premiseSource;
        private readonly Npc _npc;
        public object Fact { get; private set; }
        public event Action FactChanged;

        public Observable(INotifyPropertyChanged premise, string propertyName)
        {
            _npc = new Npc(propertyName, UpdateValue);
            ChangePremise(premise);
        }

        public Observable(Observable premiseSource, string propertyName)
        {
            _premiseSource = premiseSource;
            _npc = new Npc(propertyName, UpdateValue);
            _premiseSource.FactChanged += OnPremiseChanged;
            OnPremiseChanged();
        }

        private void ChangePremise(INotifyPropertyChanged premise)
        {
            _npc.ChangeSource(premise);
        }

        private void OnPremiseChanged()
        {
            ChangePremise(_premiseSource.Fact as INotifyPropertyChanged);
        }

        private void UpdateValue()
        {
            if (ReferenceEquals(Fact, _npc.Value))
                return;
            Fact = _npc.Value;
            FactChanged?.Invoke();
        }

        public void Dispose()
        {
            _npc.Dispose();
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