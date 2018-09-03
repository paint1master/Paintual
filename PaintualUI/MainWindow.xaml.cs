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
using System.Diagnostics;
using System.Runtime.InteropServices;
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


        private System.IO.MemoryStream _stream = new System.IO.MemoryStream(); // for serialization

        public MainWindow()
        {
            this.WindowState = WindowState.Maximized;

            InitializeComponent();

            // the VisualPropertyPage will register and unregister this event itself
            // but MainWindow must also be aware of change for other operations
            // note : ActiveContentChanged does not refer to the document that has focus but to the content which contains
            // control focus 
            WindowsManager.ActiveDocumentChanged += E_WindowsManager_ActiveDocumentChanged;
            WindowsManager.DocumentClosing += E_WindowsManager_DocumentClosing;

            _app = PaintualUI.Code.Application.Instance;

            // order of the following two lines is important, VisualPropertyPageHandler needs ref to ActiveContentHelper
            _app.ActiveContentHelper = new Code.ActiveContentHelper(WindowsManager);
            _app.VisualPropertyPageManager = new Code.VisualPropertyPageManager(WindowsManager);

            // to hide bug that crashes app sometimes when first file open is one from file system.
            NewDrawingBoard("");

            this.Closing += E_MainWindow_Closing;
        }

        #region App Functionality

        private void NewDockPane(PaintualUI.Controls.DrawingBoard db, string title)
        {
            DockPane pane = new DockPane();
            pane.MinHeight = 100;
            pane.MinWidth = 100;
            pane.Header = title;
            Grid g = new Grid();
            g.Background = Brushes.White;
            g.Children.Add(db);
            pane.Content = g;

            WindowsManager.AddDocument(pane);
        }

        private void NewDrawingBoard(string fileName)
        {
            Engine.Workflow w = Engine.WorkflowCollection.NewWorkflow();

            if (string.IsNullOrEmpty(fileName))
            {
                w.SetCanvas(new Engine.Surface.Canvas(1200, 1200, Engine.Colors.White));
            }
            else
            {
                try
                {
                    w.SetCanvas(new Engine.Surface.Canvas(fileName));
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Error reading file : ", e.Message));
                }
            }

            var db = new PaintualUI.Controls.DrawingBoard(w);

            NewDockPane(db, String.Format(t_defaultDocTabTitle + "{0}", w.Key));

            _app.ActiveContentHelper.SetCurrentDrawingBoard(db);

            // a suggested default tool to work with the new DrawingBoard
            SetActivity(new Engine.Tools.GrainyPen());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Separated from menu item event handler to keep "application" code in the same
        /// region in this code page.</remarks>
        private void OpenFile()
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

        // this needed to clear ref to Pane otherwise when closed another instance will not be created
        private void Pane_Close(object sender, RoutedEventArgs e)
        {
            // TODO: see VisualPropertyPage t_docContent
        }


        private void SetActivity(Engine.Tools.IGraphicActivity activity)
        {
            Engine.Workflow w = GetWorkflowForCurrentDrawingBoard();

            if (w == null)
            {
                return;
            }

            w.SetActivity(activity);

            _app.VisualPropertyPageManager.Show(w);
        }

        private Engine.Workflow GetWorkflowForCurrentDrawingBoard()
        {
            PaintualUI.Controls.DrawingBoard db = _app.ActiveContentHelper.GetCurrentDrawingBoard();

            if (db == null)
            {
                MessageBox.Show("Select a drawing board first");
                return null;
            }

            Engine.Workflow w = db.Workflow;

            return w;
        }

        private void ButtonSerialization_Save(object sender, System.Windows.RoutedEventArgs e)
        {
            /*_stream.SetLength(0);
            _stream.Seek(0, SeekOrigin.Begin);
            new XmlWindowsManagerSerializer((xmlElement, dockPane) => xmlElement.SetAttribute("Data", dockPane.Tag.ToString()), arg => arg.DockPane.Tag.ToString()).Serialize(_stream, WindowsManager);
            */
        }


        private void ButtonDeserialization_Load(object sender, System.Windows.RoutedEventArgs e)
        {
            /*_stream.Seek(0, SeekOrigin.Begin);

            WindowsManager.Clear();

            if (_stream.Length == 0)
            {
                return;
            }

            new XmlWindowsManagerDeserializer((dockpane, data) =>
            {
                dockpane.Header = "Solution Explorer";
                dockpane.Tag = int.Parse(data);
                Grid g = new Grid();
                g.Background = Brushes.White;
                TextBlock text = new TextBlock();
                text.Text = "Some content - " + data;
                g.Children.Add(text);
                dockpane.Content = g;
            }).Deserialize(_stream, WindowsManager);
            */
        }

        #endregion // App functionality

        #region Event handlers

        private void E_MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Engine.EngineCppLibrary.FreeLibrary();
        }

        private void E_WindowsManager_DocumentClosing(Cuisine.Windows.DocumentContent documentContent)
        {
            System.Windows.Controls.Grid g = (System.Windows.Controls.Grid)documentContent.Content;

            // can occur when new drawingBoard is created and is not set as a document in the DocumentContainer (one with tabs)
            if (g == null)
            {
                return;
            }

            if (g.Children[0] is PaintualUI.Controls.DrawingBoard == false)
            {
                // TODO : may need to handle closing of VisualPropertyPage (docPane)
                return;
            }

            PaintualUI.Controls.DrawingBoard db = (PaintualUI.Controls.DrawingBoard)g.Children[0];

            _app.ActiveContentHelper.DeleteDrawingBoard(db);
        }

        private void E_WindowsManager_ActiveDocumentChanged(object sender, Cuisine.Windows.ActiveDocumentChangedEventArgs e)
        {
            // active document, workflow info have been already set in ActiveContentManager,
            // here we need to propagate the change to any loaded window that must be informed

            // for the VisualPropertyPage, it is through ActiveContentHelper : VisualPropertyPage.Refresh()
            // to be called only if a different drawing board is becoming active. see ActiveContentHelper.SetCurrentDrawingBoard()
        }



        #endregion

        #region Menu Items

        private void New_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewDrawingBoard("");
        }

        private void Open_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
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

        private void Flow_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Effects.Flow_2());
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Effects.BlurEffect());
        }

        /*private void Ripple_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Tools.Ripple());
        }*/

        private void Brightness_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Tools.Brightness());
        }

        private void Extraction_QuickExtractAndSave_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.DrawingBoard db = _app.ActiveContentHelper.GetCurrentDrawingBoard();

            if (db == null)
            {
                MessageBox.Show("Select a drawing board first");
                return;
            }

            Engine.Workflow w = db.Workflow;

            Engine.Tools.QuickExtractAndSave qeas = new Engine.Tools.QuickExtractAndSave();

            w.SetActivity(qeas);

            _app.VisualPropertyPageManager.Show(w);

            // TODO : determine when double click should not be handled anymore and release handler
            db.SelectionDoubleClick += qeas.HandleDoubleClick;
        }
        
        /*private void BrushImages_Click(object sender, RoutedEventArgs e)
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
        }*/

        private void TestWindow_Click(object sender, RoutedEventArgs e)
        {
            ControlTestWindow ctw = new ControlTestWindow();
            ctw.Show();

        }

        private void ColorPickerStandard_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.ColorPicker.TColorPickerStandard pickerStandard = new Controls.ColorPicker.TColorPickerStandard();

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

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PNG files(*.png)|*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                Engine.Workflow w = db.Workflow;
                w.LastSavedFolder = dialog.FileName;
                w.SaveImage(dialog.FileName, Engine.Surface.ImageFileFormats.PNG);
            }
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
        
        private void BtnQuickSelect_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void GrainyTool_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Tools.GrainyPen());
        }

        private void ScanGlitch_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Effects.Scanner.Glitch());
        }

        private void ScanRadial_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Effects.Scanner.Radial());
        }

        private void ParticlePen_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Tools.ParticlePen());
        }

        private void AttractorPen_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Tools.AttractorPen());
        }

        private void VarianceGradient_Click(object sender, RoutedEventArgs e)
        {
            SetActivity(new Engine.Effects.VarianceGradientEffect());
        }

        #endregion
    }
}
