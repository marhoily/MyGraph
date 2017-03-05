using System.Windows;
using Npc;

namespace MyGraph
{
    public partial class GraphControl
    {
        private SetSynchronizer<FrameworkElement> _plotSynchronizer;

        public GraphControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                _plotSynchronizer = this.TrackSet(ctrl => ctrl.Graph.Vertices)
                    .Select(vertex => VertexTemplate.LoadContent().Cast<FrameworkElement>())
                    .With((v, c) => c.MoveTo(v.Location))
                    .SynchronizeTo(_plot.Children);
            };
            Unloaded += (s, e) =>
            {
                _plotSynchronizer.Dispose();
                _plotSynchronizer.Source.Dispose();
            };
        }
    }
}
