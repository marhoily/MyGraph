using System.ComponentModel;
using System.Windows;

namespace MyGraph
{
    public interface IVertex : INotifyPropertyChanged
    {
        Point Location { get; set; }
    }
    public interface IEdge : INotifyPropertyChanged
    {
        IVertex X { get;  }
        IVertex Y { get;  }
    }
}