using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Graph : PropertyChangedBase, IGraph
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

        public void StartEdge(Vertex source)
        {
            NewEdgeSource = source;
        }

        public void EndEdge(Vertex destination)
        {
            if (NewEdgeSource == null) return;
            Edges.Add(new Edge(NewEdgeSource, destination));
            NewEdgeSource = null;
        }

        public Graph(ObservableCollection<IVertex> vertices, ObservableCollection<IEdge> edges)
        {
            Vertices = vertices;
            Edges = edges;
        }

    }
}