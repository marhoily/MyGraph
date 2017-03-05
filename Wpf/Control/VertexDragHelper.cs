using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MyGraph
{
    internal sealed class VertexDragHelper : IDisposable
    {
        private readonly FrameworkElement _map;
        private readonly Window _window;
        private readonly Point _start;
        private bool _started;
        private readonly IVertex _vertex;

        public VertexDragHelper(FrameworkElement map, FrameworkElement control, MouseButtonEventArgs e)
        {
            _map = map;
            _window = Window.GetWindow(control);
            _start = e.GetPosition(_map);
            Debug.Assert(_window != null);
            _window.MouseMove += OnMove;
            _vertex = (IVertex)control.DataContext;
        }

        private void OnMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(_map);
            _started = _started || Point.Subtract(_start, p).Length > 10;
            if (_started) _vertex.Location = p;
        }

        public void Dispose()
        {
            _window.MouseMove -= OnMove;
        }
    }
}