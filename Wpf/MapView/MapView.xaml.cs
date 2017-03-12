using System.Windows.Input;

namespace MyGraph
{
    public partial class MapView 
    {
        public MapView()
        {
            InitializeComponent();
            _gMapControl.DragButton = MouseButton.Left;
        }
    }
}
