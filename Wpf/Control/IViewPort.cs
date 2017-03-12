using System;
using System.Windows;
using GMap.NET;

namespace MyGraph
{
    public interface IViewPort
    {
        PointLatLng FromLocalToLatLng(Point p);
        Point FromLatLngToLocal(PointLatLng point);
        event Action Changed;
    }
}