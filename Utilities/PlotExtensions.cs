using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Expression = System.Linq.Expressions.Expression;

namespace MyGraph
{
    public static class PlotExtensions
    {
        public static List<string> GetPropertyNames<TTarget, T2>(this Expression<Action<TTarget, T2>> exp)
        {
            var myVisitor = new MyVisitor(typeof(TTarget));
            myVisitor.Visit(exp.Body);
            return myVisitor.Names;
        }

        private sealed class MyVisitor : ExpressionVisitor
        {
            [NotNull]private readonly Type _filter;
            public readonly List<string> Names = new List<string>();

            public MyVisitor([NotNull]Type filter)
            {
                _filter = filter;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var propertyInfo = node.Member as PropertyInfo;
                if (propertyInfo?.DeclaringType == _filter)
                    Names.Add(propertyInfo.Name);
                return base.VisitMember(node);
            }
        }
        public static T Cast<T>(this object element) 
        {
            return (T)element;
        }
        public static T SetDataContext<T>(this T element, object value) where T : FrameworkElement
        {
            Bind.SetModel(element, value);
            return element;
        }
        public static T AddTo<T>(this T element, Plot plot) where T : UIElement
        {
            plot.Children.Add(element);
            return element;
        }
        public static T MoveTo<T>(this T element, Point p) where T : UIElement
        {
            Plot.SetX(element, p.X);
            Plot.SetY(element, p.Y);
            return element;
        }
    }
}