using System.Collections.ObjectModel;

namespace MyGraph
{
    public interface IGraph
    {
        ObservableCollection<INode> Nodes { get; }
    }

    class Graph : IGraph
    {
        public ObservableCollection<INode> Nodes { get; }

        public Graph(ObservableCollection<INode> nodes)
        {
            Nodes = nodes;
        }
    }
}