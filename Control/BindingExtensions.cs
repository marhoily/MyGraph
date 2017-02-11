using System.ComponentModel;
using System.Windows;

namespace MyGraph
{
    public static class BindingExtensions
    {
        public static Binding<TSource, TDestination> Bind<TSource, TDestination>(
            this TDestination destination, TSource source) 
            where TSource : INotifyPropertyChanged
            where TDestination : DependencyObject
        {
            Caliburn.Micro.Bind.SetModel(destination, source);
            return new Binding<TSource, TDestination>(source, destination);
        }
    }
}