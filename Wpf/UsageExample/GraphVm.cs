using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace MyGraph
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class GraphVm : PropertyChangedBase, IGraph
    {
        public ObservableCollection<IVertex> Vertices { get; } = new ObservableCollection<IVertex>();
        public ObservableCollection<IEdge> Edges { get; } = new ObservableCollection<IEdge>();

        private Point _lastClickLocation;
        public void SetLastClickLocation(Point p) => _lastClickLocation = p;
        public void AddVertex() => Vertices.Add(new VertexVm(_lastClickLocation));

        private IVertex _newEdgeSource;
        public IVertex NewEdgeSource
        {
            get { return _newEdgeSource; }
            set
            {
                if (Equals(value, _newEdgeSource)) return;
                _newEdgeSource = value;
                NotifyOfPropertyChange();
            }
        }
        public void StartEdge(VertexVm source)
        {
            NewEdgeSource = source;
        }
        public void EndEdge(VertexVm destination)
        {
            if (NewEdgeSource == null) return;
            Edges.Add(new EdgeVm(NewEdgeSource, destination));
            NewEdgeSource = null;
        }

        public void DeleteVertex(VertexVm v)
        {
            foreach (var edge in Edges.ToList())
                if (edge.End1 == v || edge.End2 == v)
                    Edges.Remove(edge);
            Vertices.Remove(v);
        }
        public void DeleteEdge(EdgeVm e) => Edges.Remove(e);
    }
}