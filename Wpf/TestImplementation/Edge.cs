using Caliburn.Micro;

namespace MyGraph
{
    public sealed class Edge : PropertyChangedBase, IEdge
    {
        public IVertex End1 { get; }
        public IVertex End2 { get; }

        public Edge(IVertex x, IVertex y)
        {
            End1 = x;
            End2 = y;
        }
    }
}