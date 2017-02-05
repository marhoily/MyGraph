using System.Windows;
using System.Windows.Controls;

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
                var nodeControl = (UIElement) NodeTemplate.LoadContent();

                Canvas.SetTop(nodeControl, node.Location.Y);
                Canvas.SetLeft(nodeControl, node.Location.X);
                _canvas.Children.Add(nodeControl);
            }
        }

        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register(
            "NodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate) GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }
    }
}