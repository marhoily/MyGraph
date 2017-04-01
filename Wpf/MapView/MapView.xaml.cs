using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;

namespace MyGraph
{
    public partial class MapView 
    {
        public MapView()
        {
            InitializeComponent();
            _gMapControl.MapProvider = GMapProviders.OpenStreetMap;
            _gMapControl.Position = new PointLatLng(53.856, 27.49);
            _gMapControl.MinZoom = 1;
            _gMapControl.MaxZoom = 20;
            _gMapControl.Zoom = 7;
            _graphView._control.ViewPort = new ViewPort(_gMapControl);
            _gMapControl.DragButton = MouseButton.Left;
        }

    }
}
