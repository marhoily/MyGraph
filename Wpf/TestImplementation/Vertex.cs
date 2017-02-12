using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Vertex : PropertyChangedBase, IVertex
    {
        public Point Location { get; }

        public Vertex(Point location)
        {
            Location = location;
        }
    }
}