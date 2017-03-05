using System;
using System.Collections.Generic;
using System.Windows;
using Npc;

namespace MyGraph
{
    public partial class GraphControl
    {
        private readonly List<IDisposable> _dragHelpers = new List<IDisposable>();

        public GraphControl()
        {
            InitializeComponent();
            SynchronizeVertices();
            MouseUp += (s, e) => _dragHelpers.ForEach(h => h.Dispose());
        }

        private void SynchronizeVertices()
        {
            SetSynchronizer<FrameworkElement> vertexSynchronizer = null;
            Loaded += (s, e) =>
            {
                vertexSynchronizer = this.TrackSet(ctrl => ctrl.Graph.Vertices)
                    .Select(CreateVertexControl)
                    .With(v => v.Track(x => x.Location), (c, location) => c.MoveTo(location))
                    .SynchronizeTo(_plot.Children);
            };
            Unloaded += (s, e) =>
            {
                vertexSynchronizer.Dispose();
                vertexSynchronizer.Source.Dispose();
            };
        }

        private FrameworkElement CreateVertexControl(IVertex vertex)
        {
            var control = VertexTemplate.LoadContent().Cast<FrameworkElement>();
            control.Tag = vertex;
            control.MouseDown += (s, e) => _dragHelpers
                .Add(new VertexDragHelper(this, control, e));
            return control;
        }
    }
}
