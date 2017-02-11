using System.Windows;

namespace MyGraph
{
    public partial class GraphControl
    {
        private readonly CachingDictionary<INode, FrameworkElement> _nodes;
        public GraphControl()
        {
            InitializeComponent();
            _nodes = new CachingDictionary<INode, FrameworkElement>(
                node => NodeTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .PlaceAt(_plot, node.Location)
                    .SetDataContext(node));
        }

        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            "Graph", typeof(IGraph), typeof(GraphControl), new PropertyMetadata(default(IGraph), (d, e) => ((GraphControl)d).GraphChanged()));

        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register(
            "NodeTemplate", typeof(DataTemplate), typeof(GraphControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate) GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }
        private void GraphChanged()
        {
            foreach (var node in Graph.Nodes)
                _nodes.Get(node);
        }
        public IGraph Graph
        {
            get { return (IGraph) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

    }
}
