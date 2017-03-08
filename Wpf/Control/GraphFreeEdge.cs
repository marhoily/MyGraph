using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class GraphFreeEdge : PropertyChangedBase
    {
        private readonly GraphControl _graphControl;
        private readonly IGraph _graph;

        public IVertex Source => _graph.NewEdgeSource;
        public Point Mouse { get; private set; }

        public GraphFreeEdge(GraphControl graphControl, IGraph graph)
        {
            _graphControl = graphControl;
            _graph = graph;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            Mouse = e.GetPosition(_graphControl);
            NotifyOfPropertyChange(nameof(Mouse));
        }
    }
}