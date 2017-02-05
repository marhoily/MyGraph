using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public class RootViewModel : PropertyChangedBase, IShell
    {
        public IGraph Graph { get; }

        public RootViewModel()
        {
            Graph = new Graph(new ObservableCollection<INode>
            {
                new Node(new Point(100, 100)),
                new Node(new Point(200, 200)),
            },
            new VirtualNode(new Point(300, 300)));
        }
    }
}