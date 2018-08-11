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

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for DrawingBoard.xaml
    /// </summary>
    public partial class DrawingBoard : UserControl
    {
        private Engine.Workflow t_workflow;
        private PaintualUI.Controls.SelectionGlass t_selectionGlass;

        public DrawingBoard()
        {
            InitializeComponent();

            this.Zoomer.ZoomFactorChanged += Zoomer_ZoomFactorChanged;
        }

        private void Zoomer_ZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            if (t_workflow == null || t_workflow.Viome == null)
            {
                return;
            }

            // this will trigger viome.CoordinatesManager.ZoomFactorChanged
            // event handled in DrawingBoard.CoordinatesManager_ZoomFactorChanged which updates the scrollbars
            // and also handled by viome and trigger RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate)
            // PaintualCanvas will receive the request and update the visual
            t_workflow.Viome.CoordinatesManager.ZoomFactor = e.ZoomFactor;

            if (t_selectionGlass != null)
            {
                // this will handle and update the selection area if needed
                t_selectionGlass.ZoomFactor = e.ZoomFactor;
            }
        }

        public void SetWorkflow(Engine.Workflow w)
        {
            t_workflow = w;
            this.DrawableSurface.SetWorkflow(w);
            t_workflow.Viome.CoordinatesManager.ZoomFactorChanged += CoordinatesManager_ZoomFactorChanged;
            t_workflow.Viome.CoordinatesManager.ImagePositionChanged += CoordinatesManager_ImagePositionChanged;
            t_workflow.Viome.SelectionGlassRequested += T_viome_SelectionGlassRequested;

            // image size is known so calculate scrollbar slider size and pos
            CalculateScrollBars();
        }

        private void T_viome_SelectionGlassRequested(object sender, WorkflowSelectionGlassEventArgs e)
        {
            if (e.RequestType == SelectionGlassRequestType.Create)
            {
                t_selectionGlass = new SelectionGlass();

                // selection Glass to be the size of the drawable surface, which contains the visible canvas = the image representation
                t_selectionGlass.Width = DrawableSurface.Width; // the canvas
                t_selectionGlass.Height = DrawableSurface.Height;

                // in case user has scrolled image prior to use the selection tool
                Thickness glassMargin = t_selectionGlass.Margin;
                glassMargin.Left = DrawableSurface.Margin.Left;
                glassMargin.Top = DrawableSurface.Margin.Top;
                t_selectionGlass.Margin = glassMargin;

                // clip to bounds refer to the bounds of the drawable surface, not the visible image representation, which means
                // that the selection rectangle can go beyond the canvas.
                t_selectionGlass.ClipToBounds = true;

                t_selectionGlass.ZoomFactor = Zoomer.ZoomFactor;

                this.ContainerGrid.Children.Add(t_selectionGlass);

                Canvas.SetZIndex(t_selectionGlass, 2);

                Canvas.SetZIndex(this.scrollHor, 3);
                Canvas.SetZIndex(this.scrollVert, 4);
                Canvas.SetZIndex(this.bottomRightCorner, 5);

                Canvas.SetZIndex(this.Zoomer, 6);

                t_selectionGlass.SelectionDoubleClick += T_selectionGlass_SelectionDoubleClick;
            }

            if (e.RequestType == SelectionGlassRequestType.Delete)
            {
                if (t_selectionGlass == null)
                {
                    return;
                }

                this.ContainerGrid.Children.Remove(t_selectionGlass);

                Canvas.SetZIndex(this.scrollHor, 2);
                Canvas.SetZIndex(this.scrollVert, 3);
                Canvas.SetZIndex(this.bottomRightCorner, 4);

                Canvas.SetZIndex(this.Zoomer, 5);

                t_selectionGlass.SelectionDoubleClick -= T_selectionGlass_SelectionDoubleClick;
                t_selectionGlass = null;
            }
        }

        private void T_selectionGlass_SelectionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // sender is SelectionRectangle;

            RaiseSelectionDoubleClick(sender, e);
        }

        private void CoordinatesManager_ImagePositionChanged(object sender, ImagePositionChangedEventArgs e)
        {
            Thickness margin = this.DrawableSurface.Margin;
            margin.Left = e.OriginPoint.X;
            margin.Top = e.OriginPoint.Y;
            this.DrawableSurface.Margin = margin;

            if (t_selectionGlass != null)
            {
                Thickness glassMargin = t_selectionGlass.Margin;
                glassMargin.Left = e.OriginPoint.X;
                glassMargin.Top = e.OriginPoint.Y;
                t_selectionGlass.Margin = glassMargin;
            }

            CalculateScrollBars();
        }

        private void CoordinatesManager_ZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            CalculateScrollBars();
        }

        private void CalculateScrollBars()
        {
            // scrollbar slider position and movement calculations change depending on the virtual dimensions of
            // the image being displayed, ie: a zoomed image needs more scrolling for a user to be able to get
            // to all of its surface.
            if (t_workflow.Viome.CoordinatesManager.FactoredSize.Width <= ViewPortWidth)
            {
                this.scrollHor.IsEnabled = false;

                if (t_workflow.Viome.CoordinatesManager.Origin.X != 0)
                {
                    t_workflow.Viome.CoordinatesManager.RepositionImage(0, t_workflow.Viome.CoordinatesManager.Origin.Y);
                    this.scrollHor.Minimum = 0;
                    this.scrollHor.Maximum = ViewPortWidth;
                    this.scrollHor.Value = 0;
                }
            }
            else
            {
                this.scrollHor.IsEnabled = true;
                this.scrollHor.Minimum = 0;
                this.scrollHor.Maximum = (t_workflow.Viome.CoordinatesManager.FactoredSize.Width - ViewPortWidth);
                this.scrollHor.Value = (t_workflow.Viome.CoordinatesManager.Origin.X * -1);
            }


            if (t_workflow.Viome.CoordinatesManager.FactoredSize.Height <= ViewPortHeight)
            {
                this.scrollVert.IsEnabled = false;

                if (t_workflow.Viome.CoordinatesManager.Origin.Y != 0)
                {
                    t_workflow.Viome.CoordinatesManager.RepositionImage(t_workflow.Viome.CoordinatesManager.Origin.X, 0);
                    this.scrollVert.Minimum = 0;
                    this.scrollVert.Maximum = ViewPortHeight;
                    this.scrollVert.Value = 0;
                }
            }
            else
            {
                this.scrollVert.IsEnabled = true;
                this.scrollVert.Minimum = 0;
                this.scrollVert.Maximum = (t_workflow.Viome.CoordinatesManager.FactoredSize.Height - ViewPortHeight);
                this.scrollVert.Value = (t_workflow.Viome.CoordinatesManager.Origin.Y * -1);
            }
        }

        public Engine.Workflow GetWorkflow()
        {
            return this.DrawableSurface.GetWorkflow();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            t_workflow.Viome.CoordinatesManager.DrawingBoardSizeChanged((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height);
            // update values of the scrollbars only. PaintualCanvas tells Workflow how to process position and size of image being displayed
            CalculateScrollBars();
            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// The available width in which a PaintualCanvas can be displayed. Is calculated using .ActualWidth minus the width of the scrollbar.
        /// </summary>
        public double ViewPortWidth
        {
            get { return ActualWidth - scrollVert.ActualWidth; }
        }

        /// <summary>
        /// The available height in which a PaintualCanvas can be displayed. Is calculated using .ActualHeight minus the height of 
        /// the scrollbar and the height of the ImageZoom control.
        /// </summary>
        public double ViewPortHeight
        {
            get { return ActualHeight - (scrollHor.ActualHeight + Zoomer.ActualHeight); }
        }

        private void scrollVert_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            Thickness margin = this.DrawableSurface.Margin;
            margin.Top = (e.NewValue * -1);
            this.DrawableSurface.Margin = margin;

            if (t_selectionGlass != null)
            {
                Thickness glassMargin = t_selectionGlass.Margin;
                glassMargin.Top = (e.NewValue * -1);
                t_selectionGlass.Margin = glassMargin;
            }

            t_workflow.Viome.CoordinatesManager.RepositionImage(t_workflow.Viome.CoordinatesManager.Origin.X, (e.NewValue * -1));
        }

        private void scrollHor_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            Thickness margin = this.DrawableSurface.Margin;
            margin.Left = (e.NewValue * -1);
            this.DrawableSurface.Margin = margin;

            if (t_selectionGlass != null)
            {
                Thickness glassMargin = t_selectionGlass.Margin;
                glassMargin.Left = (e.NewValue * -1);
                t_selectionGlass.Margin = glassMargin;
            }

            t_workflow.Viome.CoordinatesManager.RepositionImage((e.NewValue * -1), t_workflow.Viome.CoordinatesManager.Origin.Y);
        }

        #region Events

        public event Engine.Utilities.Selection.SelectionEventHandler SelectionDoubleClick;

        private void RaiseSelectionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectionDoubleClick != null)
            {
                PaintualUI.Controls.SelectionRectangle sr = (PaintualUI.Controls.SelectionRectangle)sender;

                SelectionDoubleClick(sender, new Engine.Utilities.Selection.SelectionEventArgs(sr.Rectangle));
            }
        }

        #endregion
    }
}
