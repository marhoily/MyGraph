using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;

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

    public class PointEditorViewModel : PropertyChangedBase
    {
        private readonly Func<Point> _getter;
        private readonly Action<Point> _setter;

        /// <summary>Used by Visual Studio at design time</summary>
        public PointEditorViewModel()
        {
            _getter = () => new Point(20.3475638, -8);
            _setter = _ => { };
        }

        public PointEditorViewModel([NotNull] INotifyPropertyChanged vm, [NotNull] string propertyName)
        {
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == propertyName) GetValue(); };
            var propertyInfo = vm.GetType().GetProperty(propertyName);
            _getter = () => (Point)propertyInfo.GetValue(vm);
            _setter = x => propertyInfo.SetValue(vm, x);
        }

        private void GetValue()
        {
            NotifyOfPropertyChange(nameof(X));
            NotifyOfPropertyChange(nameof(Y));
        }

        public double X
        {
            get { return _getter().X; }
            set { _setter(new Point(value, Y)); }
        }

        public double Y
        {
            get { return _getter().Y; }
            set { _setter(new Point(Y, value)); }
        }
    }
}