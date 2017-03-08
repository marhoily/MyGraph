using System.Windows;
using Caliburn.Micro;
using Npc;

namespace MyGraph
{
    public partial class GraphControl
    {
        private FrameworkElement _freeEdge;

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
            if (Graph?.NewEdgeSource != null)
            {
                CaptureMouse();
                var graphFreeEdge = new GraphFreeEdge(Graph.NewEdgeSource);
                MouseMove += (s, e) => graphFreeEdge.Mouse = e.GetPosition(this);
                MouseDown += (s, e) => LeaveNewEdgeMode();
                _freeEdge = FreeEdgeTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .BindModel(graphFreeEdge);
                _edges.Children.Add(_freeEdge);
            }
            else
            {
                LeaveNewEdgeMode();
            }
        }

        private void LeaveNewEdgeMode()
        {
            ReleaseMouseCapture();
            _edges.Children.Remove(_freeEdge);
            _freeEdge = null;
        }
    }

    public sealed class GraphFreeEdge : PropertyChangedBase
    {
        private Point _mouse;
        public IVertex Source { get; }

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

        public GraphFreeEdge(IVertex source)
        {
            Source = source;
        }
    }

}
