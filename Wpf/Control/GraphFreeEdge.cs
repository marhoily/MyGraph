using System.Windows.Input;
using Caliburn.Micro;
using GMap.NET;

namespace MyGraph
{
    public sealed class GraphFreeEdge : PropertyChangedBase
    {
        private readonly GraphControl _graphControl;
        private readonly IGraph _graph;

        public IVertex Source => _graph.NewEdgeSource;
        public PointLatLng Mouse { get; private set; }

        public GraphFreeEdge(GraphControl graphControl, IGraph graph)
        {
            _graphControl = graphControl;
            _graph = graph;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            Mouse = _graphControl.ViewPort.FromLocalToLatLng(e.GetPosition(_graphControl));
            NotifyOfPropertyChange(nameof(Mouse));
        }
    }
}