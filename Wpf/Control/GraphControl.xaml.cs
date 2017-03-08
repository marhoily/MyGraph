using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Npc;

namespace MyGraph
{
    public partial class GraphControl
    {
        public GraphControl()
        {
            InitializeComponent();

            this.WhenLoaded(() => this.TrackSet(ctrl => ctrl.Graph.Vertices)
                .Select(vertex => VertexTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .BindModel(vertex)
                    .SubscribeForDragging(this))
                .SynchronizeTo(_vertices.Children)
                .DisposeWithSource);

            this.WhenLoaded(() => this.TrackSet(ctrl => ctrl.Graph.Edges)
                .Select(edge => EdgeTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .BindModel(edge))
                .SynchronizeTo(_edges.Children)
                .DisposeWithSource);

            this.WhenLoaded(() => this.Track(ctrl => ctrl.Graph.NewEdgeSource)
                .SubscribeAndApply((o,n) => OnNewEdgeSource())
                .Dispose);

            PreviewMouseDown += (s, e) => LastClickLocation = e.GetPosition(this);
        }

        private void OnNewEdgeSource()
        {
            if (Graph?.NewEdgeSource == null) return;
            var freeEdge = FreeEdgeTemplate.LoadContent().Cast<FrameworkElement>();
            var graphFreeEdge = new GraphFreeEdge(this, Graph, _edges, freeEdge);
            freeEdge.BindModel(graphFreeEdge);
            MouseMove += graphFreeEdge.OnMouseMove;
            MouseDown += graphFreeEdge.OnMouseDown;
        }
    }

    public sealed class GraphFreeEdge : PropertyChangedBase
    {
        private readonly GraphControl _graphControl;
        private readonly IGraph _graph;
        private readonly Canvas _edges;
        private readonly FrameworkElement _freeEdge;
        private Point _mouse;
        public IVertex Source => _graph.NewEdgeSource;

        public Point Mouse
        {
            get { return _mouse; }
            set
            {
                if (value.Equals(_mouse)) return;
                _mouse = value;
                NotifyOfPropertyChange();
            }
        }

        public GraphFreeEdge(GraphControl graphControl, IGraph graph, Canvas edges, FrameworkElement freeEdge)
        {
            _graphControl = graphControl;
            _graph = graph;
            _edges = edges;
            _freeEdge = freeEdge;
            _edges.Children.Add(_freeEdge);
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            Mouse = e.GetPosition(_graphControl);
        }

        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _graph.NewEdgeSource = null;
            _edges.Children.Remove(_freeEdge);
            _graphControl.MouseMove -= OnMouseMove;
            _graphControl.MouseDown -= OnMouseDown;
        }
    }
}
