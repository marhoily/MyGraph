using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class RootViewModel : PropertyChangedBase, IShell
    {
        public GraphVm Graph { get; } = new GraphVm();

        public void Create300() => Create(300);
        public void Create1000() => Create(1000);

        public RootViewModel()
        {
            Create(10);
        }

        private void Create(int num)
        {
            var rnd = new Random();
            for (var i = 0; i < num; i++)
                Graph.Vertices.Add(new VertexVm(new Point(
                    rnd.NextDouble() * 500+5,
                    rnd.NextDouble() * 500+5)));

            var edges = Graph.Vertices.Zip(Graph.Vertices.Skip(1), (a, b) => new EdgeVm(a, b));
            foreach (var edge in edges)
                Graph.Edges.Add(edge);
        }
    }
}