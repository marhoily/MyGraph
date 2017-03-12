using System;
using System.Windows;
using GMap.NET;
using GMap.NET.WindowsPresentation;

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
}