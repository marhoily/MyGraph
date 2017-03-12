using System.Windows;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Action = System.Action;

namespace MyGraph
{

    public class ViewPort : IViewPort
    {
        private readonly GMapControl _control;

        public ViewPort(GMapControl control)
        {
            _control = control;
            _control.OnMapZoomChanged += () => Changed?.Invoke();
            _control.OnPositionChanged += e => Changed?.Invoke();
        }

        public PointLatLng FromLocalToLatLng(Point p) => 
            _control.FromLocalToLatLng((int)p.X, (int)p.Y);

        public Point FromLatLngToLocal(PointLatLng point)
        {
            var g = _control.FromLatLngToLocal(point);
            return new Point(g.X, g.Y);
        }
        public event Action Changed;
    }
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
        }

    }
}
