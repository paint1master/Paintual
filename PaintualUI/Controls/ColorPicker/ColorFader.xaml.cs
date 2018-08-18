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
using System.Drawing;
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

using Engine;

namespace PaintualUI.Controls.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorFader.xaml
    /// </summary>
    public partial class ColorFader : UserControl
    {
        private Engine.Surface.Canvas t_faderImage;
        private bool t_cursorIsMovable;
        private Engine.Point t_mousePoint;
        private Engine.Color.Models.HSV[] t_range;

        public ColorFader()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            t_cursorIsMovable = true;
            t_mousePoint = new Engine.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);

            this.InvalidateVisual();

            ConstraintMousePosition();
            Engine.Color.Cell c = GetColorFromPosition();

            OnColorChanged(new ColorChangedEventArgs(c));

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            t_mousePoint = new Engine.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);

            if (t_cursorIsMovable)
            {
                this.InvalidateVisual();

                ConstraintMousePosition();
                Engine.Color.Cell c = GetColorFromPosition();

                OnColorChanged(new ColorChangedEventArgs(c));
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            t_cursorIsMovable = false;
            base.OnMouseUp(e);

            ConstraintMousePosition();

            Engine.Color.Cell c = GetColorFromPosition();
            OnColorChanged(new ColorChangedEventArgs(c));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(ConvertFaderImage(), new Rect(5, 5, this.Width - 10, this.Height - 10));

            // if mouse goes outside of fader image, draw cursor either at top or bottom
            ConstraintMousePosition();

            DrawCursor(drawingContext);

            base.OnRender(drawingContext);
        }

        private int CalculateFaderIndex()
        {
            int result = t_mousePoint.Y - 5;

            if (result < 0)
            {
                result = 0;
            }

            if (result > this.Height - 11)
            {
                result = (int)this.Height - 11;
            }

            return result;
        }

        private void ConstraintMousePosition()
        {
            if (t_mousePoint.Y > (int)this.Height - 1)
            {
                t_mousePoint.Y = (int)this.Height - 1;
            }

            if (t_mousePoint.Y < 0)
            {
                t_mousePoint.Y = 0;
            }

            if (t_mousePoint.X > (int)this.Width - 1)
            {
                t_mousePoint.X = (int)this.Width - 1;
            }

            if (t_mousePoint.X < 0)
            {
                t_mousePoint.X = 0;
            }
        }

        private BitmapSource ConvertFaderImage()
        {
            if (t_faderImage == null)
            {
                // cannot create image after Initialize() because container control has no size yet !!??!!
                CreateFaderImage();
            }

            BitmapSource bmpSource = BitmapSource.Create(t_faderImage.Width, t_faderImage.Height, 96, 96, PixelFormats.Bgra32, null, t_faderImage.Array, t_faderImage.Stride);

            return bmpSource;
        }

        private void CreateFaderImage()
        {
            t_faderImage = new Engine.Surface.Canvas((int)this.Width - 10, (int)this.Height - 10); //, new Engine.Color.Cell(100, 100, 100, 255));

            t_range = Engine.Calc.Color.GenerateLinearGradient(new Engine.Color.Models.HSV(0, 1.0, 1.0), new Engine.Color.Models.HSV(1.0, 1.0, 1.0), t_faderImage.Height);

            int offset = 0;

            for (int y = 0; y < t_faderImage.Height; y++)
            {
                System.Drawing.Color c = t_range[y].ToRGB().ToArgb();
                Engine.Color.Cell cell = new Engine.Color.Cell(c);

                for (int x = 0; x < t_faderImage.Width; x++)
                {
                    cell.WriteBytes(t_faderImage.Array, offset);
                    offset += Engine.BytesPerPixel.BGRA;
                }
            }
        }

        private Engine.Color.Cell GetColorFromPosition()
        {
            int i = CalculateFaderIndex();
            Engine.Color.Cell c = new Engine.Color.Cell(t_faderImage.Array, t_faderImage.GetOffset(0, i));

            return c;
        }

        private void DrawCursor(DrawingContext g)
        {
            int adjustedY = CalculateFaderIndex(); //t_mousePoint.Y - 5;

            PathGeometry leftcur = new PathGeometry();
            PathFigure lpf = new PathFigure();
            lpf.StartPoint = new System.Windows.Point(0, adjustedY + 0);
            LineSegment lls1 = new LineSegment(new System.Windows.Point(5, adjustedY + 0), true);
            LineSegment lls2 = new LineSegment(new System.Windows.Point(8, adjustedY + 5), true);
            LineSegment lls3 = new LineSegment(new System.Windows.Point(5, adjustedY + 10), true);
            LineSegment lls4 = new LineSegment(new System.Windows.Point(0, adjustedY + 10), true);
            LineSegment lls5 = new LineSegment(new System.Windows.Point(0, adjustedY + 0), true);

            PathSegmentCollection lpsc = new PathSegmentCollection(5);
            lpsc.Add(lls1);
            lpsc.Add(lls2);
            lpsc.Add(lls3);
            lpsc.Add(lls4);
            lpsc.Add(lls5);

            lpf.Segments = lpsc;

            leftcur.Figures = new PathFigureCollection(1);
            leftcur.Figures.Add(lpf);

            PathGeometry rightcur = new PathGeometry();
            PathFigure rpf = new PathFigure();
            rpf.StartPoint = new System.Windows.Point(23, adjustedY + 0);
            LineSegment rls1 = new LineSegment(new System.Windows.Point(23, adjustedY + 10), true);
            LineSegment rls2 = new LineSegment(new System.Windows.Point(23, adjustedY + 10), true);
            LineSegment rls3 = new LineSegment(new System.Windows.Point(18, adjustedY + 10), true);
            LineSegment rls4 = new LineSegment(new System.Windows.Point(15, adjustedY + 5), true);
            LineSegment rls5 = new LineSegment(new System.Windows.Point(18, adjustedY + 0), true);

            PathSegmentCollection rpsc = new PathSegmentCollection(5);
            rpsc.Add(rls1);
            rpsc.Add(rls2);
            rpsc.Add(rls3);
            rpsc.Add(rls4);
            rpsc.Add(rls5);

            rpf.Segments = rpsc;

            rightcur.Figures = new PathFigureCollection(1);
            rightcur.Figures.Add(rpf);

            SolidColorBrush b = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 210, 210, 210));
            System.Windows.Media.Pen p = new System.Windows.Media.Pen(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 100, 100)), 1d);

            g.DrawGeometry(b, p, leftcur);
            g.DrawGeometry(b, p, rightcur);
        }

        public event ColorChangedEventHandler ColorChanged;

        public void OnColorChanged(PaintualUI.Controls.ColorPicker.ColorChangedEventArgs e)
        {
            ColorChanged?.Invoke(this, e);
        }
    }
}
