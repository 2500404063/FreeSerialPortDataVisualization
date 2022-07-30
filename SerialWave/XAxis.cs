using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SerialWave
{
    internal class XAxis : FrameworkElement
    {
        readonly VisualCollection visuals;
        DrawingVisual DV_XAxis = new();
        public XAxis()
        {
            this.visuals = new VisualCollection(this);
            visuals.Add(DV_XAxis);
            this.SizeChanged += XAxis_SizeChanged;
        }

        protected override int VisualChildrenCount => visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        private void XAxis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawXAxis(e.NewSize);
        }

        public void DrawXAxis(Size size)
        {
            DrawingContext context = DV_XAxis.RenderOpen();
            Pen pen = new Pen()
            {
                Brush = Brushes.Orange,
                Thickness = 1,
            };
            context.DrawLine(pen,
                new Point(0, 1),
                new Point(size.Width, 1));
            context.Close();
        }
    }
}
