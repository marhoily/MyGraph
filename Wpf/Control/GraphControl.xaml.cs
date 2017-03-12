using System.Windows;
using System.Windows.Input;
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
                .SubscribeAndApply((o, n) => OnNewEdgeSourceChanged())
                .Dispose);

            PreviewMouseDown += (s, e) =>
                Graph?.SetLastClickLocation(ViewPort.FromLocalToLatLng(e.GetPosition(this)));

            this.Track(x => x.ViewPort).SubscribeAndApply((o, n) => _vertices.ViewPort = n);
        }

        private void OnNewEdgeSourceChanged()
        {
            if (Graph?.NewEdgeSource == null) return;
            var graphFreeEdge = new GraphFreeEdge(this, Graph);
            var freeEdge = FreeEdgeTemplate.LoadContent()
                .Cast<FrameworkElement>().BindModel(graphFreeEdge);
            MouseMove += graphFreeEdge.OnMouseMove;
            _edges.Children.Add(freeEdge);
            MouseButtonEventHandler clean = null;
            clean = (s, e) =>
            {
                Graph.NewEdgeSource = null;
                _edges.Children.Remove(freeEdge);
                MouseMove -= graphFreeEdge.OnMouseMove;
                MouseDown -= clean;
            };
            MouseDown += clean;
        }
    }
}
