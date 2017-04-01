using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using GMap.NET;

namespace MyGraph
{
    public sealed class Plot : Panel
    {
        public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached(
            "Location", typeof(PointLatLng), typeof(Plot), new PropertyMetadata(default(PointLatLng)));

        public static void SetLocation(DependencyObject element, PointLatLng value)
        {
            element.SetValue(LocationProperty, value);
        }

        public static PointLatLng GetLocation(DependencyObject element)
        {
            return (PointLatLng)element.GetValue(LocationProperty);
        }

        public static readonly DependencyProperty ViewPortProperty = DependencyProperty.Register(
            "ViewPort", typeof(IViewPort), typeof(Plot), new PropertyMetadata(default(IViewPort), ViewPortPropertyChangedCallback));

        private static void ViewPortPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var plot = (Plot)dependencyObject;
            var oldValue = (IViewPort)dependencyPropertyChangedEventArgs.OldValue;
            if (oldValue != null)
            {
                oldValue.Changed -= plot.InvalidateArrange;
                oldValue.Changed -= plot.UpdateLinePositions;
            }
            var newValue = (IViewPort)dependencyPropertyChangedEventArgs.NewValue;
            if (newValue != null)
            {
                newValue.Changed += plot.InvalidateArrange;
                newValue.Changed += plot.UpdateLinePositions;
            }
        }

        public IViewPort ViewPort
        {
            get { return (IViewPort)GetValue(ViewPortProperty); }
            set { SetValue(ViewPortProperty, value); }
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement internalChild in InternalChildren)
                internalChild?.Measure(infinity);
            return new Size();
        }
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (var internalChild in InternalChildren.OfType<UIElement>())
            {
                var desiredSize = internalChild.DesiredSize;
                if (internalChild is Panel)
                {
                    internalChild.Arrange(new Rect(new Point(0, 0), desiredSize));
                }
                else
                {
                    var localPosition = ViewPort.FromLatLngToLocal(GetLocation(internalChild));
                    var center = localPosition - (Vector)desiredSize / 2.0;
                    internalChild.Arrange(new Rect(center, desiredSize));
                }
            }
            return arrangeSize;
        }

        public static readonly DependencyProperty LineStartProperty = DependencyProperty.RegisterAttached(
            "LineStart", typeof(PointLatLng), typeof(Plot), new PropertyMetadata(default(PointLatLng), LineStartChanged));

        public static void SetLineStart(DependencyObject element, PointLatLng value)
        {
            element.SetValue(LineStartProperty, value);
        }

        public static PointLatLng GetLineStart(DependencyObject element)
        {
            return (PointLatLng)element.GetValue(LineStartProperty);
        }

        public static readonly DependencyProperty LineEndProperty = DependencyProperty.RegisterAttached(
            "LineEnd", typeof(PointLatLng), typeof(Plot), new PropertyMetadata(default(PointLatLng), LineEndChanged));

        public static void SetLineEnd(DependencyObject element, PointLatLng value)
        {
            element.SetValue(LineEndProperty, value);
        }

        public static PointLatLng GetLineEnd(DependencyObject element)
        {
            return (PointLatLng)element.GetValue(LineEndProperty);
        }

        private static void LineStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (Line)d;
            var plot = line.Parent<Plot>();
            if (plot == null) return;
            var point = plot.ViewPort.FromLatLngToLocal((PointLatLng)e.NewValue);
            line.X1 = point.X;
            line.Y1 = point.Y;
        }
        private static void LineEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (Line)d;
            var plot = line.Parent<Plot>();
            if (plot == null) return;
            var point = plot.ViewPort.FromLatLngToLocal((PointLatLng)e.NewValue);
            line.X2 = point.X;
            line.Y2 = point.Y;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) => UpdateLinePositions();

        private void UpdateLinePositions()
        {
            foreach (var line in this.Descendants().OfType<Line>())
            {
                var start = ViewPort.FromLatLngToLocal(GetLineStart(line));
                line.X1 = start.X;
                line.Y1 = start.Y;
                var end = ViewPort.FromLatLngToLocal(GetLineEnd(line));
                line.X2 = end.X;
                line.Y2 = end.Y;
            }
        }
    }
}