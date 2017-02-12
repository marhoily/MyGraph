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
            Graph = new Graph(new ObservableCollection<IVertex>
            {
                new Vertex(new Point(100, 100))
            });
        }
    }
}