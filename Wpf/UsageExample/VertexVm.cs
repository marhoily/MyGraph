using System.Windows;
using Caliburn.Micro;

namespace MyGraph
{
    public sealed class VertexVm : PropertyChangedBase, IVertex
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
        public VertexVm(Point location)
        {
            Location = location;
        }
    }
}