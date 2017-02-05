using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public interface IGraph
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
    }

    class Graph : IGraph
    {
        public ObservableCollection<INode> Nodes { get; }
        public VirtualNode VirtualNode { get; set; }

        public Graph(ObservableCollection<INode> nodes, VirtualNode virtualNode)
        {
            Nodes = nodes;
            VirtualNode = virtualNode;
        }
    }
}