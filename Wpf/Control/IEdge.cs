using System.ComponentModel;

namespace MyGraph
{
    public interface IEdge : INotifyPropertyChanged
    {
        IVertex X { get;  }
        IVertex Y { get;  }
    }
}