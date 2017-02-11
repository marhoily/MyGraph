using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Node : PropertyChangedBase, INode
    {
        public Point Location { get; }

        public Node(Point location)
        {
            Location = location;
        }
    }
}