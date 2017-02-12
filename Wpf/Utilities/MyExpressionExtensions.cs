using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace MyGraph
{
    public static class MyExpressionExtensions
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

        public static string GetPropertyName(this Expression<Func<object>> exp)
        {
            var access = exp.Body as MemberExpression;
            var prop = access?.Member as PropertyInfo;
            return prop?.Name;
        }

        struct Access
        {
            public INotifyPropertyChanged Trackable { get; }
            public string PropertyName { get; }

            public Access(INotifyPropertyChanged trackable, string propertyName)
            {
                Trackable = trackable;
                PropertyName = propertyName;
            }
        }
        public static Action Track<T>(this Expression<Func<T>> exp, Action<T> onChanged)
        {
            return Track(GetAccesses(exp), onChanged);
        }

        private static Action Track<T>(IEnumerable<Access> accesses, Action<T> onChanged)
        {
            var first = accesses.FirstOrDefault();
            if (first.PropertyName == null) return () => { };
            var trackFirst = first.Trackable.Track(first.PropertyName, onChanged);
            var trackRest = Track(accesses.Skip(1), onChanged);
            return () => { trackFirst(); trackRest(); };
        }

        private static List<Access> GetAccesses<T>(Expression<Func<T>> exp)
        {
            var subexpressions = ExtractPath(exp).ToList();
            return subexpressions.Zip(subexpressions.Skip(1),
                    (x, y) => new Access(x.Compile()() as INotifyPropertyChanged, y.GetPropertyName()))
                .Where(x => x.Trackable != null && x.PropertyName != null)
                .ToList();
        }

        internal static IEnumerable<Expression<Func<object>>> ExtractPath<T>(Expression<Func<T>> exp)
        {
            var v = new AccessListVisitor(exp.Body);
            v.Visit(exp.Body);
            return v.All.Select(e => Expression.Lambda<Func<object>>(e, false));
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
            public AccessListVisitor(Expression root)
            {
                All.Push(root);
            }

            public Stack<Expression> All { get; } = new Stack<Expression>();

            protected override Expression VisitMember(MemberExpression node)
            {
                All.Push(node.Expression);
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