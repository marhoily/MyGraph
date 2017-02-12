using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyGraph
{
    public sealed class BindingRegistry<TSource, TTarget> : IDisposable
        where TSource : INotifyPropertyChanged
    {
        private readonly CachingDictionary<TSource, Binding<TSource, TTarget>> _map;
        private readonly List<Action> _disposables = new List<Action>();
        public BindingRegistry(
            Expression<Func<ObservableCollection<TSource>>> source,
            Func<TSource, Binding<TSource, TTarget>> bind)
        {
            _map = new CachingDictionary<TSource, Binding<TSource, TTarget>>(
                src => bind(src).Attach(binding => _map.Remove(binding.Source)));
            _disposables.Add(source.Track(
                added => _map.Get(added),
                removed =>
                {
                    _map.Get(removed).Dispose();
                    _map.Remove(removed);
                }));
        }

        public TTarget GetTarget(TSource source) => _map.Get(source).Target;

        public void Dispose() => _disposables.ForEach(x => x());
    }
}