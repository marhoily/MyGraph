using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyGraph
{
    public sealed class Binding<TSource, TTarget> : IDisposable
        where TSource : INotifyPropertyChanged
    {
        public TSource Source { get; }
        public TTarget Target { get; }
        private readonly List<Action> _disposables = new List<Action>();
        private readonly Dictionary<string, Action<TSource, TTarget>>
            _propertyChangedReactions = new Dictionary<string, Action<TSource, TTarget>>();

        public Binding(TSource source, TTarget target)
        {
            Source = source;
            Target = target;
            Source.PropertyChanged += SourceOnPropertyChanged;
            _disposables.Add(() => Source.PropertyChanged -= SourceOnPropertyChanged);
        }

        private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<TSource, TTarget> action;
            if (_propertyChangedReactions.TryGetValue(e.PropertyName, out action))
                action(Source, Target);
        }

        public Binding<TSource, TTarget> Attach(Action<Binding<TSource, TTarget>> onDispose)
        {
            _disposables.Add(() => onDispose(this));
            return this;
        }

        public void Dispose() => _disposables.ForEach(x => x());

        public Binding<TSource, TTarget> Link(Expression<Action<TSource, TTarget>> action)
        {
            var compiledAction = action.Compile();
            compiledAction(Source, Target);
            foreach (var propertyName in action.GetPropertyNames())
                _propertyChangedReactions[propertyName] = compiledAction;
            return this;
        }

        public Binding<TSource, TTarget> LinkTarget(Action<TTarget> establish, Action<TTarget> destroy)
        {
            establish(Target);
            _disposables.Add(() => destroy(Target));
            return this;
        }
    }
}