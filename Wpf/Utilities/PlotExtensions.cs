using System.Windows;

namespace MyGraph
{
    public static class UtilitarianExtensions
    {
        public static T Cast<T>(this object element)
        {
            return (T)element;
        }
       
        public static T MoveTo<T>(this T element, Point p) where T : UIElement
        {
            Plot.SetX(element, p.X);
            Plot.SetY(element, p.Y);
            return element;
        }
    }
}