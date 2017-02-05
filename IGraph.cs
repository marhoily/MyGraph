using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public interface IGraph : INotifyPropertyChanged
    {
        ObservableCollection<INode> Nodes { get; }
        VirtualNode VirtualNode { get; set; }
    }

    public class VirtualNode : PropertyChangedBase, INode
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
    }

    class Graph : PropertyChangedBase, IGraph
    {
        private VirtualNode _virtualNode;
        public ObservableCollection<INode> Nodes { get; }

        public VirtualNode VirtualNode
        {
            get { return _virtualNode; }
            set
            {
                if (Equals(value, _virtualNode)) return;
                _virtualNode = value;
                NotifyOfPropertyChange(nameof(VirtualNode));
            }
        }

        public void AddNode()
        {
            Debug.Assert(VirtualNode != null);
            var newNode = new Node(VirtualNode.Location);
            VirtualNode = null;
            NotifyOfPropertyChange(nameof(VirtualNode));
            Nodes.Add(newNode);
        }
        public Graph(ObservableCollection<INode> nodes, VirtualNode virtualNode)
        {
            Nodes = nodes;
            VirtualNode = virtualNode;
        }
    }
}