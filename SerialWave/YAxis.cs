using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SerialWave
{
    internal class YAxis : FrameworkElement
    {
        readonly VisualCollection visuals;
        DrawingVisual DV_YAxis = new();
        private Size oldSize;

        public double ratio = 0.05;
        public double division = 30;
        public double font_size = 12;

        public YAxis()
        {
            this.visuals = new VisualCollection(this);
            visuals.Add(DV_YAxis);
            this.SizeChanged += XAxis_SizeChanged;
        }

        protected override int VisualChildrenCount => visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        private void XAxis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size oldSize = e.NewSize;
            DrawYAxis(e.NewSize);
        }

        public void DrawYAxis(Size size)
        {
            DrawingContext context = DV_YAxis.RenderOpen();
            Pen pen = new Pen()
            {
                Brush = Brushes.Orange,
                Thickness = 1,
            };
            context.DrawLine(pen,
                new Point(size.Width - 1, 0),
                new Point(size.Width - 1, size.Height));
            for (double i = size.Height / 2; i >= 0; i = i - division)
            {
                context.DrawLine(pen,
                    new Point(size.Width, i),
                    new Point(size.Width - 10, i));
                FormattedText text = new(
                    (((size.Height / 2) - i) * ratio).ToString("0.000"),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.RightToLeft,
                    new Typeface(new FontFamily("SimSun"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal),
                    font_size,
                    Brushes.Orange,
                    96);
                context.DrawText(text, new Point(size.Width - 12, i - font_size / 2));
            }
            for (double i = size.Height / 2 + division; i <= size.Height; i = i + division)
            {
                context.DrawLine(pen,
                    new Point(size.Width, i),
                    new Point(size.Width - 10, i));
                FormattedText text = new(
                    ((i - (size.Height / 2)) * ratio).ToString("0.000") + "-",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.RightToLeft,
                    new Typeface(new FontFamily("SimSun"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal),
                    font_size,
                    Brushes.Orange,
                    96);
                context.DrawText(text, new Point(size.Width - 12, i - font_size / 2));
            }
            context.Close();
        }

        public void ReDraw()
        {
            DrawYAxis(oldSize);
        }
    }
}
