using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialWave
{
    internal class DrawPanel : FrameworkElement
    {
        readonly VisualCollection visuals;
        private DrawingVisual DV_Default = new();
        private List<string> DV_Type = new();
        private List<string> DV_Label = new();
        private List<Brush> DV_Color = new();
        private List<List<object>> DV_Val = new();
        private Size oldSize;

        public double ratio = 0.05;
        public bool IsDrawZeroLine = true;
        public double Thickness = 1;

        public DrawPanel()
        {
            visuals = new VisualCollection(this);
            visuals.Add(DV_Default);
            this.SizeChanged += DrawPanel_SizeChanged;
        }

        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        private void DrawZeroLine(Size size)
        {
            DrawingContext context = DV_Default.RenderOpen();
            Pen pen = new()
            {
                Brush = Brushes.OrangeRed,
                Thickness = 1,
            };
            context.DrawLine(pen, new Point(0, size.Height / 2), new Point(size.Width, size.Height / 2));
            context.Close();
        }

        public void AddFunction(string label, string type, string color)
        {
            DrawingVisual DV_F = new DrawingVisual();
            visuals.Add(DV_F);
            DV_Label.Add(label);
            DV_Type.Add(type);
            switch (color)
            {
                case "red": DV_Color.Add(Brushes.Red); break;
                case "green": DV_Color.Add(Brushes.Green); break;
                case "blue": DV_Color.Add(Brushes.DeepSkyBlue); break;
                case "yellow": DV_Color.Add(Brushes.Yellow); break;
                case "orange": DV_Color.Add(Brushes.Orange); break;
                case "pink": DV_Color.Add(Brushes.HotPink); break;
                case "purple": DV_Color.Add(Brushes.Purple); break;
                case "black": DV_Color.Add(Brushes.Black); break;
                case "white": DV_Color.Add(Brushes.White); break;
                default: DV_Color.Add(Brushes.Red); break;
            }
            DV_Val.Add(new List<object>());
        }

        public void AddData(int id, object val)
        {
            DV_Val[id].Add(val);
        }

        private void DrawData(Size size)
        {
            for (int i = 0; i < DV_Label.Count; i++)
            {
                this.Width = DV_Val[i].Count;
                Pen pen = new()
                {
                    Brush = DV_Color[i],
                    Thickness = Thickness,
                };
                DrawingContext context = ((DrawingVisual)visuals[i + 1]).RenderOpen();
                Point point0 = new();
                Point point1 = new();
                bool canDraw = false;
                double X = 0;
                foreach (var val in DV_Val[i])
                {
                    switch (DV_Type[i])
                    {
                        case "char":
                            byte char_val = (byte)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - ((char_val >= 0x80 ? (int)char_val - 256 : (int)char_val) / ratio);
                            break;
                        case "uchar":
                            byte uchar_val = (byte)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (uchar_val / ratio);
                            break;
                        case "short":
                            short short_val = (short)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (short_val / ratio);
                            break;
                        case "ushort":
                            ushort ushort_val = (ushort)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (ushort_val / ratio);
                            break;
                        case "int":
                            int int_val = (int)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (int_val / ratio);
                            break;
                        case "uint":
                            uint uint_val = (uint)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (uint_val / ratio);
                            break;
                        case "float":
                            float float_val = (float)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (float_val / ratio);
                            break;
                        case "double":
                            double double_val = (double)val;
                            point1.X = X;
                            point1.Y = (size.Height / 2) - (double_val / ratio);
                            break;
                        default:
                            MessageBox.Show("Wrong Type");
                            break;
                    }
                    if (canDraw) context.DrawLine(pen, point0, point1);
                    else canDraw = true;
                    point0 = point1;
                    X++;
                }
                context.Close();
            }
        }

        public void DrawSin(Size size)
        {
            DrawingContext context = DV_Default.RenderOpen();
            Pen pen = new()
            {
                Brush = Brushes.Blue,
                Thickness = 2
            };

            Point last_point = new()
            {
                X = 0,
                Y = (int)(size.Height / 2),
            };
            for (float i = 0.0f; i < 18.0f; i = i + 0.05f)
            {
                Point point = new Point
                {
                    X = (int)(i / ratio),
                    Y = (int)(size.Height / 2) - (int)(Math.Sin(i) / ratio),
                };
                context.DrawLine(pen, (Point)last_point, point);
                last_point = point;
            }
            context.Close();
        }

        private void DrawPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            oldSize = e.NewSize;
            if (IsDrawZeroLine)
            {
                DrawZeroLine(e.NewSize);
            }
            DrawData(e.NewSize);
        }

        public void ReDraw()
        {
            DrawData(oldSize);
        }
    }
}
