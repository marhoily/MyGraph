using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class RootViewModel : PropertyChangedBase, IShell
    {
        public Graph Graph { get; }

        public RootViewModel()
        {
            var a = new Vertex(new Point(100, 100));
            var b = new Vertex(new Point(300, 200));
            Graph = new Graph(new ObservableCollection<IVertex>{a,b},
                new ObservableCollection<IEdge> {new Edge(a, b)});
        }
    }
}