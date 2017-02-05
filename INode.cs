using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public interface INode : INotifyPropertyChanged
    {
        Point Location { get; }
    }

    class Node : PropertyChangedBase, INode
    {
        private Point _location;
        /*
        public void StartEdge()
        {
            IsEdgeStart = true;
        }

        public bool IsEdgeStart
        {
            get { return _isEdgeStart; }
            set
            {
                if (value == _isEdgeStart) return;
                _isEdgeStart = value;
                NotifyOfPropertyChange(nameof(IsEdgeStart));
            }
        }
        private bool _isEdgeStart;
        */

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

        public Node(Point location)
        {
            Location = location;
            _id = ++_counter;
        }

        private static int _counter;
        private readonly int _id;
        public override string ToString() => $"Node({_id})";
    }
}