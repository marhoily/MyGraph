using System.Collections.Generic;
using System.Windows;

namespace MyGraph
{
    public static class VisualExtensions
    {
        public static IEnumerable<T> AndVisualParents<T>(this object src)
            where T : FrameworkElement
        {
            var curr = src as FrameworkElement;
            while (curr != null)
            {
                yield return curr as T;
                curr = curr.Parent as FrameworkElement;
            }
        }
    }
}