using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyGraph
{
    public interface IGraph : INotifyPropertyChanged
    {
        ObservableCollection<IVertex> Vertices { get; }
        ObservableCollection<IEdge> Edges { get; }

        IVertex NewEdgeSource { get; set; }
    }
}