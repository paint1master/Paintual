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

using System.Windows.Controls;
using System.Windows.Media;

using Cuisine.Windows;

namespace PaintualUI.Code
{
    /// <summary>
    /// Helps managing the interaction between the VisualPropertyPage, 
    /// its Panel container, MainWindow, the active Viom, and the graphic activity (tool, effect, animation)
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
        /// Creates an instance of the VisualPropertyPage and has it fills its content according to the
        /// current DrawingBoard and selected tool, effect, animation.
        /// </summary>
        public void Show(Engine.Workflow w)
        {
            if (t_visualPropertyPage != null)
            {          
                t_docContent.Close -= Pane_Close;
                Grid g = (Grid)t_docContent.Content;
                g.Children.Remove(t_visualPropertyPage);
                t_visualPropertyPage = null;
                g = null;

                t_visualPropertyPage = new PaintualUI.Controls.PropertyPage.VisualPropertyPage();
                g = new Grid();
                g.Background = Brushes.DarkGray;
                t_docContent.Content = g;
                g.Children.Add(t_visualPropertyPage);

                t_docContent.Close += Pane_Close;

                t_visualPropertyPage.Build(w);
                return;
            }

            t_visualPropertyPage  = new PaintualUI.Controls.PropertyPage.VisualPropertyPage();

            if (t_docContent == null)
            {
                t_docContent = CreateContainer();
            }

            t_visualPropertyPage.Build(w);
        }

        public void Refresh(object sender, PaintualUI.Code.CurrentDrawingBoardChangedEventArgs e)
        {
            Engine.Workflow w = _app.ActiveContentHelper.GetCurrentDrawingBoard().Workflow;

            Show(w);
        }

        private DockPane CreateContainer()
        {
            DockPane pane = new DockPane();

            pane.MinHeight = 100;
            pane.MinWidth = 200;
            pane.MaxWidth = 400;
            pane.Header = "Visual Property Page";
            Grid g = new Grid();
            g.Background = Brushes.DarkGray;
            g.Children.Add(t_visualPropertyPage);
            pane.Content = g;

            pane.Close += Pane_Close;

            t_manager.AddPinnedWindow(pane, Dock.Right);

            return pane;
        }

        private void Pane_Close(object sender, System.Windows.RoutedEventArgs e)
        {
            t_docContent = null;
        }
    }
}
