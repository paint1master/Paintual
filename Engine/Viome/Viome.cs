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
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace Engine
{
    /// <summary>
    /// Viome : Visual Input Output Manager (Workflow)
    /// </summary>
    public class Viome : IDisposable
    {
        private int t_key;
        private Engine.Surface.Canvas t_canvas;

        private DrawingBoardModes t_currentDrawingBoardMode = DrawingBoardModes.None;
        private Engine.CoordinatesManager t_coordinatesManager;
        /// <summary>
        /// Interpolates mouse points and calls Draw
        /// </summary>
        private Engine.Tools.MotionAttribute t_motionAttribute;

        private Engine.Tools.IGraphicActivity t_activity;

        private Engine.Threading.BackgroundQueue t_queue;
        private bool t_allowInvalidate = false;

        private MouseAndKeyboardManagerBase t_mouseAndKeyboardManager;


        public Viome(int key)
        {
            t_key = key;
            t_coordinatesManager = new Engine.CoordinatesManager();
            t_coordinatesManager.ZoomFactorChanged += T_coordinatesManager_ZoomFactorChanged;
            t_coordinatesManager.ImagePositionChanged += T_coordinatesManager_ImagePositionChanged;
            t_motionAttribute = new Tools.MotionAttribute(this);
            t_queue = new Engine.Threading.BackgroundQueue("Workflow", true);
            t_mouseAndKeyboardManager = new MouseAndKeyboardManagerBase(this);
        }

        private void T_coordinatesManager_ImagePositionChanged(object sender, ImagePositionChangedEventArgs e)
        {
            RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate);
        }

        private void T_coordinatesManager_ZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate);
        }

        public void SetImage(Engine.Surface.Canvas image)
        {
            t_canvas = image;
            t_coordinatesManager.SetImageSize(image.Width, image.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="forceRefresh">Forces the UI to refresh the visual</param>
        public void SetImage(Engine.Surface.Canvas image, bool forceRefresh)
        {
            SetImage(image);

            if (forceRefresh)
            {
                RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate);
            }
        }

        public void SaveImage(string fileName, Engine.Surface.ImageFileFormats format)
        {
            Engine.Surface.Ops.Save(t_canvas, fileName, format);
        }

        internal void SelectionGlassRequest(SelectionGlassRequestType type)
        {
            switch(type)
            {
                case SelectionGlassRequestType.Create:
                case SelectionGlassRequestType.Delete:
                    RaiseSelectionGlassRequested(type);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(String.Format("In Engine.Workflow.SelectionGlassRequest(), type '{0}' is not supported.", type));
            }
        }

        public void SetActivity(Engine.Tools.IGraphicActivity activity)
        {
            t_activity = activity;
            t_activity.Initialize(this);

            if (t_activity is Engine.Effects.EffectBase)
            {
                // for effects, must disable mouse input in workflow because
                // they are working on different thread and cause failure in EffectBase.Process()
                t_currentDrawingBoardMode = DrawingBoardModes.Disabled;

                // DrawingBoardMode can be re-enabled by clicking on "Draw" button in UI
                // the Draw button remplaces the current activity with a (drawing) tool.
            }
            
            if(t_activity is Engine.Tools.Tool)
            {
                t_currentDrawingBoardMode = DrawingBoardModes.None; // will cycle to SuspendDraw and Draw
            }

            if (t_activity.HasVisualProperties)
            {
                RaisePropertyPageActionRequested(WorkflowPropertyPageRequestType.ContentUpdate);
            }
            else
            {
                RaisePropertyPageActionRequested(WorkflowPropertyPageRequestType.ClosePage);
            }
        }

        public void FeedMouseAction(Engine.MousePoint e)
        {
            t_mouseAndKeyboardManager.FeedMouseAction(e);
        }

        /// <summary>
        /// Feeds the Workflow with key code information from the UI
        /// </summary>
        /// <param name="e"></param>
        public void FeedKeyCode(KeyEventArgs e)
        {
            t_mouseAndKeyboardManager.FeedKeyCode(e);
        }

        public System.Windows.Media.Imaging.BitmapSource GetBmpSource()
        {
            // TODO just get the updated portion of the image to be displayed here
            BitmapSource bmpSource = BitmapSource.Create(t_canvas.Width, t_canvas.Height, 96, 96, PixelFormats.Bgra32, null, t_canvas.Array, t_canvas.Stride);

            return bmpSource;
        }

        internal void AllowInvalidate()
        {
            t_allowInvalidate = true;
        }

        internal void DisallowInvalidate()
        {
            t_allowInvalidate = false;
        }

        /// <summary>
        /// When true, can be used by a UI element to be informed that the canvas has been updated. This is set to true when there is mouse
        /// actions related to drawing actions. Is automatically set to false when mouse activity ends.
        /// </summary>
        public bool AllowCanvasRefresh
        {
            get { return t_allowInvalidate; }
        }

        public Engine.Surface.Canvas Canvas
        {
            get { return t_canvas; }
        }

        public Engine.CoordinatesManager CoordinatesManager
        {
            get { return t_coordinatesManager; }
        }

        public Engine.Tools.IGraphicActivity CurrentActivity
        {
            get { return t_activity; }
        }

        public Engine.Effects.EffectBase CurrentEffect
        {
            get
            {
                if (t_activity is Engine.Effects.EffectBase)
                {
                    return (Engine.Effects.EffectBase)t_activity;
                }

                return null;
            }
        }

        public Engine.Tools.Tool CurrentTool
        {
            get
            {
                if (t_activity is Engine.Tools.Tool)
                {
                    return (Engine.Tools.Tool)t_activity;
                }

                return null;
            }
        }

        internal DrawingBoardModes CurrentDrawingBoardMode { get => t_currentDrawingBoardMode; set => t_currentDrawingBoardMode = value; }

        public int Key
        {
            get { return t_key; }
        }

        internal Engine.Tools.MotionAttribute MotionAttribute
        {
            get { return t_motionAttribute; }
        }

        internal Engine.Threading.BackgroundQueue ThreadingQueue
        {
            get { return t_queue; }
        }

        #region Events

        public event WorkflowDrawingBoardEventHandler DrawingBoardSizeRequested;
        public event WorkflowDrawingBoardEventHandler InvalidateRequested;
        public event WorkflowSelectionGlassEventHandler SelectionGlassRequested;

        // the following not used at this time
        public event WorkflowPropertyPageEventHandler PropertyPageContentUpdateRequested;

        private void RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType requestType)
        {
            /*if (requestType == WorkflowDrawingBoardRequestType.ZoomIn)
            {
                if (ZoomInRequested != null)
                {
                    ZoomInRequested(this, new WorkflowDrawingBoardEventArgs(requestType));
                }

                return;
            }

            if (requestType == WorkflowDrawingBoardRequestType.ZoomOut)
            {
                if (ZoomOutRequested != null)
                {
                    ZoomOutRequested(this, new WorkflowDrawingBoardEventArgs(requestType));
                }

                return;
            }*/

            if (requestType == WorkflowDrawingBoardRequestType.Invalidate)
            {
                if (InvalidateRequested != null)
                {
                    InvalidateRequested(this, new WorkflowDrawingBoardEventArgs(requestType));
                }

                return;
            }

            if (requestType == WorkflowDrawingBoardRequestType.ViewPortSize)
            {
                if (DrawingBoardSizeRequested != null)
                {
                    DrawingBoardSizeRequested(this, new WorkflowDrawingBoardEventArgs(requestType));
                }

                return;
            }
        }

        private void RaisePropertyPageActionRequested(WorkflowPropertyPageRequestType requestType)
        {
            if (requestType == WorkflowPropertyPageRequestType.ContentUpdate)
            {
                if (PropertyPageContentUpdateRequested != null)
                {
                    PropertyPageContentUpdateRequested(this, new WorkflowPropertyPageEventArgs(requestType));
                }

                return;
            }
        }

        private void RaiseSelectionGlassRequested(SelectionGlassRequestType requestType)
        {
            if (SelectionGlassRequested != null)
            {
                SelectionGlassRequested(this, new WorkflowSelectionGlassEventArgs(requestType));
            }
        }

        #endregion

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
                        t_canvas = null;
                        t_activity = null;

                        t_coordinatesManager.Dispose();
                        t_coordinatesManager = null;

                        t_queue = null;

                        t_mouseAndKeyboardManager = null;

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

    #region Enums
    public enum SelectionGlassRequestType
    {
        Create,
        Delete
    }

    public enum WorkflowDrawingBoardRequestType
    {
        ZoomIn,
        ZoomOut,
        Invalidate,
        ViewPortSize
    }

    public enum WorkflowPropertyPageRequestType
    {
        ContentUpdate,
        ClosePage
    }

    public enum DrawingBoardModes
    {
        Disabled,
        None,
        Pan,
        SuspendPan,
        Draw,
        SuspendDraw
    }
    #endregion

    public delegate void WorkflowDrawingBoardEventHandler(object sender, WorkflowDrawingBoardEventArgs e);
    public delegate void WorkflowSelectionGlassEventHandler(object sender, WorkflowSelectionGlassEventArgs e);

    public delegate void WorkflowPropertyPageEventHandler(object sender, WorkflowPropertyPageEventArgs e);



    public class WorkflowDrawingBoardEventArgs : EventArgs
    {
        public WorkflowDrawingBoardRequestType RequestType;

        public WorkflowDrawingBoardEventArgs(WorkflowDrawingBoardRequestType requestType)
        {
            this.RequestType = requestType;
        }
    }

    public class WorkflowPropertyPageEventArgs : EventArgs
    {
        public WorkflowPropertyPageRequestType RequestType;

        public WorkflowPropertyPageEventArgs(WorkflowPropertyPageRequestType requestType)
        {
            this.RequestType = requestType;
        }
    }

    public class WorkflowSelectionGlassEventArgs : EventArgs
    {
        public SelectionGlassRequestType RequestType;

        public WorkflowSelectionGlassEventArgs(SelectionGlassRequestType requestType)
        {
            this.RequestType = requestType;
        }
    }
}
