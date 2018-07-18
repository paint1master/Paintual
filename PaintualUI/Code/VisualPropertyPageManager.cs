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
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cuisine.Windows;


namespace PaintualUI.Code
{
    /// <summary>
    /// Helps managing the interaction between the VisualPropertyPage, 
    /// its Panel container, MainWindow, the active Viom, and the graphic activity (tool, effect)
    /// </summary>
    internal class VisualPropertyPageManager
    {
        private Cuisine.Windows.DockPane t_docContent;
        private PaintualUI.Controls.PropertyPage.VisualPropertyPage t_visualPropertyPage;
        private Cuisine.Windows.WindowsManager t_manager;

        // this just to make code shorter
        private PaintualUI.Code.Application _app;

        public VisualPropertyPageManager(Cuisine.Windows.WindowsManager manager)
        {
            t_manager = manager;
            _app = PaintualUI.Code.Application.Instance;

            _app.ActiveContentHelper.CurrentDrawingBoardChanged += Refresh;
        }



        /// <summary>
        /// Loads an instance of the VisualPropertyPage and/or updates its control contents
        /// </summary>
        public void Show()
        {
            Nullify();

            PaintualUI.Controls.PropertyPage.VisualPropertyPage vpp = new PaintualUI.Controls.PropertyPage.VisualPropertyPage();
            t_visualPropertyPage = vpp;

            // required for the event that updates vpp content gets fired.
            Engine.Viome v = _app.ActiveContentHelper.GetCurrentDrawingBoard().GetViome();
            vpp.SetWorkflow(v);

            if (t_docContent == null)
            {
                t_docContent = CreateContainer();
            }
            else
            {
                t_docContent.Content = t_visualPropertyPage;
            }
        }

        /// <summary>
        /// Clears the VisualPage of any controls that may have been created for a tool or effect.
        /// </summary>
        /// <remarks>If the VisualPropertyPage has not been instanced yet, calling this method has no effect.</remarks>
        public void Clear()
        {
            if (t_visualPropertyPage != null)
            {
                t_visualPropertyPage.Clear();
            }
        }


        /// <summary>
        /// Closes the VisualPropertyPage and the visual container
        /// </summary>
        public void Close()
        {
            /*Nullify();

            if (t_docContent != null)
            {
                t_docContent.Close();
                t_docContent = null;
            }*/
        }

        /// <summary>
        /// Builds the controls in the VisualPropertyPage based on the content of the provided VisualProperties instance.
        /// </summary>
        public void Build()
        {
            if(t_visualPropertyPage != null)
            {
                Engine.Viome v = _app.ActiveContentHelper.GetCurrentDrawingBoard().GetViome();
                Engine.Tools.IGraphicActivity activity = v.CurrentActivity;
                Engine.Effects.VisualProperties vp = activity.GetVisualProperties();

                t_visualPropertyPage.Build(vp);
            }
        }

        /// <summary>
        /// Rebuilds the controls in the VisualPropertyPage and fills the controls with values saved within the Graphic activity
        /// (effect, tool) set in the provided Viome instance.
        /// </summary>
        public void Refresh()
        {
            if (t_visualPropertyPage == null)
            {
                return;
            }

            Clear();

            // Build() sets also the visual properties in the current VisualPropertyPage
            Build();

            // fill
            Engine.Viome v = _app.ActiveContentHelper.GetCurrentDrawingBoard().GetViome();
            Engine.Tools.IGraphicActivity activity = v.CurrentActivity;

            // null can occur when image is loaded, tool or effect selected but no values entered yet in VisualPropertyPage
            // Refresh is called by DockManager whenever focus changes between windows and containers
            if (activity.CollectedPropertyValues != null)
            {
                t_visualPropertyPage.Fill(activity.CollectedPropertyValues);
            }
        }

        public void Refresh(object sender, PaintualUI.Code.CurrentDrawingBoardChangedEventArgs e)
        {
            // TODO : later delete this method, we seem not needing the sender or eventargs because we now can get what we want from _app
            Refresh();
        }

        private DockPane CreateContainer()
        {
            DockPane pane = new DockPane();
            pane.Close += Pane_Close;
            pane.MinHeight = 100;
            pane.MinWidth = 100;
            pane.Header = "Visual Property Page";
            Grid g = new Grid();
            g.Background = Brushes.White;
            g.Children.Add(t_visualPropertyPage);
            pane.Content = g;

            t_manager.AddPinnedWindow(pane, Dock.Right);

            return pane;
        }

        private void Pane_Close(object sender, System.Windows.RoutedEventArgs e)
        {
            t_docContent = null;
        }

        private void Nullify()
        {
            if (t_visualPropertyPage != null)
            {
                t_visualPropertyPage = null;
            }
        }
    }
}
