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
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Engine;

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for SelectionGlass.xaml
    /// </summary>
    public partial class SelectionGlass : UserControl
    {
        private bool t_selectionMade;

        private PaintualUI.Controls.SelectionRectangle t_selection;

        private double t_zoomFactor;

        private Engine.DrawingBoardModes t_drawingMode = Engine.DrawingBoardModes.None;

        public SelectionGlass()
        {
            InitializeComponent();

            t_selectionMade = false;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (t_selectionMade == false)
            {
                // creates the selection rectangle and adds it to the container
                t_selection = new PaintualUI.Controls.SelectionRectangle(Glass, e.GetPosition(this), t_zoomFactor);
                t_selection.DoubleClick += T_selection_DoubleClick;

                t_drawingMode = Engine.DrawingBoardModes.Draw;
            }
        }

        private void T_selection_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            RaiseSelectionDoubleClick(sender, e);
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (t_drawingMode == Engine.DrawingBoardModes.Draw)
            {
                t_selection.UpdateCreationSize(e.GetPosition(this));

                this.InvalidateVisual();
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // this can happen when user moves mouse over drawign board before clicking the Apply button
            if (t_selection == null)
            {
                return;
            }

            t_selectionMade = true;
            t_drawingMode = Engine.DrawingBoardModes.None;
            t_selection.AllowResize = true;
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            /*MessageBox.Show("key pressed in paintual canvas");
            e.Handled = true;*/
        }

        public double ZoomFactor
        {
            get { return t_zoomFactor; }
            set
            {
                t_zoomFactor = value;

                if (t_selectionMade)
                {
                    t_selection.UpdateZoomedSize(t_zoomFactor);

                    this.InvalidateVisual();
                }
            }
        }

        #region Events
        public event MouseButtonEventHandler SelectionDoubleClick;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">usually the SelectionRectangle that raised the double click event.</param>
        /// <param name="e"></param>
        private void RaiseSelectionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectionDoubleClick != null)
            {
                SelectionDoubleClick(sender, e);
            }
        }

        #endregion

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    DrawingContext dc = drawingContext;

        //    dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(100, 80, 180)), null, t_rect); 

        //    //base.OnRender(drawingContext);
        //}

        /*private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            t_workflow.FeedKeyCode(e);
            MessageBox.Show("key pressed in paintual canvas");
        }*/
    }
}
