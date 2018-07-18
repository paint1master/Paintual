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

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for PaintualCanvas.xaml
    /// </summary>
    public partial class PaintualCanvas : UserControl, IDisposable
    {
        private Engine.Viome t_viome;
        private System.Windows.Threading.DispatcherTimer t_timer;

        public PaintualCanvas()
        {
            InitializeComponent();

            t_timer = new System.Windows.Threading.DispatcherTimer();
            t_timer.Tick += new EventHandler(UpdateVisual);
            t_timer.Interval = new TimeSpan(0, 0, 0, 0, 35); // below 35 not all draw points are shown on MouseUp // they will on next mouse down
            // TODO : workflow to be able to inform PaintualCanvas that the timer can be stopped when there is no drawing activity, ie Effect activity
            t_timer.Start();
        }

        public void SetWorkflow(Engine.Viome w)
        {
            t_viome = w;
            t_viome.InvalidateRequested += T_viome_InvalidateRequested;
        }

        public Engine.Viome GetViome()
        {
             return t_viome; 
        }

        private void T_viome_InvalidateRequested(object sender, Engine.WorkflowDrawingBoardEventArgs e)
        {
            this.InvalidateVisual();
        }

        private void DrawingBoardSizeHasChanged(int width, int height)
        {
            t_viome.CoordinatesManager.DrawingBoardSizeChanged(width, height);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (t_viome == null)
            {
                return;
            }

            drawingContext.DrawImage(t_viome.GetBmpSource(), t_viome.CoordinatesManager.GetImageSizeAndPosition());

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// method signature to match required EventHandler of DispatcherTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateVisual(object sender, EventArgs e)
        {
            if (t_viome.AllowCanvasRefresh)
            {
                this.InvalidateVisual();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Engine.MousePoint mp = new Engine.MousePoint(e.GetPosition(this).X, e.GetPosition(this).Y, Engine.MouseActionType.MouseDown);
            t_viome.FeedMouseAction(mp);
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Engine.MousePoint mp = new Engine.MousePoint(e.GetPosition(this).X, e.GetPosition(this).Y, Engine.MouseActionType.MouseMove);
            t_viome.FeedMouseAction(mp);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Engine.MousePoint mp = new Engine.MousePoint(e.GetPosition(this).X, e.GetPosition(this).Y, Engine.MouseActionType.MouseUp);
            t_viome.FeedMouseAction(mp);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            /*MessageBox.Show("key pressed in paintual canvas");
            e.Handled = true;*/
        }

        /*private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            t_workflow.FeedKeyCode(e);
            MessageBox.Show("key pressed in paintual canvas");
        }*/

        #region Dispose
        // for details see D:\Docs\My Projects\bmp_creator\Dispose which leads to http://dave-black.blogspot.ca/2011/03/how-do-you-properly-implement.html

        // remember to make this class inherit from IDisposable -> MyDisposableClass : IDisposable

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Default initialization for a bool is 'false'</remarks>
        private bool IsDisposed { get; set; }

        /// <summary>
        /// Implementation of Dispose according to .NET Framework Design Guidelines.
        /// </summary>
        /// <remarks>Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.

            // Always use SuppressFinalize() in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overloaded Implementation of Dispose.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// <list type="bulleted">Dispose(bool isDisposing) executes in two distinct scenarios.
        /// <item>If <paramref name="isDisposing"/> equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.</item>
        /// <item>If <paramref name="isDisposing"/> equals <c>false</c>, the method has been called 
        /// by the runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</item></list>
        /// </remarks>
        protected virtual void Dispose(bool isDisposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            try
            {
                if (!this.IsDisposed)
                {
                    // Explicitly set root references to null to expressly tell the GarbageCollector
                    // that the resources have been disposed of and it's ok to release the memory 
                    // allocated for them.
                    if (isDisposing)
                    {
                        // Release all managed resources here
                        // Need to unregister/detach yourself from the events. Always make sure
                        // the object is not null first before trying to unregister/detach them!
                        // Failure to unregister can be a BIG source of memory leaks
                        /*if (someDisposableObjectWithAnEventHandler != null)
                        {
                            someDisposableObjectWithAnEventHandler.SomeEvent -= someDelegate;
                            someDisposableObjectWithAnEventHandler.Dispose();
                            someDisposableObjectWithAnEventHandler = null;
                        }*/
                        if (t_viome != null)
                        {
                            t_viome.InvalidateRequested -= T_viome_InvalidateRequested;
                            // do not delete t_workflow because it doesn't belong to PaintualCanvas.
                        }
                        // If this is a WinForm/UI control, uncomment this code
                        //if (components != null)
                        //{
                        //    components.Dispose();
                        //}
                    }
                    // Release all unmanaged resources here  
                    // (example)             if (someComObject != null && Marshal.IsComObject(someComObject))
                    //{
                    //    Marshal.FinalReleaseComObject(someComObject);
                    //    someComObject = null;
                    //}
                }
            }
            finally
            {
                this.IsDisposed = true;
            }
        }

        //TODO Uncomment this code if this class will contain members which are UNmanaged
        ///// <summary>Finalizer for MyDisposableClass</summary>
        ///// <remarks>This finalizer will run only if the Dispose method does not get called.
        ///// It gives your base class the opportunity to finalize.
        ///// DO NOT provide finalizers in types derived from this class.
        ///// All code executed within a Finalizer MUST be thread-safe!</remarks>
        //  ~MyDisposableClass()
        //  {
        //     Dispose( false );
        //  }
        #endregion


    }
}
