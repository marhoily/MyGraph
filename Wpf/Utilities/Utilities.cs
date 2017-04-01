using System;
using System.Windows;
using Caliburn.Micro;
using Action = System.Action;

namespace MyGraph
{
    public static class Utilities
    {
        public static void WhenLoaded(this FrameworkElement element, Func<Action> act)
        {
            Action dispose = null;
            element.Loaded += (s, e) => dispose = act();
            element.Unloaded += (s, e) => dispose();
        }

        public static T Cast<T>(this object element)
        {
            return (T)element;
        }
       
        public static T BindModel<T>(this T element, object model) 
            where T : FrameworkElement
        {
            // Bind.SetModel does set DataContext but it does not do it immediately
            // many data binding errors have time to occur
            element.DataContext = model;
            Bind.SetModel(element, model);
            return element;
        }
    }
}