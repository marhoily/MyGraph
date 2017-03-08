using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Graph : PropertyChangedBase, IGraph
    {
        public ObservableCollection<IVertex> Vertices { get; }
        public ObservableCollection<IEdge> Edges { get; }

        public IVertex NewEdgeSource { get; private set; }
        public void StartEdge(Vertex source)
        {
            NewEdgeSource = source;
            NotifyOfPropertyChange(nameof(NewEdgeSource));
        }

        public void EndEdge(Vertex destination)
        {
            if (NewEdgeSource == null) return;
            Edges.Add(new Edge(NewEdgeSource, destination));
            NewEdgeSource = null;
            NotifyOfPropertyChange(nameof(NewEdgeSource));
        }

        public Graph(ObservableCollection<IVertex> vertices, ObservableCollection<IEdge> edges)
        {
            Vertices = vertices;
            Edges = edges;
        }

    }
}