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

    public sealed class CollectionObserver<T> : IDisposable
    {
        private readonly IObservable<ObservableCollection<T>> _collectionSource;
        private ObservableCollection<T> _collection;
        private readonly Action<T> _added;
        private readonly Action<T> _removed;

        public CollectionObserver(
            IObservable<ObservableCollection<T>> collectionSource,
            Action<T> added, Action<T> removed)
        {
            _collectionSource = collectionSource;
            _added = added;
            _removed = removed;
            UpdateCollection(collectionSource.Value);
            collectionSource.Subscribe(UpdateCollection);
        }

        private void UpdateCollection(ObservableCollection<T> collection)
        {
            if (_collection != null)
            {
                _collection.CollectionChanged -= Handler;
                foreach (var item in _collection)
                    _removed(item);
            }
            _collection = collection;
            if (_collection != null)
            {
                _collection.CollectionChanged += Handler;
                foreach (var item in _collection)
                    _added(item);
            }
        }
        private void Handler(object s, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case Add:
                    foreach (T item in e.NewItems)
                        _added(item);
                    break;
                case Remove:
                    foreach (T item in e.OldItems)
                        _removed(item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            _collectionSource.Dispose();
            if (_collection != null)
                _collection.CollectionChanged -= Handler;
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

        public static IObservable<TResult> Track<TResult>(this INotifyPropertyChanged source, Expression<Func<TResult>> pathExpression)
        {
            return source.Track<TResult>(pathExpression
                .ExtractPropertyNames().Skip(count: 1).ToArray());
        }
        public static IObservable<TResult> Track<TSource, TResult>(this TSource source, Expression<Func<TSource, TResult>> pathExpression)
            where TSource : INotifyPropertyChanged
        {
            return source.Track<TResult>(pathExpression
                .ExtractPropertyNames().Skip(count: 1).ToArray());
        }
        public static IObservable<T> Track<T>(
            this INotifyPropertyChanged source, params string[] path)
        {
            if (path.Length == 0) throw new ArgumentOutOfRangeException(nameof(path));
            if (path.Length == 1) return source.Observe<T>(path.Single());
            var first = source.Observe<INotifyPropertyChanged>(path.First());
            var middle = path.Skip(count: 1).Take(path.Length - 2);
            return middle.Aggregate(first, (current, part) 
                    => current.Observe<INotifyPropertyChanged>(part))
                .Observe<T>(path.Last());
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
            var npc = typeof(INotifyPropertyChanged);
            var goodPart = exp.ExtractSubexpressions()
                .SkipWhile(e => !npc.IsAssignableFrom(e.Body.Type))
                .ToList();
            var first = goodPart
                .Select(e => e.Compile()())
                .OfType<INotifyPropertyChanged> ()
                .First();
            var observable = first.Track<ObservableCollection<T>>(
                goodPart.Select(e => e.GetPropertyName()).Skip(1).ToArray());
            return observable.Track(added, removed).Dispose;
        }

        public static CollectionObserver<T> Track<T>(
            this IObservable<ObservableCollection<T>> collectionSource,
            Action<T> added, Action<T> removed)
        {
            return new CollectionObserver<T>(collectionSource, added, removed);
        }
    }
}