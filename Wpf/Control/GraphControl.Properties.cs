using System.Windows;

namespace MyGraph
{
    public partial class GraphControl
    {
        public static readonly DependencyProperty VertexTemplateProperty = DependencyProperty.Register(
            "VertexTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate VertexTemplate
        {
            get { return (DataTemplate) GetValue(VertexTemplateProperty); }
            set { SetValue(VertexTemplateProperty, value); }
        }

        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            "Graph", typeof(IGraph), typeof(GraphControl), new PropertyMetadata(default(IGraph)));

        public IGraph Graph
        {
            get { return (IGraph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }
    }
}