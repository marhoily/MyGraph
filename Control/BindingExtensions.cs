using System.ComponentModel;

namespace MyGraph
{
    public static class BindingExtensions
    {
        public static Binding<TSource, TDestination> Bind<TSource, TDestination>(
            this TDestination destination, TSource source) where TSource : INotifyPropertyChanged
        {
            return new Binding<TSource, TDestination>(source, destination);
        }
    }
}