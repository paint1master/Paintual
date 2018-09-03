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
        private PaintualUI.Controls.SelectionGlass t_selectionGlass;

        public DrawingBoard()
        {
            InitializeComponent();

            this.Zoomer.ZoomFactorChanged += E_Zoomer_ZoomFactorChanged;
            this.Zoomer.ZoomFactorUpdateRequested += E_Zoomer_ZoomFactorUpdateRequested;

            this.Loaded += DrawingBoard_Loaded;
        }

        public DrawingBoard(Engine.Workflow w) : this()
        {
            Workflow = w;
            this.DrawableSurface.SetWorkflow(w);
            Workflow.CoordinatesManager.ZoomFactorChanged += E_CoordinatesManager_ZoomFactorChanged;
            Workflow.CoordinatesManager.ImagePositionChanged += E_CoordinatesManager_ImagePositionChanged;
            Workflow.DrawingBoardSizeRequested += E_Workflow_DrawingBoardSizeRequested;
            Workflow.SelectionGlassRequested += E_Workflow_SelectionGlassRequested;
        }

        private void DrawingBoard_Loaded(object sender, RoutedEventArgs e)
        {
            Workflow.CoordinatesManager.SetViewPortSize(ViewPortWidth, ViewPortHeight, false);
            this.Zoomer.SetZoomFactor(Workflow.CoordinatesManager.ZoomFactor);

            // image size is known so calculate scrollbar slider size and pos
            CalculateScrollBars();
        }



        private void E_Workflow_DrawingBoardSizeRequested(object sender, WorkflowDrawingBoardEventArgs e)
        {
            if (ViewPortWidth == 0 || ViewPortHeight == 0)
            {
                // the drawing board has not fully been created, probably not yet within the visual tree
                return;
            }

            Workflow.CoordinatesManager.SetViewPortSize(ViewPortWidth, ViewPortHeight, false);
        }

        private void E_Workflow_SelectionGlassRequested(object sender, WorkflowSelectionGlassEventArgs e)
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

                Canvas.SetZIndex(ScrollHor, 3);
                Canvas.SetZIndex(ScrollVert, 4);
                Canvas.SetZIndex(this.bottomRightCorner, 5);

                Canvas.SetZIndex(this.Zoomer, 6);

                t_selectionGlass.SelectionDoubleClick += E_selectionGlass_SelectionDoubleClick;
            }

            if (e.RequestType == SelectionGlassRequestType.Delete)
            {
                if (t_selectionGlass == null)
                {
                    return;
                }

                this.ContainerGrid.Children.Remove(t_selectionGlass);

                Canvas.SetZIndex(ScrollHor, 2);
                Canvas.SetZIndex(ScrollVert, 3);
                Canvas.SetZIndex(this.bottomRightCorner, 4);

                Canvas.SetZIndex(this.Zoomer, 5);

                t_selectionGlass.SelectionDoubleClick -= E_selectionGlass_SelectionDoubleClick;
                t_selectionGlass = null;
            }
        }

        private void E_selectionGlass_SelectionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // sender is SelectionRectangle;

            OnSelectionDoubleClick(sender, e);
        }

        private void E_CoordinatesManager_ImagePositionChanged(object sender, ImagePositionChangedEventArgs e)
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

        private void E_CoordinatesManager_ZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            CalculateScrollBars();
            this.Zoomer.SetZoomFactor(e.ZoomFactor);
        }

        private void E_Zoomer_ZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            if (Workflow == null)
            {
                return;
            }

            // this will trigger viome.CoordinatesManager.ZoomFactorChanged
            // event handled in DrawingBoard.CoordinatesManager_ZoomFactorChanged which updates the scrollbars
            // and also handled by viome and trigger RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate)
            // PaintualCanvas will receive the request and update the visual
            Workflow.CoordinatesManager.ZoomFactor = e.ZoomFactor;

            if (t_selectionGlass != null)
            {
                // this will handle and update the selection area if needed
                t_selectionGlass.ZoomFactor = e.ZoomFactor;
            }
        }

        private void E_Zoomer_ZoomFactorUpdateRequested(object sender, ZoomFactorUpdateRequestEventArgs e)
        {
            Workflow.CoordinatesManager.SetViewPortSize(ViewPortWidth, ViewPortHeight, true);
        }



        private void CalculateScrollBars()
        {
            // scrollbar slider position and movement calculations change depending on the virtual dimensions of
            // the image being displayed, ie: a zoomed image needs more scrolling for a user to be able to get
            // to all of its surface.
            if (Workflow.CoordinatesManager.FactoredSize.Width <= ViewPortWidth)
            {
                ScrollHor.IsEnabled = false;

                if (Workflow.CoordinatesManager.Origin.X != 0)
                {
                    Workflow.CoordinatesManager.RepositionImage(0, Workflow.CoordinatesManager.Origin.Y);
                    ScrollHor.Minimum = 0;
                    ScrollHor.Maximum = ViewPortWidth;
                    ScrollHor.Value = 0;
                }
            }
            else
            {
                ScrollHor.IsEnabled = true;
                ScrollHor.Minimum = 0;
                ScrollHor.Maximum = (Workflow.CoordinatesManager.FactoredSize.Width - ViewPortWidth);
                ScrollHor.Value = (Workflow.CoordinatesManager.Origin.X * -1);
            }


            if (Workflow.CoordinatesManager.FactoredSize.Height <= ViewPortHeight)
            {
                ScrollVert.IsEnabled = false;

                if (Workflow.CoordinatesManager.Origin.Y != 0)
                {
                    Workflow.CoordinatesManager.RepositionImage(Workflow.CoordinatesManager.Origin.X, 0);
                    ScrollVert.Minimum = 0;
                    ScrollVert.Maximum = ViewPortHeight;
                    ScrollVert.Value = 0;
                }
            }
            else
            {
                ScrollVert.IsEnabled = true;
                ScrollVert.Minimum = 0;
                ScrollVert.Maximum = (Workflow.CoordinatesManager.FactoredSize.Height - ViewPortHeight);
                ScrollVert.Value = (Workflow.CoordinatesManager.Origin.Y * -1);
            }
        }
        
        private void ScrollVert_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
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

            Workflow.CoordinatesManager.RepositionImage(Workflow.CoordinatesManager.Origin.X, (e.NewValue * -1));
        }

        private void ScrollHor_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
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

            Workflow.CoordinatesManager.RepositionImage((e.NewValue * -1), Workflow.CoordinatesManager.Origin.Y);
        }

        #region Properties
        /// <summary>
        /// The available width in which a PaintualCanvas can be displayed. Is calculated using .ActualWidth minus the width of the scrollbar.
        /// </summary>
        public double ViewPortWidth
        {
            get { return ActualWidth - ScrollVert.ActualWidth; }
        }

        /// <summary>
        /// The available height in which a PaintualCanvas can be displayed. Is calculated using .ActualHeight minus the height of 
        /// the scrollbar and the height of the ImageZoom control.
        /// </summary>
        public double ViewPortHeight
        {
            get { return ActualHeight - (ScrollHor.ActualHeight + Zoomer.ActualHeight); }
        }

        public Engine.Workflow Workflow { get; private set; }
        #endregion // Properties

        #region Events

        public event Engine.Utilities.Selection.SelectionEventHandler SelectionDoubleClick;

        private void OnSelectionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectionDoubleClick != null)
            {
                PaintualUI.Controls.SelectionRectangle sr = (PaintualUI.Controls.SelectionRectangle)sender;

                SelectionDoubleClick(sender, new Engine.Utilities.Selection.SelectionEventArgs(sr.Rectangle));
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Workflow.CoordinatesManager.DrawingBoardSizeChanged((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height);
            // update values of the scrollbars only. PaintualCanvas tells Workflow how to process position and size of image being displayed
            CalculateScrollBars();
            base.OnRenderSizeChanged(sizeInfo);
        }
        #endregion
    }
}
