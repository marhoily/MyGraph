using Caliburn.Micro;
using GMap.NET;

namespace MyGraph
{
    public sealed class VertexVm : PropertyChangedBase, IVertex
    {
        private PointLatLng _location;

        public PointLatLng Location
        {
            get { return _location; }
            set
            {
                if (value.Equals(_location)) return;
                _location = value;
                NotifyOfPropertyChange();
            }
        }
        public VertexVm(PointLatLng location)
        {
            Location = location;
        }
    }
}