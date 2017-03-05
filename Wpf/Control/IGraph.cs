using System.Collections.ObjectModel;

namespace MyGraph
{
    public interface IGraph
    {
        ObservableCollection<IVertex> Vertices { get; }
        ObservableCollection<IEdge> Edges { get; }
    }
}