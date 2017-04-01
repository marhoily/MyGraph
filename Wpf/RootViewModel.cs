using System;
using System.Linq;
using Caliburn.Micro;
using GMap.NET;
using JetBrains.Annotations;

namespace MyGraph
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class RootViewModel : PropertyChangedBase, IShell
    {
        public MapViewModel Map { get; } = new MapViewModel();

        public void Create300() => Create(300);
        public void Create1000() => Create(1000);

        public RootViewModel()
        {
            Create(1);
        }

        public void ChangeZoom()
        {
            Map.Zoom = 8;
        }

        private void Create(int num)
        {
            var rnd = new Random();
            var vertices = Map.Graph.Vertices;
            for (var i = 0; i < num; i++)
                vertices.Add(new VertexVm(new PointLatLng(
                    rnd.NextDouble() * 3+52,
                    rnd.NextDouble() * 5+26)));
            //_gMapControl.Position = new PointLatLng(53.856, 27.49);

            foreach (var edge in vertices.Zip(vertices.Skip(1), (a, b) => new EdgeVm(a, b)))
                Map.Graph.Edges.Add(edge);
        }
    }
}