using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public static class PlotExtensions
    {
        public static T Cast<T>(this object element) 
        {
            return (T)element;
        }
        public static T SetDataContext<T>(this T element, object value) where T : FrameworkElement
        {
            Bind.SetModel(element, value);
            return element;
        }
        public static T PlaceAt<T>(this T element, Plot plot, Point p) where T : UIElement
        {
            Plot.SetX(element, p.X);
            Plot.SetY(element, p.Y);
            plot.Children.Add(element);
            return element;
        }
    }
}