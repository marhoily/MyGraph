using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public interface IGraph : INotifyPropertyChanged
    {
        ObservableCollection<IEdge> Edges { get; }
        IEdge CreateVirtualEdge(INode a, INode b);

        ObservableCollection<INode> Nodes { get; }
        INode VirtualNode { get; set; }
        INode CreateVirtualNode(Point location);
    }

    public interface IEdge : INotifyPropertyChanged
    {
        INode A { get; }
        INode B { get; }
    }

    class Edge : PropertyChangedBase, IEdge
    {
        public INode A { get; }
        public INode B { get; }

        public Edge(INode a, INode b)
        {
            A = a;
            B = b;
        }
    }

    public class Graph : PropertyChangedBase, IGraph
    {
        private Node _virtualNode;
        public ObservableCollection<INode> Nodes { get; }

        INode IGraph.VirtualNode { get { return VirtualNode; } set { VirtualNode = (Node)value; } }
        public Node VirtualNode
        {
            get { return _virtualNode; }
            set
            {
                if (Equals(value, _virtualNode)) return;
                _virtualNode = value;
                NotifyOfPropertyChange(nameof(VirtualNode));
                NotifyOfPropertyChange(nameof(VirtualNodeLocationEditor));
            }
        }

        public PointEditorViewModel VirtualNodeLocationEditor =>
            VirtualNode == null ? null : new PointEditorViewModel(VirtualNode, nameof(Node.Location));

        public void AddNodeFailed()
        {
            Debug.Assert(VirtualNode != null);
            VirtualNode = null;
            NotifyOfPropertyChange(nameof(VirtualNode));

        }
        public void AddNode()
        {
            Debug.Assert(VirtualNode != null);
            var newNode = new Node(VirtualNode.Location);
            VirtualNode = null;
            NotifyOfPropertyChange(nameof(VirtualNode));
            Nodes.Add(newNode);
        }
        public Graph(ObservableCollection<INode> nodes, Node virtualNode, ObservableCollection<IEdge> edges)
        {
            Nodes = nodes;
            VirtualNode = virtualNode;
            Edges = edges;
        }

        public ObservableCollection<IEdge> Edges { get; }


        public IEdge CreateVirtualEdge(INode a, INode b)
        {
            return new Edge(a, b);
        }

        public INode CreateVirtualNode(Point location)
        {
            return new Node(location);
        }
    }
}