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

namespace PaintualUI.Code
{
    /// <summary>
    /// Provides means to access the active drawing board among all windows opened in the DockingManager
    /// </summary>
    internal class ActiveContentHelper
    {
        private Cuisine.Windows.WindowsManager t_windowsManager;
        private PaintualUI.Controls.DrawingBoard t_currentDrawingBoard;

        // this just to make code shorter
        private PaintualUI.Code.Application _app;

        public ActiveContentHelper(Cuisine.Windows.WindowsManager windowsManager)
        {
            t_windowsManager = windowsManager;
            t_windowsManager.ActiveDocumentChanged += T_windowsManager_ActiveDocumentChanged;

            _app = PaintualUI.Code.Application.Instance;
        }

        private void T_windowsManager_ActiveDocumentChanged(object sender, Cuisine.Windows.ActiveDocumentChangedEventArgs e)
        {
            System.Windows.Controls.TabControl tabControl = e.TabControl;

            // SelectedContent is what is to become the previously selected item
            //System.Windows.Controls.Grid g = tabControl.SelectedContent as System.Windows.Controls.Grid;

            if (tabControl.SelectedItem == null)
            {
                return;
            }

            // SelectedItem is the one becoming active which gives the right DrawingBoard for our code
            Cuisine.Windows.DocumentContent dc = (Cuisine.Windows.DocumentContent)tabControl.SelectedItem;

            System.Windows.Controls.Grid g = (System.Windows.Controls.Grid)dc.Content;

            // can occur when new drawingBoard is created and is not set as a document in the DocumentContainer (one with tabs)
            if (g == null)
            {
                return;
            }

            if (g.Children[0] is PaintualUI.Controls.DrawingBoard == false)
            {
                return;
            }

            PaintualUI.Controls.DrawingBoard db = (PaintualUI.Controls.DrawingBoard)g.Children[0];

            SetCurrentDrawingBoard(db);
        }

        public void SetCurrentDrawingBoard(PaintualUI.Controls.DrawingBoard db)
        {
            if (db != t_currentDrawingBoard)
            {
                t_currentDrawingBoard = db;

                Engine.Workflow w = t_currentDrawingBoard.GetWorkflow();
                Engine.Application.Workflows.SetAsActiveWorkflow(w.Key);

                // VisualPropertyPage has registered to this event
                RaiseCurrentDrawingBoardChanged();
            }
        }

        public void DeleteDrawingBoard(PaintualUI.Controls.DrawingBoard db)
        {
            Engine.Workflow w = db.GetWorkflow();
            Engine.Application.Workflows.EndWorkflow(w.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the current drawing board, may return null.</returns>
        public PaintualUI.Controls.DrawingBoard GetCurrentDrawingBoard()
        {
             return t_currentDrawingBoard;
        }

        public event CurrentDrawingBoardChangedEventHandler CurrentDrawingBoardChanged;

        public void RaiseCurrentDrawingBoardChanged()
        {
            if (CurrentDrawingBoardChanged != null)
            {
                // the drawing board may be null in some cases
                CurrentDrawingBoardChanged(this, new CurrentDrawingBoardChangedEventArgs(t_currentDrawingBoard));
            }
        }

        public delegate void CurrentDrawingBoardChangedEventHandler(object sender, CurrentDrawingBoardChangedEventArgs e);
    }

    public class CurrentDrawingBoardChangedEventArgs : EventArgs
    {
        public PaintualUI.Controls.DrawingBoard DrawingBoard;

        public CurrentDrawingBoardChangedEventArgs(PaintualUI.Controls.DrawingBoard drawingBoard)
        {
            DrawingBoard = drawingBoard;
        }
    }
}
