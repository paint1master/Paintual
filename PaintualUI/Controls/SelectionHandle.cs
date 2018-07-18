/**********************************************************

MIT License

Copyright (c) 2018 Michel Belisle

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

**********************************************************/

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintualUI.Controls
{
    public class SelectionHandle
    {
        /// <summary>
        /// The number of pixels to offset (for X and Y) the position of the handle in relation to the corner above which they are placed.
        /// </summary>
        public const int Offset = 4;

        private System.Windows.Shapes.Rectangle t_handle;

        /// <summary>
        /// Remembers the position of t_handle at 100% magnifcation; makes all calculations simpler.
        /// </summary>
        private System.Windows.Point t_fullSizeLocation;

        private int t_order;

        public SelectionHandle(double initialLocationX, double initialLocationY)
        {
            t_handle = new System.Windows.Shapes.Rectangle();
            t_handle.Fill = System.Windows.Media.Brushes.White;
            t_handle.Stroke = System.Windows.Media.Brushes.Black;
            t_handle.StrokeThickness = 1d;
            t_handle.Width = 9;
            t_handle.Height = 9;
            t_handle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            t_handle.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            SetPosition(initialLocationX, initialLocationY);

            t_handle.MouseDown += OnMouseDown;
            t_handle.MouseMove += OnMouseMove;
            t_handle.MouseUp += OnMouseUp;
            t_handle.MouseLeave += OnMouseLeave;
        }

        public void SetPosition(double x, double y)
        {
            t_fullSizeLocation = new Point(x, y);

            Thickness t = t_handle.Margin;
            t.Left = x;
            t.Top = y;
            t_handle.Margin = t;
        }

        public void AdjustPosition(double zoomFactor)
        {
            Thickness t = t_handle.Margin;

            t.Left = t_fullSizeLocation.X * zoomFactor;
            t.Top = t_fullSizeLocation.Y * zoomFactor;
            t_handle.Margin = t;
        }


        public System.Windows.Point Position
        {
            get { return t_fullSizeLocation; }
        }

        /// <summary>
        /// A visually renderable element that can be displayed by WPF
        /// </summary>
        public System.Windows.Shapes.Rectangle Shape
        {
            get { return t_handle; }
        }

        public int Order
        {
            get { return t_order; }
            set { t_order = value; }
        }

        #region Events

        public event System.Windows.Input.MouseButtonEventHandler MouseDown;
        public event System.Windows.Input.MouseButtonEventHandler MouseUp;
        public event System.Windows.Input.MouseEventHandler MouseMove;
        public event System.Windows.Input.MouseEventHandler MouseLeave;

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MouseDown != null)
            {
                MouseDown(this, e);
            }
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MouseMove != null)
            {
                MouseMove(this, e);
            }
        }

        private void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MouseUp != null)
            {
                MouseUp(this, e);
            }
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MouseLeave != null)
            {
                MouseLeave(this, e);
            }
        }

        #endregion


    }
}
