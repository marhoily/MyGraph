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

    public interface IEdge
    {
        INode A { get; }
        INode B { get; }
    }

    class Edge : IEdge
    {
        public INode A { get; }
        public INode B { get; }

        public Edge(INode a, INode b)
        {
            A = a;
            B = b;
        }
    }

   /* public class VirtualNode : PropertyChangedBase, INode
    {
        private Point _location;
        public PointEditorViewModel LocationEditor { get; }
        public VirtualNode(Point location)
        {
            Location = location;
            LocationEditor = new PointEditorViewModel(this, nameof(Location));
            _id = ++_counter;
        }
        public Point Location
        {
            get { return _location; }
            set
            {
                if (value.Equals(_location)) return;
                _location = value;
                NotifyOfPropertyChange(nameof(Location));
            }
        }

        private static int _counter;
        private readonly int _id;
        public override string ToString() => $"VirtualNode({_id})";
        public bool IsEdgeStart => false;
    }
    */
    class Graph : PropertyChangedBase, IGraph
    {
        private INode _virtualNode;
        public ObservableCollection<INode> Nodes { get; }

        public INode VirtualNode
        {
            get { return _virtualNode; }
            set
            {
                if (Equals(value, _virtualNode)) return;
                _virtualNode = value;
                NotifyOfPropertyChange(nameof(VirtualNode));
            }
        }

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
        public Graph(ObservableCollection<INode> nodes, INode virtualNode, ObservableCollection<IEdge> edges)
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