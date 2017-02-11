using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Graph : PropertyChangedBase, IGraph
    {
        public ObservableCollection<INode> Nodes { get; }

        public Graph(ObservableCollection<INode> nodes)
        {
            Nodes = nodes;
        }
    }
}