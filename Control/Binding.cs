using System;
using System.Collections.Generic;

namespace MyGraph
{
    public sealed class Binding<TSource, TDestination> : IDisposable
    {
        public TSource Source { get; }
        public TDestination Destination { get; }
        private readonly List<Action> _disposables = new List<Action>();
        public Binding(TSource source, TDestination destination)
        {
            Source = source;
            Destination = destination;
        }

        public Binding<TSource, TDestination> Attach(Action<Binding<TSource, TDestination>> onDispose)
        {
            _disposables.Add(() => onDispose(this));
            return this;
        }

        public void Dispose() => _disposables.ForEach(x => x());
    }
}