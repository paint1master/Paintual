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
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Calc;

namespace PaintualUI.Controls
{
    internal enum SelectionMobility
    {
        CanMove,
        CannotMove
    }

    internal enum Resizing
    {
        IsResizing,
        IsNot
    }

    public class SelectionRectangle : System.Windows.UIElement, Engine.Utilities.Selection.ISelectionBase
    {
        private System.Windows.Shapes.Rectangle t_selection;

        /// <summary>
        /// Keeps track of the 100% magnification size and location of the selection rectangle.
        /// </summary>
        private System.Windows.Rect t_selectionTrueSize;
        private double t_zoomFactor = 1d;

        private System.Windows.Controls.Panel t_container;

        /// <summary>
        /// Remembers the position of the mouse while dragging/drawing the selection rectangle. Variable used only
        /// in the creation process.
        /// </summary>
        private System.Windows.Point t_createDragPosition;

        private System.Windows.Point t_movePoint;

        private PaintualUI.Controls.SelectionHandle[] t_handles;

        private SelectionMobility t_mobility = SelectionMobility.CannotMove;
        private Resizing t_resizing = Resizing.IsNot;

        private bool t_allowResize = false;

        private double[][,] t_matrices;
        private int t_currentMatrix;

        /// <summary>
        /// keeps track of mouse position when moving the handle when resizing selection. Helps calculate
        /// delta values.
        /// </summary>
        private System.Windows.Point t_handleMousePoint;
        private System.Windows.Point[] t_transformedRectanglePoints = new System.Windows.Point[4];
        private System.Windows.Point[] t_untransformedRectanglePoints = new System.Windows.Point[4];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container">The UIElement that will hold the selection rectangle.</param>
        /// <param name="location">The position of the upper left corner of the selection rectangle.</param>
        /// <param name="zoomFactor">provide the zoom factor value but it is not taken into calculations yet</param>
        public SelectionRectangle(System.Windows.Controls.Panel container, System.Windows.Point location, double zoomFactor)
        {
            t_handles = new SelectionHandle[4];

            t_container = container;
            t_selection = new System.Windows.Shapes.Rectangle();
            t_createDragPosition = new System.Windows.Point(location.X, location.Y);

            // TODO : add a second rectangle with white stroke so the overall selection can be seen on any back image color
            // or use a two-color stroke 
            t_selection.Stroke = System.Windows.Media.Brushes.Black;
            t_selection.StrokeThickness = 1d;
            t_selection.Fill = System.Windows.Media.Brushes.Transparent;
            t_selection.Width = 5;
            t_selection.Height = 5;

            t_selection.HorizontalAlignment = HorizontalAlignment.Left;
            t_selection.VerticalAlignment = VerticalAlignment.Top;

            SelectionLocationLeft = location.X;
            SelectionLocationTop = location.Y;

            t_selectionTrueSize = new Rect(new System.Windows.Point(location.X / zoomFactor, location.Y / zoomFactor), new System.Windows.Size(t_selection.Width / zoomFactor, t_selection.Height / zoomFactor));
            t_zoomFactor = zoomFactor;

            t_selection.MouseDown += Selection_MouseDown; // also manages the double click event, see https://stackoverflow.com/questions/1631906/distinguish-between-mouse-doubleclick-and-mouse-click-in-wpf
            t_selection.MouseMove += Selection_MouseMove;
            t_selection.MouseLeave += T_selection_MouseLeave;
            t_selection.MouseUp += Selection_MouseUp;

            container.Children.Add(t_selection);

            // handle order change when rectangle is resized and either or both sizes get a negative value
            t_handles[0] = CreateHandles(t_createDragPosition);
            t_handles[0].Order = 0;
            t_container.Children.Add(t_handles[0].Shape);

            t_handles[1] = CreateHandles(t_createDragPosition);
            t_handles[1].Order = 1;
            t_container.Children.Add(t_handles[1].Shape);

            t_handles[2] = CreateHandles(t_createDragPosition);
            t_handles[2].Order = 2;
            t_container.Children.Add(t_handles[2].Shape);

            t_handles[3] = CreateHandles(t_createDragPosition);
            t_handles[3].Order = 3;
            t_container.Children.Add(t_handles[3].Shape);

            t_matrices = new double[4][,];

            // assign the rotation angle appropriate to the default handle order
            // 0---------1
            // |         |
            // |         |
            // 3---------2

            // that is, if handle 2 is being moved, then no rotation is required, hence rotation 0
            // if handle 1 is being moved, rectangle is virtually rotated 90 so that delta calculations are same as if handle 2 being moved
            t_matrices[2] = Engine.Calc.Matrix.RotationMatrix0degrees();
            t_matrices[1] = Engine.Calc.Matrix.RotationMatrix90degrees();
            t_matrices[0] = Engine.Calc.Matrix.RotationMatrix180degrees();
            t_matrices[3] = Engine.Calc.Matrix.RotationMatrix270degrees();
        }

