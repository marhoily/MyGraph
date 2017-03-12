using Caliburn.Micro;
using JetBrains.Annotations;

namespace MyGraph
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class MapViewModel : PropertyChangedBase, IShell
    {
        public GraphVm Graph { get; } = new GraphVm();

        private double _zoom;
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value.Equals(_zoom)) return;
                _zoom = value;
                NotifyOfPropertyChange();
            }
        }
    }
}