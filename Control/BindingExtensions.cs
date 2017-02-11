using System.ComponentModel;
using System.Windows;

namespace MyGraph
{
    public static class BindingExtensions
    {
        public static Binding<TSource, TTarget> Bind<TSource, TTarget>(
            this TTarget target, TSource source) 
            where TSource : INotifyPropertyChanged
            where TTarget : DependencyObject
        {
            Caliburn.Micro.Bind.SetModel(target, source);
            return new Binding<TSource, TTarget>(source, target);
        }
    }
}