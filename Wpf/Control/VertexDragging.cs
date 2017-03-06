using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MyGraph
{
    internal static class VertexDragging
    {
        /// <summary>Subscribes for the control's MouseDown event,
        ///  then looks for the <see cref="IVertex"/> in its 
        /// <see cref="FrameworkElement.DataContext"/>
        /// and sets its <see cref="IVertex.Location"/> property
        /// when deags
        ///  
        /// </summary>
        public static T SubscribeForDragging<T>(this T control, GraphControl map)
            where T : FrameworkElement
        {
            control.MouseDown += (s, e) =>
            {
                var window = Window.GetWindow(control);
                Debug.Assert(window != null);
                var helper = new DragState(map, control, e);
                MouseButtonEventHandler clean = null;
                clean = (sender, args) =>
                {
                    window.ReleaseMouseCapture();
                    window.MouseMove -= helper.OnMove;
                    window.MouseUp -= clean;
                };
                window.CaptureMouse();
                window.MouseMove += helper.OnMove;
                window.MouseUp += clean;
            };
            return control;
        }

        private sealed class DragState
        {
            private readonly FrameworkElement _map;
            private readonly Point _start;
            private readonly IVertex _vertex;
            private bool _started;

            public DragState(FrameworkElement map,
                FrameworkElement control, MouseEventArgs e)
            {
                _map = map;
                _start = e.GetPosition(_map);
                _vertex = (IVertex) control.DataContext;
            }

            public void OnMove(object sender, MouseEventArgs e)
            {
                var p = e.GetPosition(_map);
                _started = _started || Point.Subtract(_start, p).Length > 10;
                if (_started) _vertex.Location = p;
            }
        }
    }
}