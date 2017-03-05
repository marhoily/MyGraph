using System.Windows;
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
                .SynchronizeTo(_plot.Children)
                .DisposeWithSource);

            this.WhenLoaded(() => this.TrackSet(ctrl => ctrl.Graph.Edges)
                .Select(edge => EdgeTemplate.LoadContent()
                    .Cast<FrameworkElement>()
                    .BindModel(edge))
                .SynchronizeTo(_edges.Children)
                .DisposeWithSource);
        }
    }
}
