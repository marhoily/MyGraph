using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using static System.Collections.Specialized.NotifyCollectionChangedAction;

namespace MyGraph
{
    public interface IObservable<out T> : IDisposable
    {
        T Value { get; }
        void Subscribe(Action<T> handler);
    }

    public abstract class Disposables : IDisposable
    {
        public List<Action> Resources { get; } = new List<Action>();
        public void Dispose() => Resources.ForEach(dispose => dispose());
    }
    public sealed class Npc<T> : Disposables, IObservable<T>
    {
        private INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly List<Action<T>> _changed = new List<Action<T>>();

        public Npc(string propertyName)
        {
            _propertyName = propertyName;
            Resources.Add(Unsubscribe);
        }

        public T Value { get; private set; }
        public void Subscribe(Action<T> handler) => _changed.Add(handler);

        public void ChangeSource(INotifyPropertyChanged source)
        {
            if (ReferenceEquals(_source, source))
                return;
            if (_source != null)
            {
                _source.PropertyChanged -= OnPropertyChanged;
            }
            _source = source;
            UpdateValue();
            if (_source != null)
            {
                _source.PropertyChanged += OnPropertyChanged;
            }
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
                UpdateValue();
        }
        private void UpdateValue()
        {
            var value = (T)_source?
                .GetType()
                .GetProperty(_propertyName)
                .GetValue(_source);

            if (Equals(Value, value))
                return;
            Value = value;
            _changed.ForEach(handle => handle(value));
        }
        private void Unsubscribe()
        {
            if (_source != null)
                _source.PropertyChanged -= OnPropertyChanged;
        }
    }

    public static class NpcExtensions
    {
        public static Action Track<T>(this INotifyPropertyChanged trackable, string propertyName, Action<T> onChanged)
        {
            if (trackable == null) return () => { };
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e.PropertyName == propertyName) onChanged((T)s);
            };
            trackable.PropertyChanged += handler;
            return () => trackable.PropertyChanged -= handler;
        }

        public static IObservable<TResult> Observe<TSource, TResult>(this TSource source,
            Expression<Func<TSource, TResult>> pathExpression, Action<TResult> handler)
            where TSource : INotifyPropertyChanged
        {
            return source.Observe(handler, pathExpression
                .ExtractPropertyNames().Skip(count: 1).ToArray());
        }
        public static IObservable<T> Observe<T>(
            this INotifyPropertyChanged source, Action<T> handler, params string[] path)
        {
            if (path.Length == 0) throw new ArgumentOutOfRangeException(nameof(path));
            if (path.Length == 1) return source.Observe(path.Single(), handler);
            var first = source.Observe<INotifyPropertyChanged>(path.First());
            var middle = path.Skip(count: 1).Take(path.Length - 2);
            return middle.Aggregate(first, (current, part) 
                    => current.Observe<INotifyPropertyChanged>(part))
                .Observe(path.Last(), handler);
        }
        public static IObservable<T> Observe<T>(this INotifyPropertyChanged source, string propertyName, Action<T> handler = null)
        {
            return CreateNpc(source, propertyName, handler);
        }
        public static IObservable<T> Observe<T>(this IObservable<INotifyPropertyChanged> source, string propertyName, Action<T> handler = null)
        {
            var npc = CreateNpc(source.Value, propertyName, handler);
            npc.Resources.Add(source.Dispose);
            source.Subscribe(npc.ChangeSource);
            return npc;
        }

        private static Npc<T> CreateNpc<T>(INotifyPropertyChanged source, string propertyName, Action<T> handler)
        {
            var npc = new Npc<T>(propertyName);
            npc.ChangeSource(source);
            if (handler != null) npc.Subscribe(handler);
            return npc;
        }

        public static Action Track<T>(
            this Expression<Func<ObservableCollection<T>>> exp,
            Action<T> added, Action<T> removed)
        {
            var getCollection = exp.Compile();
            var collection = getCollection();
            NotifyCollectionChangedEventHandler handler = (s, e) =>
            {
                switch (e.Action)
                {
                    case Add:
                        foreach (T item in e.NewItems)
                            added(item);
                        break;
                    case Remove:
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
    }
}