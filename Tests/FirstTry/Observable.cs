// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;
using System.ComponentModel;

namespace Tests.FirstTry
{
    internal sealed class Observable : IDisposable
    {
        private readonly Observable _premiseSource;
        private INotifyPropertyChanged _premise;
        public object Conclusion { get; private set; }
        public event Action ConclusionChanged;
        private readonly string _propertyName;

        public Observable(INotifyPropertyChanged premise, string propertyName)
        {
            _propertyName = propertyName;
            UpdatePremise(premise);
        }
        public Observable(Observable premiseSource, string propertyName)
        {
            _premiseSource = premiseSource;
            _propertyName = propertyName;
            _premiseSource.ConclusionChanged += WhenPremiseChanged;
            UpdatePremise(_premiseSource.Conclusion as INotifyPropertyChanged);
        }

        private void WhenPremiseChanged()
        {
            UpdatePremise(_premiseSource.Conclusion as INotifyPropertyChanged);
        }
        private void UpdatePremise(INotifyPropertyChanged premise)
        {
            if (ReferenceEquals(_premise, premise))
                return;
            if (_premise != null)
            {
                _premise.PropertyChanged -= WhenConclusionChanged;
            }
            else
            {
                1.ToString();
            }
            _premise = premise;
            if (_premise != null)
            {
                UpdateConclusion();
                _premise.PropertyChanged += WhenConclusionChanged;
            }
            else
            {
                1.ToString();
            }
        }
        private void WhenConclusionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
            {
                UpdateConclusion();
            }
            else
            {
                1.ToString();
            }
        }
        private void UpdateConclusion()
        {
            var value = _premise
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(_premise);

            if (ReferenceEquals(Conclusion, value))
                return;
            Conclusion = value;
            ConclusionChanged?.Invoke();
        }

        public void Dispose()
        {
            if (_premise != null)
            {
                _premise.PropertyChanged -= WhenConclusionChanged;
            }
            else
            {
                1.ToString();
            }
            if (_premiseSource != null)
            {
                _premiseSource.ConclusionChanged -= WhenPremiseChanged;
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
            if (handler != null) observable.ConclusionChanged += handler;
            return observable;
        }
        public static Observable Observe(this Observable source, string propertyName, Action handler = null)
        {
            var observable = new Observable(source, propertyName);
            if (handler != null) observable.ConclusionChanged += handler;
            return observable;
        }
    }

}