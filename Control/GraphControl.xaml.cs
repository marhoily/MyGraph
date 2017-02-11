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
                vertice => VertexTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .PlaceAt(_plot, vertice.Location)
                    .SetDataContext(vertice)
                    .Bind(vertice));
        }

        private void GraphChanged()
        {
            foreach (var vertice in Graph.Vertices)
                _vertices.GetDestination(vertice);
        }
    }
}
