using System.Collections.ObjectModel;

namespace MyGraph
{
    public interface IGraph
    {
        ObservableCollection<INode> Nodes { get; }
    }
}