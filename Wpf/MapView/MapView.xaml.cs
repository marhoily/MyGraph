using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
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
        }

        private async void MapView_OnLoaded(object sender, RoutedEventArgs e)
        {
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            _gMapControl.Zoom = 9;
            Debug.WriteLine(_gMapControl.MapProvider.Projection?.ToString() ?? "<null>");
        }
    }
}
