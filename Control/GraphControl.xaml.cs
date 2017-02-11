using System;
using System.Collections.Generic;
using System.Windows;

namespace MyGraph
{
    public sealed class BindingRegistry<TSource, TDestination>
    {
        private readonly CachingDictionary<TSource, Binding<TSource, TDestination>> _map;

        public BindingRegistry(Func<TSource, Binding<TSource, TDestination>> bind)
        {
            _map = new CachingDictionary<TSource, Binding<TSource, TDestination>>(
                src => bind(src).Attach(binding => _map.Remove(binding.Source)));
        }

        public TDestination GetDestination(TSource source) => _map.Get(source).Destination;
    }

    public static class BindingExtensions
    {
        public static Binding<TSource, TDestination> Bind<TSource, TDestination>(
            this TDestination destination, TSource source)
        {
            return new Binding<TSource, TDestination>(source, destination);
        }
    }
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
    public partial class GraphControl
    {
        private readonly BindingRegistry<INode, FrameworkElement> _nodes;

        public GraphControl()
        {
            InitializeComponent();
            _nodes = new BindingRegistry<INode, FrameworkElement>(
                node => NodeTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .PlaceAt(_plot, node.Location)
                    .SetDataContext(node)
                    .Bind(node));
        }

        private void GraphChanged()
        {
            foreach (var node in Graph.Nodes)
                _nodes.GetDestination(node);
        }
    }
}
