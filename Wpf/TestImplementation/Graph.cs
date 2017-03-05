using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Graph : PropertyChangedBase, IGraph
    {
        public ObservableCollection<IVertex> Vertices { get; }
        public ObservableCollection<IEdge> Edges { get; }

        public Graph(ObservableCollection<IVertex> vertices, ObservableCollection<IEdge> edges)
        {
            Vertices = vertices;
            Edges = edges;
        }

    }
}