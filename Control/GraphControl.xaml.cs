using System.Windows;

namespace MyGraph
{
    public partial class GraphControl
    {
        private readonly BindingRegistry<IVertex, FrameworkElement> _vertices;

        public GraphControl()
        {
            InitializeComponent();
            _vertices = new BindingRegistry<IVertex, FrameworkElement>(
                vertex => VertexTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .Bind(vertex)
                    .Link((v, c) => c.MoveTo(v.Location))
                    .LinkTarget(c => _plot.Children.Add(c), c =>_plot.Children.Remove(c)));
        }

        private void GraphChanged()
        {
            foreach (var vertex in Graph.Vertices)
                _vertices.GetDestination(vertex);
        }
    }
}
