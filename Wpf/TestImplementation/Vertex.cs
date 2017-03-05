using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Vertex : PropertyChangedBase, IVertex
    {
        private Point _location;

        public Point Location
        {
            get { return _location; }
            set
            {
                if (value.Equals(_location)) return;
                _location = value;
                NotifyOfPropertyChange();
            }
        }

        public Vertex(Point location)
        {
            Location = location;
        }
    }
    public sealed class Edge : PropertyChangedBase, IEdge
    {
        public IVertex X { get; }
        public IVertex Y { get; }

        public Edge(IVertex x, IVertex y)
        {
            X = x;
            Y = y;
        }
    }
}