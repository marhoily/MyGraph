﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace MyGraph
{
    public sealed partial class GraphControl : UserControl
    {
        private readonly Canvas _canvas;
        private readonly Dictionary<IEdge, FrameworkElement> _edges = new Dictionary<IEdge, FrameworkElement>();
        private readonly Dictionary<INode, FrameworkElement> _nodes = new Dictionary<INode, FrameworkElement>();
        private IEdge _virtualEdge;
        private INode _virtualNode;

        public GraphControl()
        {
            Content = _canvas = new Canvas();
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            Background = Brushes.Transparent;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_virtualEdge == null) return;
            _virtualEdge.B.Location = e.GetPosition(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                if (MapControl(e.OriginalSource))
                {
                    var position = e.GetPosition(this);
                    if (Graph.VirtualNode == null)
                        AddVirtualNode(position);
                    else
                        Graph.VirtualNode.Location = position;
                    e.Handled = false;
                }

            _virtualEdge = null;
        }

        private bool MapControl(object obj)
        {
            var e = obj as FrameworkElement;
            return e != null && e.DataContext == DataContext;
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

            foreach (var edge in Graph.Edges)
                Add(edge, EdgeTemplate);

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
            Graph.VirtualNode = new Node(location);
        }

        private void Add([NotNull] INode node, [NotNull] DataTemplate template)
        {
            var nodeControl = (FrameworkElement)template.LoadContent();
            Bind.SetModel(nodeControl, node);
            MoveWhenResized(nodeControl, node.Location);
            _canvas.Children.Add(nodeControl);
            _nodes[node] = nodeControl;
            node.PropertyChanged += OnNodePropertyChanged;
        }

        private void Add([NotNull] IEdge edge, [NotNull] DataTemplate template)
        {
            var edgeControl = (Line)template.LoadContent();
            Bind.SetModel(edgeControl, edge);
            edgeControl.Width = ActualWidth;
            edgeControl.Height = ActualHeight;
            Panel.SetZIndex(edgeControl, value: -1);
            MoveEdge(edge, edgeControl);
            _canvas.Children.Add(edgeControl);
            _edges[edge] = edgeControl;
        }

        private static void MoveEdge(IEdge edge, Line edgeControl)
        {
            edgeControl.X1 = edge.A.Location.X;
            edgeControl.Y1 = edge.A.Location.Y;
            edgeControl.X2 = edge.B.Location.X;
            edgeControl.Y2 = edge.B.Location.Y;
        }


        private static void MoveWhenResized(FrameworkElement nodeControl, Point nodeLocation)
        {
            SizeChangedEventHandler resized = (s, e) => { Move(nodeControl, nodeLocation); };
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

        private void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (INode)sender;
            var element = _nodes[node];
            switch (e.PropertyName)
            {
                case nameof(INode.Location):
                    Move(element, node.Location);
                    MoveEdges(node);
                    break;
                case nameof(INode.IsEdgeStart):
                    var virtualNode = Graph.CreateVirtualNode(Mouse.GetPosition(this));
                    Add(virtualNode, VirtualNodeTemplate);
                    _virtualEdge = Graph.CreateVirtualEdge(node, virtualNode);
                    Add(_virtualEdge, VirtualEdgeTemplate);
                    break;
            }
        }

        private void MoveEdges(INode node)
        {
            foreach (var edge in _edges.Keys)
                if (edge.A == node || edge.B == node)
                    MoveEdge(edge, (Line)_edges[edge]);
        }

    }
}