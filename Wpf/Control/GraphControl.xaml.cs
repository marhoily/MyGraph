using System.Windows;
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
                .Select(CreateVertexControl)
                .SynchronizeTo(_plot.Children)
                .DisposeWithSource);

            this.WhenLoaded(() => this.TrackSet(ctrl => ctrl.Graph.Edges)
                .Select(CreateEdgeControl)
                .SynchronizeTo(_edges.Children)
                .DisposeWithSource);
        }


        private FrameworkElement CreateVertexControl(IVertex vertex)
        {
            var control = VertexTemplate.LoadContent().Cast<FrameworkElement>();
            Bind.SetModel(control, vertex);
            control.Tag = vertex;
            control.MouseDown += (s, e) =>
            {
                var h = new VertexDragHelper(this, control, e);
                MouseUp += (s1, e1) => h.Dispose();
            };
            return control;
        }
        private FrameworkElement CreateEdgeControl(IEdge edge)
        {
            var control = EdgeTemplate.LoadContent().Cast<FrameworkElement>();
            Bind.SetModel(control, edge);
            control.Tag = edge;
            return control;
        }
    }
}
