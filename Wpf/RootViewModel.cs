using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace MyGraph
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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
            var vertices = Graph.Vertices;
            for (var i = 0; i < num; i++)
                vertices.Add(new VertexVm(new Point(
                    rnd.NextDouble() * 500+5,
                    rnd.NextDouble() * 500+5)));

            foreach (var edge in vertices.Zip(vertices.Skip(1), (a, b) => new EdgeVm(a, b)))
                Graph.Edges.Add(edge);
        }
    }
}