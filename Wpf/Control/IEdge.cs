using System.ComponentModel;

namespace MyGraph
{
    public interface IEdge : INotifyPropertyChanged
    {
        IVertex End1 { get;  }
        IVertex End2 { get;  }
    }
}