using System.Windows;

namespace MyGraph
{
    public interface INode
    {
        Point Location { get; }
    }

    class Node : INode
    {
        public Point Location { get; }

        public Node(Point location)
        {
            Location = location;
        }
    }
}