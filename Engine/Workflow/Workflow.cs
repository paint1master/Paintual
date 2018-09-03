using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace Engine
{
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
        ViewPortSize,
        HandleEndOfProcess,
        DetachHandleEndOfProcess
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

    public class Workflow : IDisposable
    {
        private Engine.Surface.Canvas t_canvas;

        /// <summary>
        /// Interpolates mouse points
        /// </summary>
        private Engine.MotionAttribute t_motionAttribute;

        private Engine.Threading.BackgroundQueue t_queue;

        private MouseAndKeyboardManagerBase t_mouseAndKeyboardManager;

        internal Workflow(int key)
        {
            Key = key;

            DrawingBoardMode = DrawingBoardModes.None;
            AllowInvalidate = false;

            CoordinatesManager = new Engine.CoordinatesManager();
            CoordinatesManager.ZoomFactorChanged += E_coordinatesManager_ZoomFactorChanged;
            t_motionAttribute = new Engine.MotionAttribute();
            t_queue = new Engine.Threading.BackgroundQueue("Workflow", true);
            t_mouseAndKeyboardManager = new MouseAndKeyboardManagerBase(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">The main image of the current worflow process to be displayed on the PaintualCanvas within
        /// the DrawingBoard (UI)</param>
        public void SetCanvas(Engine.Surface.Canvas c)
        {
            t_canvas = c;
            CoordinatesManager.SetImageSize(c.Width, c.Height);
        }

        public void SetActivity(Engine.Tools.IGraphicActivity activity)
        {
            GraphicActivity = activity;
            GraphicActivity.Initialize(this);

            OnDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.DetachHandleEndOfProcess);

            if (GraphicActivity is Engine.Effects.Effect)
            {
                // for effects, must disable mouse input in workflow because
                // they are working on different thread and cause failure in EffectBase.Process()
                DrawingBoardMode = DrawingBoardModes.Disabled;

                // DrawingBoardMode can be re-enabled by clicking on "Draw" button in UI
                // or selecting a new drawing tool.
            }

            if (GraphicActivity is Engine.Tools.Tool)
            {
                DrawingBoardMode = DrawingBoardModes.None; // will cycle to SuspendDraw and Draw
            }

            if (GraphicActivity.HasVisualProperties)
            {
                OnPropertyPageActionRequested(WorkflowPropertyPageRequestType.ContentUpdate);
            }
            else
            {
                OnPropertyPageActionRequested(WorkflowPropertyPageRequestType.ClosePage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Called by the UI (PaintualCanvas) upon the OnRender event.</remarks>
        public System.Windows.Media.Imaging.BitmapSource GetImage()
        {
            if (CurrentEffect != null && CurrentEffect.ImageProcessed != null)
            {
                return MakeImage(CurrentEffect.ImageProcessed);
            }
            else
            {
                return MakeImage(Canvas);
            }
        }

        private System.Windows.Media.Imaging.BitmapSource MakeImage(Engine.Surface.Canvas c)
        {
            // TODO just get the updated portion of the image to be displayed here
            BitmapSource bmpSource = BitmapSource.Create(c.Width,
                c.Height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                c.Array,
                c.Stride);

            // TODO see also https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?view=netframework-4.7.2
            // where there is an example to use System.Windows.Media.Imaging.WriteableBitmap, one that is an array of int
            // that can be manipulated. web link also saved in bmp_creator/improving surface code

            return bmpSource;
        }

        /// <summary>
        /// Called by EffectBase when updating Canvas (t_imageSource in GraphicActivity) with the modified t_imageProcessed
        /// </summary>
        /// <param name="c"></param>
        /// <param name="forceRefresh"></param>
        internal void UpdateImage(Engine.Surface.Canvas c, bool forceRefresh)
        {
            SetCanvas(c);
            ChangeImage(c, forceRefresh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="forceRefresh">Forces the UI to refresh the visual</param>
        private void ChangeImage(Engine.Surface.Canvas image, bool forceRefresh)
        {
            OnDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.ViewPortSize);

            if (forceRefresh)
            {
                OnDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate);
            }
        }

        public void SaveImage(string fileName, Engine.Surface.ImageFileFormats format)
        {
            Engine.Surface.Ops.Save(Canvas, fileName, format);
        }

        /// <summary>
        /// The current version of the image.
        /// </summary>
        public Engine.Surface.Canvas Canvas { get => t_canvas; }

        public int Key { get; private set; }

        public Engine.Tools.IGraphicActivity GraphicActivity { get; private set; }

        public Engine.Effects.Effect CurrentEffect
        {
            get
            {
                if (GraphicActivity is Engine.Effects.Effect)
                {
                    return (Engine.Effects.Effect)GraphicActivity;
                }

                return null;
            }
        }

        public Engine.Tools.Tool CurrentTool
        {
            get
            {
                if (GraphicActivity is Engine.Tools.Tool)
                {
                    return (Engine.Tools.Tool)GraphicActivity;
                }

                return null;
            }
        }

        public string LastSavedFolder { get; set; }

        private void E_coordinatesManager_ZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            OnDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.Invalidate);
        }

        internal void SelectionGlassRequest(SelectionGlassRequestType type)
        {
            switch (type)
            {
                case SelectionGlassRequestType.Create:
                case SelectionGlassRequestType.Delete:
                    OnSelectionGlassRequested(type);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(String.Format("In Engine.Workflow.SelectionGlassRequest(), type '{0}' is not supported.", type));
            }
        }

        internal void SetUIAwarenessOfEnfOfProcess()
        {
            OnDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.HandleEndOfProcess);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>Called by UI (PaintualCanvas) to provide mouse actions and coordinates to the MouseAndKeyboardManager
        /// which in turn will ask CoordinatesManager to adjust coordinates according to scrolling and zooming.</remarks>
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


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Method instead of property setter, required by MouseAndKeyboardManager threading queue.</remarks>
        public void DisallowInvalidate()
        {
            AllowInvalidate = false;
        }

        public Engine.CoordinatesManager CoordinatesManager { get; private set; }

        public bool AllowInvalidate { get; internal set; }

        internal DrawingBoardModes DrawingBoardMode { get ; set; }

        internal Engine.MotionAttribute MotionAttribute
        {
            get { return t_motionAttribute; }
        }

        internal Engine.Threading.BackgroundQueue ThreadingQueue
        {
            get { return t_queue; }
        }

        public event WorkflowDrawingBoardEventHandler DrawingBoardSizeRequested;
        public event WorkflowDrawingBoardEventHandler InvalidateRequested;
        public event WorkflowSelectionGlassEventHandler SelectionGlassRequested;

        // the following not used at this time
        public event WorkflowPropertyPageEventHandler PropertyPageContentUpdateRequested;

        internal void OnDrawingBoardActionRequested(WorkflowDrawingBoardRequestType requestType)
        {
            if (requestType == WorkflowDrawingBoardRequestType.Invalidate)
            {
                InvalidateRequested?.Invoke(this, new WorkflowDrawingBoardEventArgs(requestType));
                return;
            }

            if (requestType == WorkflowDrawingBoardRequestType.ViewPortSize)
            {
                DrawingBoardSizeRequested?.Invoke(this, new WorkflowDrawingBoardEventArgs(requestType));

                return;
            }

            if (requestType == WorkflowDrawingBoardRequestType.HandleEndOfProcess)
            {
                InvalidateRequested?.Invoke(this, new WorkflowDrawingBoardEventArgs(requestType));
                return;
            }
        }

        private void OnPropertyPageActionRequested(WorkflowPropertyPageRequestType requestType)
        {
            if (requestType == WorkflowPropertyPageRequestType.ContentUpdate)
            {
                PropertyPageContentUpdateRequested?.Invoke(this, new WorkflowPropertyPageEventArgs(requestType));
            }
        }

        private void OnSelectionGlassRequested(SelectionGlassRequestType requestType)
        {
            SelectionGlassRequested?.Invoke(this, new WorkflowSelectionGlassEventArgs(requestType));
        }

        public event WorkflowClosingEventHandler Closing;

        internal void OnClosing()
        {
            // the drawing board may be null in some cases
            Closing?.Invoke(this, new EventArgs());
        }

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

                        CoordinatesManager.Dispose();
                        CoordinatesManager = null;

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

    public delegate void WorkflowClosingEventHandler(object sender, EventArgs e);

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
