using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

namespace MyGraph
{
    public sealed class GraphControl : UserControl
    {
        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register("Graph", typeof(IGraph), typeof(GraphControl), new PropertyMetadata(default(IGraph), OnGraphChanged));
        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register("NodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty VirtualNodeTemplateProperty = DependencyProperty.Register("VirtualNodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        private readonly Canvas _canvas;
        private INode _virtualNode;
        private readonly Dictionary<INode, FrameworkElement> _nodes = new Dictionary<INode, FrameworkElement>();

        public GraphControl()
        {
            Content = _canvas = new Canvas();
            MouseUp += OnMouseUp;
            Background = Brushes.Transparent;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) return;
            var position = e.GetPosition(this);
            if (Graph.VirtualNode == null)
                AddVirtualNode(position);
            else
                Graph.VirtualNode.Location = position;
            e.Handled = false;
        }


        public IGraph Graph
        {
            get { return (IGraph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate)GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }
        public DataTemplate VirtualNodeTemplate
        {
            get { return (DataTemplate)GetValue(VirtualNodeTemplateProperty); }
            set { SetValue(VirtualNodeTemplateProperty, value); }
        }

        private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GraphControl)d).OnGraphChanged();
        }
        private void OnGraphChanged()
        {
            Graph.Nodes.CollectionChanged += NodesOnCollectionChanged;
            Graph.PropertyChanged += GraphOnPropertyChanged;

            foreach (var node in Graph.Nodes)
                Add(node, NodeTemplate);

            if (Graph.VirtualNode != null)
                DrawVirtualNode();
        }

        private void NodesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var node in e.NewItems)
                        Add((INode)node, NodeTemplate);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var node in e.NewItems)
                        Remove((INode)node);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GraphOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IGraph.VirtualNode):
                    if (_virtualNode != null)
                        Remove(_virtualNode);
                    _virtualNode = Graph.VirtualNode;
                    if (_virtualNode != null)
                        DrawVirtualNode();
                    break;
            }
        }



        private void DrawVirtualNode()
        {
            Debug.Assert(Graph.VirtualNode != null);
            _virtualNode = Graph.VirtualNode;
            Add(_virtualNode, VirtualNodeTemplate);
        }

        private void AddVirtualNode(Point location)
        {
            Graph.VirtualNode = new VirtualNode(location);
        }

        private void Add([NotNull]INode node, DataTemplate template)
        {
            var nodeControl = (FrameworkElement)template.LoadContent();
            MoveWhenInitialized(nodeControl, node.Location);
            Move(nodeControl, node.Location);
            _canvas.Children.Add(nodeControl);
            _nodes[node] = nodeControl;
            node.PropertyChanged += OnNodePropertyChanged;
        }

        private static void MoveWhenInitialized(FrameworkElement nodeControl, Point nodeLocation)
        {
            SizeChangedEventHandler resized = (s, e) =>
            {
                Move(nodeControl, nodeLocation);
            };
            nodeControl.SizeChanged += resized;
            RoutedEventHandler[] unloaded = { null };
            unloaded[0] = (s, e) =>
            {
                nodeControl.SizeChanged -= resized;
                nodeControl.Unloaded -= unloaded[0];
            };
            nodeControl.Unloaded += unloaded[0];
        }


        private void Remove(INode n)
        {
            _canvas.Children.Remove(_nodes[n]);
            _nodes.Remove(n);
            n.PropertyChanged -= OnNodePropertyChanged;
        }

        private static void Move(FrameworkElement nodeControl, Point location)
        {
            Canvas.SetTop(nodeControl, location.Y - nodeControl.ActualHeight / 2);
            Canvas.SetLeft(nodeControl, location.X - nodeControl.ActualWidth / 2);

        }

       // protected override Size ArrangeOverride(Size arrangeBounds)
       // {
       //     foreach (var p in _nodes)
       //         Move(p.Value, p.Key.Location);
       //     return base.ArrangeOverride(arrangeBounds);
       // }
       //
        private void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (INode)sender;
            var element = _nodes[node];
            Move(element, node.Location);
        }
    }
}