        private PaintualUI.Controls.SelectionHandle CreateHandles(System.Windows.Point p)
        {
            PaintualUI.Controls.SelectionHandle h = new SelectionHandle(p.X - SelectionHandle.Offset, p.Y - SelectionHandle.Offset);
            return h;
        }

        /// <summary>
        /// Called by the SelectionGlass when the selection rectangle is first created. After creation, moving selection handles
        /// events are in charge of providing new size and position values to the rectangle.
        /// </summary>
        /// <param name="point"></param>
        public void UpdateCreationSize(System.Windows.Point point)
        {
            double deltaX = point.X - t_createDragPosition.X;

            if (deltaX < 0)
            {
                deltaX = 0d;
            }

            double deltaY = point.Y - t_createDragPosition.Y;

            if (deltaY < 0)
            {
                deltaY = 0d;
            }

            t_selection.Width = deltaX;
            t_selection.Height = deltaY;

            t_selectionTrueSize.Width = t_selection.Width / t_zoomFactor;
            t_selectionTrueSize.Height = t_selection.Height / t_zoomFactor;

            // rectangle creation scenario: do not move t_handles[0]
            t_handles[1].SetPosition(point.X - SelectionHandle.Offset, t_handles[1].Position.Y);

            t_handles[2].SetPosition(point.X - SelectionHandle.Offset, point.Y - SelectionHandle.Offset);

            t_handles[3].SetPosition(t_handles[3].Position.X, point.Y - SelectionHandle.Offset);

            // the container is responsible for calling this.InvalidateVisual();
        }

        /// <summary>
        /// Relocates the handles as the selection rectangle is being moved.
        /// </summary>
        /// <param name="deltaX">The amount in pixels by which to horizontaly move the handles.</param>
        /// <param name="deltaY">The amount in pixels by which to vertically move the handles.</param>
        private void MoveHandles(double deltaX, double deltaY)
        {
            t_handles[0].SetPosition(t_handles[0].Position.X + deltaX, t_handles[0].Position.Y + deltaY);
            t_handles[1].SetPosition(t_handles[1].Position.X + deltaX, t_handles[1].Position.Y + deltaY);
            t_handles[2].SetPosition(t_handles[2].Position.X + deltaX, t_handles[2].Position.Y + deltaY);
            t_handles[3].SetPosition(t_handles[3].Position.X + deltaX, t_handles[3].Position.Y + deltaY);
        }

        /// <summary>
        /// Updates handles positions as the selection rectangle is being resized by one handle.
        /// </summary>
        private void UpdateHandlesPosition()
        {
            t_handles[0].SetPosition(SelectionLocationLeft - SelectionHandle.Offset, SelectionLocationTop - SelectionHandle.Offset);
            t_handles[1].SetPosition(SelectionLocationLeft + t_selection.Width - SelectionHandle.Offset, SelectionLocationTop - SelectionHandle.Offset);
            t_handles[2].SetPosition(SelectionLocationLeft + t_selection.Width - SelectionHandle.Offset, SelectionLocationTop + t_selection.Height - SelectionHandle.Offset);
            t_handles[3].SetPosition(SelectionLocationLeft - SelectionHandle.Offset, SelectionLocationTop + t_selection.Height - SelectionHandle.Offset);
        }

