using System.ComponentModel;
using GMap.NET;

namespace MyGraph
{
    public interface IVertex : INotifyPropertyChanged
    {
        PointLatLng Location { get; set; }
    }
}