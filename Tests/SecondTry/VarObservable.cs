// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;
using Tests.RealTracker;

namespace Tests.SecondTry
{
    internal interface IObservable : IDisposable
    {
        object Value { get; }
        event Action Changed;
    }
    internal sealed class VarObservable : IObservable
    {
        private readonly IObservable _premiseSource;
        private readonly Npc _npc;
        public object Value { get; private set; }
        public event Action Changed;
        
        public VarObservable(IObservable premiseSource, string propertyName)
        {
            _premiseSource = premiseSource;
            _npc = new Npc(propertyName, UpdateValue);
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
            var observable = new Npc(propertyName, handler);
            observable.ChangeSource(source);
            return observable;
        }
        public static IObservable Observe(this IObservable source, string propertyName, Action handler = null)
        {
            var observable = new VarObservable(source, propertyName);
            if (handler != null) observable.Changed += handler;
            return observable;
        }
    }

}