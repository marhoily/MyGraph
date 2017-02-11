using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace MyGraph
{
    public static class ExpressionExtensions
    {
        public static Action Track<T>(this INotifyPropertyChanged trackable, string propertyName, Action<T> onChanged)
        {
            if (trackable == null) return () => { };
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == propertyName) onChanged((T) s);
            };
            trackable.PropertyChanged += handler;
            return () => trackable.PropertyChanged -= handler;
        }

        public static Action Track<T>(
            this Expression<Func<T>> exp, Action<T> onChanged)
        {
            var v = new AccessListVisitor();
            v.Visit(exp.Body);
            PropertyChangedEventHandler handler = (s, e) =>
            {
            };
            foreach (var expression in v.All)
            {
                var getPartial = Expression.Lambda<Func<object>>(expression, false).Compile();
                var part = getPartial() as INotifyPropertyChanged;
                if (part == null) continue;
                part.PropertyChanged += handler;
                //yield return () => part.PropertyChanged -= handler;
            }
            return () => { };
        }
        public static Action Track<T>(this Expression<Func<ObservableCollection<T>>> exp,
            Action<T> added, Action<T> removed)
        {
            var getCollection = exp.Compile();
            var collection = getCollection();
            NotifyCollectionChangedEventHandler handler = (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (T item in e.NewItems)
                            added(item);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (T item in e.OldItems)
                            removed(item);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            collection.CollectionChanged += handler;
            foreach (var item in collection)
                added(item);

            return () => collection.CollectionChanged -= handler; 
        }
        

        internal sealed class AccessListVisitor : ExpressionVisitor
        {
            public List<Expression> All { get; } = new List<Expression>();

            protected override Expression VisitMember(MemberExpression node)
            {
                All.Add(node.Expression);
                return base.VisitMember(node);
            }
        }


        public static List<string> GetPropertyNames<TTarget, T2>(this Expression<Action<TTarget, T2>> exp)
        {
            var myVisitor = new NameVisitor(typeof(TTarget));
            myVisitor.Visit(exp.Body);
            return myVisitor.Names;
        }

        private sealed class NameVisitor : ExpressionVisitor
        {
            [NotNull]
            private readonly Type _filter;
            public readonly List<string> Names = new List<string>();

            public NameVisitor([NotNull] Type filter)
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
    }
}