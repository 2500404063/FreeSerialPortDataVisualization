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
    /// <summary>
    /// Coordinate.xaml 的交互逻辑
    /// </summary>
    public partial class Coordinate : UserControl
    {
        private bool IsControlDown = false;
        private bool IsShiftDown = false;
        private bool IsRightControl = false;
        private bool canAutoScroll = true;

        public Coordinate()
        {
            InitializeComponent();
        }

        public void ChangeBackgroundColor(string color)
        {
            switch (color)
            {
                case "black": background.Background = Brushes.Black; break;
                case "white": background.Background = Brushes.White; break;
                case "gray": background.Background = Brushes.DimGray; break;
                default:
                    break;
            }
        }

        private void ScrollToRight()
        {
            if (canAutoScroll)
            {
                viewer.ScrollToRightEnd();
            }
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Data Scale
            if (IsControlDown)
            {
                if (e.Delta > 0)
                {
                    if (paper.ratio > 0.01)
                    {
                        paper.ratio -= 0.01;
                        yaxis.ratio -= 0.01;
                    }
                    else if (paper.ratio - 0.001 > 0)
                    {
                        paper.ratio -= 0.001;
                        yaxis.ratio -= 0.001;
                    }
                }
                else
                {
                    if (paper.ratio > 0.01)
                    {
                        paper.ratio += 0.01;
                        yaxis.ratio += 0.01;
                    }
                    else
                    {
                        paper.ratio += 0.001;
                        yaxis.ratio += 0.001;
                    }
                }
                paper.ReDraw();
                yaxis.ReDraw();
            }
            //Thickness
            else if (IsRightControl)
            {
                if (e.Delta > 0)
                    paper.Thickness += 0.5;
                else
                    paper.Thickness -= 0.5;
                paper.ReDraw();
            }
            //Division
            else if (IsShiftDown)
            {
                if (e.Delta > 0)
                    yaxis.division += 10;
                else
                    yaxis.division -= 10;
                yaxis.ReDraw();
                paper.ReDraw();
            }
            else
            {
                ScrollViewer scrollViewer = (ScrollViewer)sender;
                float div = 1;
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - (long)(e.Delta / div));
                if (scrollViewer.HorizontalOffset == scrollViewer.ScrollableWidth)
                {
                    canAutoScroll = true;
                }
                else
                {
                    canAutoScroll = false;
                }
            }
        }

        public void AddFunction(string label, string type, string corlor)
        {
            paper.AddFunction(label, type, corlor);
        }

        public void AddData(int id, object val)
        {
            paper.AddData(id, val);
        }

        public void ReDraw()
        {
            paper.ReDraw();
            ScrollToRight();
        }

        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                IsShiftDown = true;
            }
            else if (e.Key == Key.RightCtrl)
            {
                IsRightControl = true;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                IsControlDown = true;
            }
        }

        private void ScrollViewer_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                IsShiftDown = false;
            }
            else if (e.Key == Key.RightCtrl)
            {
                IsRightControl = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                IsControlDown = false;
            }
        }
    }
}
