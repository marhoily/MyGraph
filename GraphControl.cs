using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace MyGraph
{
    public sealed class GraphControl : UserControl
    {
        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register("Graph", typeof(IGraph), typeof(GraphControl), new PropertyMetadata(default(IGraph), OnGraphChanged));
        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register("NodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty VirtualNodeTemplateProperty = DependencyProperty.Register("VirtualNodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        private readonly Canvas _canvas;
        private readonly Dictionary<INode, UIElement> _nodes = new Dictionary<INode, UIElement>();

        public GraphControl()
        {
            Content = _canvas = new Canvas();
            MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) return;
            var position = e.GetPosition(this);
            if (Graph.VirtualNode == null)
                AddVirtualNode(position);
            else
                Graph.VirtualNode.Location = position;
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
                AddNode(node, NodeTemplate);

            if (Graph.VirtualNode != null)
                DrawVirtualNode();
        }

        private void DrawVirtualNode()
        {
            Debug.Assert(Graph.VirtualNode != null);
            AddNode(Graph.VirtualNode, VirtualNodeTemplate);
        }

        private void AddVirtualNode(Point location)
        {
            Graph.VirtualNode = new VirtualNode(location);
            DrawVirtualNode();
        }

        private void AddNode([NotNull]INode node, DataTemplate template)
        {
            var nodeControl = (UIElement)template.LoadContent();
            Move(nodeControl, node.Location);
            _canvas.Children.Add(nodeControl);
            _nodes[node] = nodeControl;
            node.PropertyChanged += OnNodePropertyChanged;
        }

        private static void Move(UIElement nodeControl, Point location)
        {
            Canvas.SetTop(nodeControl, location.Y);
            Canvas.SetLeft(nodeControl, location.X);

        }
        private void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (INode)sender;
            var element = _nodes[node];
            Move(element, node.Location);
        }


    }
}