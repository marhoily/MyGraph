using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MyGraph
{
    public static class HierarchyExtensions
    {
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject src)
        {
            var p = VisualTreeHelper.GetParent(src);
            while (p != null)
            {
                yield return p;
                p = VisualTreeHelper.GetParent(p);
            }
        }

        public static IEnumerable<DependencyObject> Children(this DependencyObject src)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(src); i++)
                yield return VisualTreeHelper.GetChild(src, i);
        }

        public static IEnumerable<DependencyObject> Descendants(this DependencyObject src)
        {
            foreach (var child in src.Children())
            {
                yield return child;
                foreach (var descendant in child.Descendants())
                    yield return descendant;
            }
        }

        public static T Parent<T>(this DependencyObject src) where T : DependencyObject
        {
            return Ancestors(src).OfType<T>().FirstOrDefault();
        }
    }
}