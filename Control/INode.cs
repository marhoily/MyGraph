using System.ComponentModel;
using System.Windows;

namespace MyGraph
{
    public interface INode : INotifyPropertyChanged
    {
        Point Location { get; }
    }
}