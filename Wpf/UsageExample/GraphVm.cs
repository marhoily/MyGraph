using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class GraphVm : PropertyChangedBase, IGraph
    {
        public ObservableCollection<IVertex> Vertices { get; }
        public ObservableCollection<IEdge> Edges { get; }

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

        public GraphVm(ObservableCollection<IVertex> vertices, ObservableCollection<IEdge> edges)
        {
            Vertices = vertices;
            Edges = edges;
        }

    }
}