using System.Windows;

namespace MyGraph
{
    public partial class GraphControl
    {
        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register(
            "NodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate) GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }

        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            "Graph", typeof(IGraph), typeof(GraphControl), new PropertyMetadata(default(IGraph), (d, e) => ((GraphControl)d).GraphChanged()));

        public IGraph Graph
        {
            get { return (IGraph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }
    }
}