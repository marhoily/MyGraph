using Caliburn.Micro;

namespace MyGraph
{
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