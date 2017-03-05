using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static System.Windows.FrameworkPropertyMetadataOptions;

namespace MyGraph
{
    public sealed class Plot : Panel
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.RegisterAttached(
            "X", typeof(double), typeof(Plot), new FrameworkPropertyMetadata(0.0, AffectsParentArrange));

        public static void SetX(DependencyObject element, double value)
        {
            element.SetValue(XProperty, value);
        }

        public static double GetX(DependencyObject element)
        {
            return (double) element.GetValue(XProperty);
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.RegisterAttached(
            "Y", typeof(double), typeof(Plot), new FrameworkPropertyMetadata(0.0, AffectsParentArrange));

        public static void SetY(DependencyObject element, double value)
        {
            element.SetValue(YProperty, value);
        }

        public static double GetY(DependencyObject element)
        {
            return (double) element.GetValue(YProperty);
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
                var x = GetX(internalChild) - desiredSize.Width /2;
                var y = GetY(internalChild) - desiredSize.Height /2;
                internalChild.Arrange(new Rect(new Point(x, y), desiredSize));
            }
            return arrangeSize;
        }
    }
}