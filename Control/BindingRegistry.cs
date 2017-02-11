using System;
using System.ComponentModel;

namespace MyGraph
{
    public sealed class BindingRegistry<TSource, TTarget> 
        where TSource : INotifyPropertyChanged
    {
        private readonly CachingDictionary<TSource, Binding<TSource, TTarget>> _map;

        public BindingRegistry(Func<TSource, Binding<TSource, TTarget>> bind)
        {
            _map = new CachingDictionary<TSource, Binding<TSource, TTarget>>(
                src => bind(src).Attach(binding => _map.Remove(binding.Source)));
        }

        public TTarget GetTarget(TSource source) => _map.Get(source).Target;
    }
}