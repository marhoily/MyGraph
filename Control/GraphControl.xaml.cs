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
                    .AddTo(_plot)
                    .SetDataContext(vertex)
                    .Bind(vertex)
                    .Link(v => v.Location, (v, c) => c.MoveTo(v.Location)));
        }

        private void GraphChanged()
        {
            foreach (var vertex in Graph.Vertices)
                _vertices.GetDestination(vertex);
        }
    }
}
