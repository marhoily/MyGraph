using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class RootViewModel : PropertyChangedBase, IShell
    {
        public Graph Graph { get; }

        public void Create300()
        {
            var rnd = new Random();
            for (var i = 0; i < 300; i++)
                Graph.Vertices.Add(new Vertex(new Point(
                    rnd.NextDouble() * 500,
                    rnd.NextDouble() * 500)));

            var edges = Graph.Vertices.Zip(Graph.Vertices.Skip(1), (a, b) => new Edge(a, b));
            foreach (var edge in edges)
                Graph.Edges.Add(edge);
        }
        public void Create1000()
        {
            var rnd = new Random();
            for (var i = 0; i < 1000; i++)
                Graph.Vertices.Add(new Vertex(new Point(
                    rnd.NextDouble() * 500,
                    rnd.NextDouble() * 500)));

            var edges = Graph.Vertices.Zip(Graph.Vertices.Skip(1), (a, b) => new Edge(a, b));
            foreach (var edge in edges)
                Graph.Edges.Add(edge);
        }
        public RootViewModel()
        {
            Graph = new Graph(
                new ObservableCollection<IVertex>(), 
                new ObservableCollection<IEdge>());
        }
    }
}