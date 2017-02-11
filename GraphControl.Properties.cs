using System.Windows;

namespace MyGraph
{
    public sealed partial class GraphControl
    {
        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register("Graph", typeof(IGraph),
            typeof(GraphControl), new PropertyMetadata(default(IGraph), OnGraphChanged));

        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register("NodeTemplate",
            typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty VirtualNodeTemplateProperty =
            DependencyProperty.Register("VirtualNodeTemplate", typeof(DataTemplate), typeof(GraphControl),
                new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty EdgeTemplateProperty = DependencyProperty.Register(
            "EdgeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty VirtualEdgeTemplateProperty = DependencyProperty.Register(
            "VirtualEdgeTemplate", typeof(DataTemplate), typeof(GraphControl),
            new PropertyMetadata(default(DataTemplate)));

        public IGraph Graph
        {
            get { return (IGraph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate) GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }

        public DataTemplate VirtualNodeTemplate
        {
            get { return (DataTemplate) GetValue(VirtualNodeTemplateProperty); }
            set { SetValue(VirtualNodeTemplateProperty, value); }
        }

        public DataTemplate EdgeTemplate
        {
            get { return (DataTemplate) GetValue(EdgeTemplateProperty); }
            set { SetValue(EdgeTemplateProperty, value); }
        }

        public DataTemplate VirtualEdgeTemplate
        {
            get { return (DataTemplate) GetValue(VirtualEdgeTemplateProperty); }
            set { SetValue(VirtualEdgeTemplateProperty, value); }
        }
    }
}