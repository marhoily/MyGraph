using System.Collections.Generic;
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
            foreach (var node in Graph.Nodes)
                DrawNode(node, NodeTemplate);

            if (Graph.VirtualNode != null)
                DrawVirtualNode();
        }

        private void DrawVirtualNode()
        {
            Debug.Assert(Graph.VirtualNode != null);
            DrawNode(Graph.VirtualNode, VirtualNodeTemplate);
        }

        private void AddVirtualNode(Point location)
        {
            Graph.VirtualNode = new VirtualNode(location);
            DrawVirtualNode();
        }

        private void DrawNode([NotNull]INode node, DataTemplate template)
        {
            var nodeControl = (FrameworkElement)template.LoadContent();
            Move(nodeControl, node.Location);
            _canvas.Children.Add(nodeControl);
            _nodes[node] = nodeControl;
            node.PropertyChanged += OnNodePropertyChanged;
        }

        private static void Move(FrameworkElement nodeControl, Point location)
        {
            Canvas.SetTop(nodeControl, location.Y-nodeControl.ActualHeight/2);
            Canvas.SetLeft(nodeControl, location.X - nodeControl.ActualWidth / 2);

        }
        private void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (INode)sender;
            var element = _nodes[node];
            Move(element, node.Location);
        }
    }
}