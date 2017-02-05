﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyGraph
{
    public sealed class GraphControl : UserControl
    {
        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            "Graph", typeof(IGraph), typeof(GraphControl),
            new PropertyMetadata(default(IGraph), OnGraphChanged));

        private readonly Canvas _canvas;

        public GraphControl()
        {
            Content = _canvas = new Canvas();
        }

        public IGraph Graph
        {
            get { return (IGraph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GraphControl) d).OnGraphChanged();
        }

        private void OnGraphChanged()
        {
            foreach (var node in Graph.Nodes)
            {
                var nodeControl = new Ellipse
                {
                    Fill = Brushes.Chocolate,
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Brown,
                    StrokeThickness = 1
                };
                Canvas.SetTop(nodeControl, node.Location.Y);
                Canvas.SetLeft(nodeControl, node.Location.X);
                _canvas.Children.Add(nodeControl);
            }
        }
    }
}