using System.Collections.ObjectModel;
using System.Windows;

namespace MyGraph
{
    public interface IGraph
    {
        ObservableCollection<INode> Nodes { get; }
        VirtualNode VirtualNode { get; }
    }

    public class VirtualNode : INode
    {
        public VirtualNode(Point location)
        {
            Location = location;
        }

        public Point Location { get; set; }
    }

    class Graph : IGraph
    {
        public ObservableCollection<INode> Nodes { get; }
        public VirtualNode VirtualNode { get; }

        public Graph(ObservableCollection<INode> nodes, VirtualNode virtualNode)
        {
            Nodes = nodes;
            VirtualNode = virtualNode;
        }
    }
}