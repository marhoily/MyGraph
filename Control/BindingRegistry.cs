using System;
using System.ComponentModel;

namespace MyGraph
{
    public sealed class BindingRegistry<TSource, TDestination> 
        where TSource : INotifyPropertyChanged
    {
        private readonly CachingDictionary<TSource, Binding<TSource, TDestination>> _map;

        public BindingRegistry(Func<TSource, Binding<TSource, TDestination>> bind)
        {
            _map = new CachingDictionary<TSource, Binding<TSource, TDestination>>(
                src => bind(src).Attach(binding => _map.Remove(binding.Source)));
        }

        public TDestination GetDestination(TSource source) => _map.Get(source).Destination;
    }
}