        public void UpdateZoomedSize(double zoomFactor)
        {
            // the actual size doesn't change, but the visible selection changes
            t_zoomFactor = zoomFactor;

            t_selection.Width = t_selectionTrueSize.Width * t_zoomFactor;
            t_selection.Height = t_selectionTrueSize.Height * t_zoomFactor;

            SelectionLocationLeft = t_selectionTrueSize.Left * t_zoomFactor;
            SelectionLocationTop = t_selectionTrueSize.Top * t_zoomFactor;

            UpdateHandlesPosition();
        }

        public bool AllowResize
        {
            get { return t_allowResize; }
            set {
                if (t_allowResize == false && value == true)
                {
                    t_allowResize = value;

                    foreach (SelectionHandle sh in t_handles)
                    {
                        sh.MouseDown += Sh_MouseDown;
                        sh.MouseMove += Sh_MouseMove;
                        sh.MouseUp += Sh_MouseUp;
                    }
                }
                else if(t_allowResize == true && value == false)
                {
                    t_allowResize = value;

                    foreach (SelectionHandle sh in t_handles)
                    {
                        sh.MouseDown -= Sh_MouseDown;
                        sh.MouseMove -= Sh_MouseMove;
                        sh.MouseUp -= Sh_MouseUp;
                    }
                }
            }
        }

        public System.Windows.Rect Rectangle
        {
            get
            {
                // TODO : may need to use SelectionTrueSize
                System.Windows.Rect r = new Rect(new Point(t_selectionTrueSize.Left, t_selectionTrueSize.Top), new Size(t_selectionTrueSize.Width, t_selectionTrueSize.Height));
                //System.Windows.Rect r = new Rect(new Point(SelectionLocationLeft, SelectionLocationTop), new Size(t_selection.Width, t_selection.Height));

                return r;
            }
        }

        /// <summary>
        /// Returns the distance of the selection rectangle from the left border of its container
        /// </summary>
        private double SelectionLocationLeft
        {
            get
            {
                Thickness t = t_selection.Margin;
                return t.Left;
            }
            set {
                Thickness t = t_selection.Margin;
                t.Left = value;
                t_selection.Margin = t;
            }
        }

        /// <summary>
        /// Returns the distance of the selection  rectangle from the top border of its container
        /// </summary>
        private double SelectionLocationTop
        {
            get
            {
                Thickness t = t_selection.Margin;
                return t.Top;
            }
            set
            {
                Thickness t = t_selection.Margin;
                t.Top = value;
                t_selection.Margin = t;
            }
        }

        #region Rectangle event handlers
        private void Selection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // double ckick = for some tools, this means "save selection"
            if (e.ClickCount == 2)
            {
                RaiseDoubleClick(e);
                return;
            }

            t_mobility = SelectionMobility.CanMove;
            t_movePoint = new Point(e.GetPosition(t_container).X, e.GetPosition(t_container).Y);
        }

        private void Selection_MouseMove(object sender, MouseEventArgs e)
        {
            if (t_mobility == SelectionMobility.CanMove)
            {
                double deltaX = e.GetPosition(t_container).X - t_movePoint.X;

                double deltaY = e.GetPosition(t_container).Y - t_movePoint.Y;

                Thickness t = t_selection.Margin;
                t.Left += deltaX;
                t.Top += deltaY;
                t_selection.Margin = t;

                t_selectionTrueSize.X = t.Left / t_zoomFactor;
                t_selectionTrueSize.Y = t.Top / t_zoomFactor;

                t_movePoint = new Point(e.GetPosition(t_container).X, e.GetPosition(t_container).Y);

                MoveHandles(deltaX, deltaY);
            }
        }

        private void Selection_MouseUp(object sender, MouseButtonEventArgs e)
        {
            t_mobility = SelectionMobility.CannotMove;
        }

