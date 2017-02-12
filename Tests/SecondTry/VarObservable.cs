// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Tests.RealTracker;

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
        private readonly Npc _npc;
        public object Fact { get; private set; }
        public event Action FactChanged;
        
        public VarObservable(IObservable premiseSource, string propertyName)
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
    internal sealed class ConstObservable : IObservable
    {
        private readonly Npc _npc;
        public object Fact => _npc.Value;
        public event Action FactChanged;

        public ConstObservable([NotNull]INotifyPropertyChanged premise, string propertyName)
        {
            _npc = new Npc(propertyName, () => FactChanged?.Invoke());
            _npc.ChangeSource(premise);
        }
        public void Dispose() => _npc.Dispose();
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