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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Cuisine.Windows;
using Microsoft.Win32; // for OpenFileDialog


namespace PaintualUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string t_defaultDocTabTitle = "Documement ";

        // this just to make code shorter
        private PaintualUI.Code.Application _app;

        public MainWindow()
        {
            InitializeComponent();

            // the VisualPropertyPage will register and unregister this event itself
            // but MainWindow must also be aware of change for other operations
            // note : ActiveContentChanged does not refer to the document that has focus but to the content which contains
            // control focus 
            WindowsManager.ActiveDocumentChanged += WindowsManager_ActiveDocumentChanged;
            WindowsManager.DocumentClosing += WindowsManager_DocumentClosing;

            _app = PaintualUI.Code.Application.Instance;

            // order of the following two lines is important, VisualPropertyPageHandler needs ref to ActiveContentHelper
            _app.ActiveContentHelper = new Code.ActiveContentHelper(WindowsManager);
            _app.VisualPropertyPageHandler = new Code.VisualPropertyPageManager(WindowsManager);

            // TODO : use this to know value on Surface and laptop
            //MessageBox.Show(Engine.Calc.Matrix.NumericsVectorCount().ToString());
        }

        private void WindowsManager_DocumentClosing(Cuisine.Windows.DocumentContent documentContent)
        {
            ;
        }

        private void WindowsManager_ActiveDocumentChanged(object sender, Cuisine.Windows.ActiveDocumentChangedEventArgs e)
        {
            // active document and workflow info have been already set in ActiveContentManager,
            // here we need to propagate the change to any loaded window that must be informed

            // for the VisualPropertyPage, it is through ActiveContentHelper : VisualPropertyPage.Refresh()
            // to be called only if a different drawing board is becoming active. see ActiveContentHelper.SetCurrentDrawingBoard()
        }

        private void New_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // keep this code for when C++ code will be linked again to the project
            /*MeaningOfLife.Cpp.CLI.Logic.InitializeLibrary(@"D:\Docs\My Projects\PaintualW\x64\Debug\EngineC.dll");

            int ii = 0;

            using (var wrapper = new MeaningOfLife.Cpp.CLI.Logic())
            {
                ii = wrapper.Get();
            }

            string title = t_defaultDocTabTitle;

            DocumentContent[] cnts = dockManager.Documents.ToArray();
            int i = 1; //ii;
            while (cnts.FirstOrDefault(c => c.Title == title) != null)
            {
                title = string.Format(t_defaultDocTabTitle + "{0}", i);
                i++;
            }*/

            NewDrawingBoard("");
        }

        private void Open_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "JPG Images (*.jpg)|*.jpg|PNG Images (*.png)|*.png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                NewDrawingBoard(openFileDialog.FileName);
            }
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.Preferences pref = new Controls.Preferences();

            DockPane pane = new DockPane();
            pane.MinHeight = 24;
            pane.MinWidth = 500;
            pane.Header = "Preferences";
            Grid g = new Grid();
            g.Background = Brushes.DarkGray;
            g.Children.Add(pref);
            pane.Content = g;
            Dock direction = Dock.Right;

            pane.Close += Pane_Close;

            WindowsManager.AddPinnedWindow(pane, direction);
        }

        // this needed to clear ref to Pane otherwise when closed another instance will not be created
        private void Pane_Close(object sender, RoutedEventArgs e)
        {
            // TODO: see VisualPropertyPage t_docContent
        }

        private void Extraction_QuickExtractAndSave_Click(object sender, RoutedEventArgs e)
        {
            _app.VisualPropertyPageHandler.Clear();

            PaintualUI.Controls.DrawingBoard db = _app.ActiveContentHelper.GetCurrentDrawingBoard();

            if (db == null)
            {
                MessageBox.Show("Select a drawing board first");
                return;
            }

            Engine.Viome w = db.GetViome();

            Engine.Tools.QuickExtractAndSave qeas = new Engine.Tools.QuickExtractAndSave();

            w.SetActivity(qeas);

            _app.VisualPropertyPageHandler.Show();

            _app.VisualPropertyPageHandler.Build();

            // TODO : determine when double click should not be handled anymore and release handler
            db.SelectionDoubleClick += qeas.HandleDoubleClick;
        }

        private void NewDrawingBoard(string fileName)
        {
            Engine.Surface.Canvas canvas = null;

            if (string.IsNullOrEmpty(fileName))
            {
                canvas = new Engine.Surface.Canvas(600, 600, Engine.Color.Cell.ShadeOfGray(255));
            }
            else
            {
                canvas = new Engine.Surface.Canvas(fileName);
            }

            var db = new PaintualUI.Controls.DrawingBoard();

            // canvas must be provided before WorkflowCollection raises change event
            // because subscribed VisualPropertyPage will require canvas very early
            Engine.Viome w = Engine.Application.Viomes.NewWorkflow(canvas);
            int key = w.Key;
            w.SetActivity(new Engine.Tools.ThinLineTool());
            db.SetWorkflow(w);

            DockPane pane = new DockPane();
            pane.MinHeight = 100;
            pane.MinWidth = 100;
            pane.Header = String.Format(t_defaultDocTabTitle + "{0}", key);
            Grid g = new Grid();
            g.Background = Brushes.White;
            g.Children.Add(db);
            pane.Content = g;

            WindowsManager.AddDocument(pane);
            _app.ActiveContentHelper.SetCurrentDrawingBoard(db);
        }

        private void BrushImages_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.BrushImageList bil = new Controls.BrushImageList();

            DockPane pane = new DockPane();
            pane.MinHeight = 100;
            pane.MinWidth = 100;
            pane.Header = "Brush Image List";
            Grid g = new Grid();
            g.Background = Brushes.White;
            g.Children.Add(bil);
            pane.Content = g;
            Dock direction = Dock.Right;

            WindowsManager.AddPinnedWindow(pane, direction);
        }

        private void TestWindow_Click(object sender, RoutedEventArgs e)
        {
            /*PaintualW.Controls.TestWindow w = new Controls.TestWindow();

            var newContent = new DocumentContent();
            newContent.ContentTypeDescription = "Test Window";
            newContent.Title = "Test Window";
            newContent.InfoTip = System.IO.Path.GetTempPath();
            newContent.Content = w;
            newContent.Show(dockManager, false);
            newContent.Activate();*/
        }

        private void ColorPickerStandard_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.ColorPicker.ColorPickerStandard pickerStandard = new Controls.ColorPicker.ColorPickerStandard();

            DockPane pane = new DockPane();
            pane.MinHeight = 100;
            pane.MinWidth = 100;
            pane.Header = "Color Picker (Standard)";
            Grid g = new Grid();
            g.Background = Brushes.White;
            g.Children.Add(pickerStandard);
            pane.Content = g;
            Dock direction = Dock.Right;

            WindowsManager.AddPinnedWindow(pane, direction);
        }

        private void NoiseFactory_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Effects.Noise.NoiseFactory());
        }

        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.DrawingBoard db = _app.ActiveContentHelper.GetCurrentDrawingBoard();

            if (db == null)
            {
                MessageBox.Show("Select a drawing board first");
                return;
            }

            Engine.Viome w = db.GetViome();

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PNG files(*.png)|*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                w.SaveImage(dialog.FileName, Engine.Surface.ImageFileFormats.PNG);
            }
        }

        private void SetActivity(Engine.Tools.IGraphicActivity activity)
        {
            _app.VisualPropertyPageHandler.Clear();

            Engine.Viome w = GetViomeForCurrentDrawingBoard();

            w.SetActivity(activity);

            _app.VisualPropertyPageHandler.Show();

            _app.VisualPropertyPageHandler.Build();
        }

        private Engine.Viome GetViomeForCurrentDrawingBoard()
        {
            PaintualUI.Controls.DrawingBoard db = _app.ActiveContentHelper.GetCurrentDrawingBoard();

            if (db == null)
            {
                MessageBox.Show("Select a drawing board first");
                return null;
            }

            Engine.Viome v = db.GetViome();

            return v;
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // update Preferences

            // persist app layout windows through serializing

            Close();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // gets key input from keyboard when no other control has focus or a control has
            // allowed key event to bubble up.
            //MessageBox.Show("key");
        }

        private void BtnDraw_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Tools.ThinLineTool());
        }

        private void BtnQuickSelect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
