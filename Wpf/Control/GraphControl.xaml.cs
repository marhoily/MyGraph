using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
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
            SynchronizeEdges();
            MouseUp += (s, e) => _dragHelpers.ForEach(h => h.Dispose());
        }

        private void SynchronizeVertices()
        {
            SetSynchronizer<FrameworkElement> synchronizer = null;
            Loaded += (s, e) =>
            {
                synchronizer = this.TrackSet(ctrl => ctrl.Graph.Vertices)
                    .Select(CreateVertexControl)
                    .With(v => v.Track(x => x.Location), (c, location) => c.MoveTo(location))
                    .SynchronizeTo(_plot.Children);
            };
            Unloaded += (s, e) =>
            {
                synchronizer.Dispose();
                synchronizer.Source.Dispose();
            };
        }
        private void SynchronizeEdges()
        {
            SetSynchronizer<FrameworkElement> synchronizer = null;
            Loaded += (s, e) =>
            {
                synchronizer = this.TrackSet(ctrl => ctrl.Graph.Edges)
                    .Select(CreateEdgeControl)
                    .SynchronizeTo(_plot.Children);
            };
            Unloaded += (s, e) =>
            {
                synchronizer.Dispose();
                synchronizer.Source.Dispose();
            };
        }

        private FrameworkElement CreateVertexControl(IVertex vertex)
        {
            var control = VertexTemplate.LoadContent().Cast<FrameworkElement>();
            Bind.SetModel(control, vertex);
            control.Tag = vertex;
            control.MouseDown += (s, e) => _dragHelpers
                .Add(new VertexDragHelper(this, control, e));
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
