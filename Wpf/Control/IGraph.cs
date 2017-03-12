using System.Collections.ObjectModel;
using System.ComponentModel;
using GMap.NET;

namespace MyGraph
{
    public interface IGraph : INotifyPropertyChanged
    {
        ObservableCollection<IVertex> Vertices { get; }
        ObservableCollection<IEdge> Edges { get; }

        IVertex NewEdgeSource { get; set; }
        void SetLastClickLocation(PointLatLng p);
    }
}