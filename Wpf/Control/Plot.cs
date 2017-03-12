using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            return (PointLatLng) element.GetValue(LocationProperty);
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
            }
            var newValue = (IViewPort)dependencyPropertyChangedEventArgs.NewValue;
            if (newValue != null)
            {
                newValue.Changed += plot.InvalidateArrange;
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
                var localPosition = ViewPort.FromLatLngToLocal(GetLocation(internalChild));
                var center = localPosition - (Vector)desiredSize/2.0;
                internalChild.Arrange(new Rect(center, desiredSize));
            }
            return arrangeSize;
        }
    }
}