        private void T_selection_MouseLeave(object sender, MouseEventArgs e)
        {
            t_mobility = SelectionMobility.CannotMove;
        }

        #endregion

        #region SelectionRectangle Events

        public event MouseButtonEventHandler DoubleClick;

        private void RaiseDoubleClick(MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(this, e);
            }
        }

        #endregion


        #region Events handlers for selection handles
        private void Sh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            t_resizing = Resizing.IsResizing;

            t_handleMousePoint = new Point(e.GetPosition(t_container).X, e.GetPosition(t_container).Y);

            // the handle being pressed determines a virtual selection rectangle transformation so that all
            // handles can use same resizing code
            SelectionHandle sh = (SelectionHandle)sender;

            double left = SelectionLocationLeft;
            double top = SelectionLocationTop;

            System.Windows.Point point0 = new Point(left, top);
            System.Windows.Point point1 = new Point(left + t_selection.Width, top);
            System.Windows.Point point2 = new Point(left + t_selection.Width, top + t_selection.Height);
            System.Windows.Point point3 = new Point(left, top + t_selection.Height);

            switch (sh.Order)
            {
                case 0:
                    t_currentMatrix = 0; // rotation 180 °

                    t_transformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(point2, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(point3, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(point0, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(point1, t_matrices[t_currentMatrix]);
                    break;

                case 1:
                    t_currentMatrix = 1; // rotation 90 °

                    t_transformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(point3, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(point0, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(point1, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(point2, t_matrices[t_currentMatrix]);
                    break;

                case 2:
                    t_currentMatrix = 2; // rotation 0 °

                    t_transformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(point0, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(point1, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(point2, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(point3, t_matrices[t_currentMatrix]);

                    break;

                case 3:
                    t_currentMatrix = 3; // rotation 270 °

                    t_transformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(point1, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(point2, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(point3, t_matrices[t_currentMatrix]);
                    t_transformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(point0, t_matrices[t_currentMatrix]);
                    break;
            }

            t_container.MouseMove += Sh_MouseMove;
        }

        private void Sh_MouseMove(object sender, MouseEventArgs e)
        {
            if (t_resizing == Resizing.IsResizing)
            {
                // note : because the sender can either be the SelectionHandle or the container (grid)
                // cannot use directly a reference to the SelectionHandle here. Luckily, the code does not
                // require such reference.  Grid can trigger move code because it overcomes the fact that
                // user can move the mouse faster than the SelectionHandle can be relocated, causing temporary
                // loss of focus on SelectionHandle
                // SelectionHandle sh = (SelectionHandle)sender;

                // get mouse position from the container of the rectangle not the handle
                double[] p = new double[] { e.GetPosition(t_container).X, e.GetPosition(t_container).Y };

                double deltaX = p[0] - t_handleMousePoint.X;
                double deltaY = p[1] - t_handleMousePoint.Y;

                t_handleMousePoint = new Point(p[0], p[1]);

                // transform delta to fit with transformed rectangle points
                System.Windows.Point transformedDelta = Engine.Calc.Matrix.Dot(new System.Windows.Point(deltaX, deltaY), t_matrices[t_currentMatrix]);

                // point 0 doesn't change
                //t_transformedRectanglePoints[0] = new Point(t_transformedRectanglePoints[0].X, t_transformedRectanglePoints[0].Y);
                t_transformedRectanglePoints[1] = new Point(t_transformedRectanglePoints[1].X + transformedDelta.X, t_transformedRectanglePoints[1].Y);
                t_transformedRectanglePoints[2] = new Point(t_transformedRectanglePoints[2].X + transformedDelta.X, t_transformedRectanglePoints[2].Y + transformedDelta.Y);
                t_transformedRectanglePoints[3] = new Point(t_transformedRectanglePoints[3].X, t_transformedRectanglePoints[3].Y + transformedDelta.Y);

                // untransform points
                switch (t_currentMatrix)
                {
                    case 0:
                        t_untransformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[2], t_matrices[0]);
                        t_untransformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[3], t_matrices[0]);
                        t_untransformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[0], t_matrices[0]);
                        t_untransformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[1], t_matrices[0]);
                        break;

                    case 1:
                        t_untransformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[1], t_matrices[3]);
                        t_untransformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[2], t_matrices[3]);
                        t_untransformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[3], t_matrices[3]);
                        t_untransformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[0], t_matrices[3]);
                        break;

                    case 2:
                        t_untransformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[0], t_matrices[2]);
                        t_untransformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[1], t_matrices[2]);
                        t_untransformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[2], t_matrices[2]);
                        t_untransformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[3], t_matrices[2]);
                        break;

                    case 3:
                        t_untransformedRectanglePoints[0] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[3], t_matrices[1]);
                        t_untransformedRectanglePoints[1] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[0], t_matrices[1]);
                        t_untransformedRectanglePoints[2] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[1], t_matrices[1]);
                        t_untransformedRectanglePoints[3] = Engine.Calc.Matrix.Dot(t_transformedRectanglePoints[2], t_matrices[1]);
                        break;
                }

                double width = t_untransformedRectanglePoints[1].X - t_untransformedRectanglePoints[0].X;
                double height = t_untransformedRectanglePoints[3].Y - t_untransformedRectanglePoints[0].Y;

                if (width < 0)
                {
                    System.Windows.Point tempW0 = new Point(t_untransformedRectanglePoints[0].X, t_untransformedRectanglePoints[0].Y);
                    System.Windows.Point tempW1 = new Point(t_untransformedRectanglePoints[1].X, t_untransformedRectanglePoints[1].Y);
                    System.Windows.Point tempW2 = new Point(t_untransformedRectanglePoints[2].X, t_untransformedRectanglePoints[2].Y);
                    System.Windows.Point tempW3 = new Point(t_untransformedRectanglePoints[3].X, t_untransformedRectanglePoints[3].Y);

                    t_untransformedRectanglePoints[0] = tempW1;
                    t_untransformedRectanglePoints[1] = tempW0;
                    t_untransformedRectanglePoints[2] = tempW3;
                    t_untransformedRectanglePoints[3] = tempW2;
                }

                if (height < 0)
                {
                    System.Windows.Point tempH0 = new Point(t_untransformedRectanglePoints[0].X, t_untransformedRectanglePoints[0].Y);
                    System.Windows.Point tempH1 = new Point(t_untransformedRectanglePoints[1].X, t_untransformedRectanglePoints[1].Y);
                    System.Windows.Point tempH2 = new Point(t_untransformedRectanglePoints[2].X, t_untransformedRectanglePoints[2].Y);
                    System.Windows.Point tempH3 = new Point(t_untransformedRectanglePoints[3].X, t_untransformedRectanglePoints[3].Y);

                    t_untransformedRectanglePoints[0] = tempH3;
                    t_untransformedRectanglePoints[1] = tempH2;
                    t_untransformedRectanglePoints[2] = tempH1;
                    t_untransformedRectanglePoints[3] = tempH0;
                }

                SelectionLocationLeft = t_untransformedRectanglePoints[0].X;
                SelectionLocationTop = t_untransformedRectanglePoints[0].Y;

                t_selection.Width = t_untransformedRectanglePoints[1].X - t_untransformedRectanglePoints[0].X;
                t_selection.Height = t_untransformedRectanglePoints[3].Y - t_untransformedRectanglePoints[0].Y;

                t_selectionTrueSize.Width = t_selection.Width / t_zoomFactor;
                t_selectionTrueSize.Height = t_selection.Height / t_zoomFactor;
                t_selectionTrueSize.X = SelectionLocationLeft / t_zoomFactor;
                t_selectionTrueSize.Y = SelectionLocationTop / t_zoomFactor;

                UpdateHandlesPosition();
            }
        }

        private void Sh_MouseUp(object sender, MouseButtonEventArgs e)
        {
            t_resizing = Resizing.IsNot;
            t_container.MouseMove -= Sh_MouseMove;
        }
        #endregion
    }
}
