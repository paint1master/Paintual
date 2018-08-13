using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Engine
{
    public class Workflow : IDisposable
    {
        private int t_key;
        private Engine.Viome t_viome;

        /// <summary>
        /// The current version of the image.
        /// </summary>
        private Engine.Surface.Canvas t_currentCanvas;


        internal Workflow(int key)
        {
            t_key = key;
            t_currentCanvas = new Engine.Surface.Canvas(1200, 1200, Engine.Color.Cell.ShadeOfGray(255));
            t_viome = Engine.Application.Viomes.NewViome(this);
        }

        internal Workflow(int key, string fileName)
        {
            t_key = key;
            t_currentCanvas = new Surface.Canvas(fileName);
            t_viome = Engine.Application.Viomes.NewViome(this);
        }

        public Engine.Viome Viome { get => t_viome; }

        public void SetActivity(Engine.Tools.IGraphicActivity activity)
        {
            GraphicActivity = activity;
            GraphicActivity.Initialize(this);

            t_viome.ChangeActivity(activity);
        }

        #region Image methods
        public void SetImage(Engine.Surface.Canvas c)
        {
            t_currentCanvas = c;
            t_viome.ChangeImage(c, true);
        }

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
            // that can be manipulated. web link also savec in bmp_creator/improving surface code

            return bmpSource;
        }

        public void SetImage(Engine.Surface.Canvas c, bool forceRefresh)
        {
            t_currentCanvas = c;
            t_viome.ChangeImage(c, forceRefresh);
        }

        public void SaveImage(string fileName, Engine.Surface.ImageFileFormats format)
        {
            Engine.Surface.Ops.Save(t_currentCanvas, fileName, format);
        }

        #endregion //Image methods

        #region Properties
        public Engine.Surface.Canvas Canvas { get => t_currentCanvas; }

        public int Key { get => t_key; }

        public Engine.Tools.IGraphicActivity GraphicActivity { get; private set; }

        public Engine.Effects.EffectBase CurrentEffect
        {
            get
            {
                if (GraphicActivity is Engine.Effects.EffectBase)
                {
                    return (Engine.Effects.EffectBase)GraphicActivity;
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



        #endregion

        public delegate void WorkflowClosingEventHandler(object sender, EventArgs e);

        public event WorkflowClosingEventHandler Closing;

        public void RaiseClosing()
        {
            if (Closing != null)
            {
                // the drawing board may be null in some cases
                Closing(this, new EventArgs());
            }
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
                        t_viome = null;
                        t_currentCanvas = null;
                        /*t_activity = null;

                        t_coordinatesManager.Dispose();
                        t_coordinatesManager = null;

                        t_queue = null;

                        t_mouseAndKeyboardManager = null;*/

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
}
