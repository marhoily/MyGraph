using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace MyGraph
{
    public sealed class Binding<TSource, TDestination> : IDisposable
        where TSource : INotifyPropertyChanged
    {
        public TSource Source { get; }
        public TDestination Destination { get; }
        private readonly List<Action> _disposables = new List<Action>();
        private readonly Dictionary<string, Action<TSource, TDestination>> 
            _propertyChangedReactions = new Dictionary<string, Action<TSource, TDestination>>();

        public Binding(TSource source, TDestination destination)
        {
            Source = source;
            Destination = destination;
            Source.PropertyChanged += SourceOnPropertyChanged;
            _disposables.Add(() => Source.PropertyChanged -= SourceOnPropertyChanged);
        }

        private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<TSource, TDestination> action;
            if (_propertyChangedReactions.TryGetValue(e.PropertyName, out action))
                action(Source, Destination);
        }

        public Binding<TSource, TDestination> Attach(Action<Binding<TSource, TDestination>> onDispose)
        {
            _disposables.Add(() => onDispose(this));
            return this;
        }

        public void Dispose() => _disposables.ForEach(x => x());

        public Binding<TSource, TDestination> Link(
            Func<TSource, object> propertyName, Action<TSource, TDestination> action)
        {
            action(Source, Destination);
            return OnChange(propertyName, action);
        }
        public Binding<TSource, TDestination> OnChange(
            Func<TSource, object> propertyName, Action<TSource, TDestination> action)
        {
            var exp = propertyName as MemberExpression;
            var prop = exp?.Member as PropertyInfo;
            if (prop == null) throw new ArgumentOutOfRangeException(nameof(propertyName));
            _propertyChangedReactions[prop.Name] = action;
            return this;
        }
    }
}