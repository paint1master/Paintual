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


namespace Engine
{
    public class CoordinatesManager
    {
        private double t_zoomFactor = 1d;

        /// <summary>
        /// The actual size of the image, in pixels. This value is never modified by the zoom factor.
        /// </summary>
        private Engine.Size t_imageSize = new Size(0, 0);

        /// <summary>
        /// Represents the actual image size, zoom factor independent, as a size struct suitable for WPF measurement operations.
        /// </summary>
        /// <remark>Eventually will replace <see cref="t_imageSize">t_imageSize</see> when all Paintual code 
        /// is ported to WPF.</remark>
        private System.Windows.Size t_WPFImageSize = new System.Windows.Size(0, 0);

        /// <summary>
        /// The factored size is the dimension of the image after the zoomfactor being calculated.
        /// It is a virtual size to calculate scrolling sliders position.
        /// </summary>
        private Engine.Size t_factoredSize = new Size(0, 0);

        private Size t_drawingBoardSize;

        private Engine.Point t_boardCenterPoint;

        /// <summary>
        /// A point that represents the relative position of the image in relation to its ViewPort container (the DrawingBoard).
        /// Changing the value of the Origin moves the PaintualCanvas by the same amount.
        /// </summary>
        private Engine.Point t_originPoint = new Point(0, 0);


        public CoordinatesManager()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">The new width of the ViewPort (PaintualCanvas)</param>
        /// <param name="height">The new height of the ViewPort (PaintualCanvas)</param>
        /// <remarks>Called by PaintualCanvas consequence of a request from Workflow.DrawingBoardSizeRequested event</remarks>
        public void DrawingBoardSizeChanged(int width, int height)
        {
            t_drawingBoardSize = new Engine.Size(width, height);
            t_boardCenterPoint = new Engine.Point(t_drawingBoardSize.Width / 2, t_drawingBoardSize.Height / 2);


            SetOriginToZeroIfNeeded();


            /*Engine.Calc.SizeComparison comp = Engine.Calc.Math.IsWiderOrHigher(t_imageSize.Width, t_drawingBoardSize.Width, t_imageSize.Height, t_drawingBoardSize.Height);

            if (comp == Engine.Calc.SizeComparison.IsSmaller)
            {
                t_originalImage_OriginPoint = new Point(t_boardCenterPoint.X - (t_imageSize.Width / 2), t_boardCenterPoint.Y - (t_imageSize.Height / 2));
                return;
            }*/

            // here resize image to fit the drawing board. calculate it so that there is a margin of at least 30 px around image to allow paint operations
            // on the edges
        }

        private void SetOriginToZeroIfNeeded()
        {
            bool change = false;


            if (t_factoredSize.Width > t_drawingBoardSize.Width)
            {
                if ((t_originPoint.X * -1) + t_drawingBoardSize.Width > t_factoredSize.Width)
                {
                    t_originPoint.X = t_drawingBoardSize.Width - t_factoredSize.Width;
                    change = true;
                }
            }

            if (t_factoredSize.Height > t_drawingBoardSize.Height)
            {
                if ((t_originPoint.Y * -1) + t_drawingBoardSize.Height > t_factoredSize.Height)
                {
                    t_originPoint.Y = t_drawingBoardSize.Height - t_factoredSize.Height;
                    change = true;
                }
            }

            /*if (t_factoredSize.Width < t_drawingBoardSize.Width)
            {
                //if(t_originPoint.X < 0)

                t_originPoint.X = 0;
                change = true;
            }

            if (t_factoredSize.Height < t_drawingBoardSize.Height)
            {
                t_originPoint.Y = 0;
                change = true;

            }*/

            if (change)
            {
                RaiseImagePositionChanged();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal Engine.MousePoint MousePointRelativeToImagePositionAndZoom(Engine.MousePoint e)
        {
            // values of mouse points are those relative to PaintualCanvas. If canvas moves, values of mouse points
            // are ok.
            Engine.MousePoint p = new MousePoint((int)(e.X / t_zoomFactor), (int)(e.Y / t_zoomFactor));
            return p;
        }

        private void CalculateFactoredSize()
        {
            t_factoredSize = new Size((int)(t_imageSize.Width * t_zoomFactor), (int)(t_imageSize.Height * t_zoomFactor));


            SetOriginToZeroIfNeeded();
        }

        /// <summary>
        /// Represents a region on the ViewPort (PaintualCanvas) that contains the entire image, 
        /// depending on zoom factor.
        /// </summary>
        /// <returns>A System.Windows.Rect struct.</returns>
        /// <remarks>Point is always (0, 0), and matches the upper left corner of the PaintualCanvas. To center the image or
        /// to scroll it, Margin values of the PaintualCanvas are changed</remarks>
        public Rect GetImageSizeAndPosition()
        {
            Rect r = new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(t_WPFImageSize.Width * t_zoomFactor, t_WPFImageSize.Height * t_zoomFactor));

            return r;
        }

        internal void SetImageSize(int width, int height)
        {
            t_imageSize = new Size(width, height);
            t_WPFImageSize = new System.Windows.Size(width, height);

            CalculateFactoredSize();
        }

        public void RepositionImage(double newX, double newY)
        {
            t_originPoint = new Engine.Point((int)newX, (int)newY);
        }

        /// <summary>
        /// Represents the actual size of the image in pixels. This value is never modified by the zoom factor.
        /// </summary>
        public Engine.Size ImageSize
        {
            get { return t_imageSize;}
        }

        /// <summary>
        /// The FactoredSize is the dimension of the image after the zoomfactor being calculated.
        /// It is a virtual size to calculate scrolling sliders position.
        /// </summary>
        public Size FactoredSize
        {
            get { return t_factoredSize; }
        }

        public void SetZoomFactor(double zoomFactor)
        {
            if (zoomFactor == t_zoomFactor)
            {
                return;
            }

            if (zoomFactor < 0.05f)
            {
                t_zoomFactor = 0.05f;
            }
            else if (zoomFactor > 15f)
            {
                t_zoomFactor = 15;

            }
            else
            {
                t_zoomFactor = zoomFactor;
            }

            CalculateFactoredSize();

            RaiseZoomFactorChanged();
        }

        public void ZoomIn()
        {
            SetZoomFactor(t_zoomFactor * 1.25f);
        }

        public void ZoomOut()
        {
            SetZoomFactor(t_zoomFactor / 1.25f);
        }

        /// <summary>
        /// A point that represents the relative position of the image in relation to its ViewPort container (the DrawingBoard).
        /// Changing the value of the Origin moves the PaintualCanvas by the same amount.
        /// </summary>
        public Engine.Point Origin
        {
            get { return t_originPoint; }
        }

        /// <summary>
        /// A value between 0.05 and 15
        /// </summary>
        public double ZoomFactor
        {
            get { return t_zoomFactor; }
            set
            {
                SetZoomFactor(value);
            }
        }

        #region Events

        public event ZoomFactorChangedEventHandler ZoomFactorChanged;
        public delegate void ZoomFactorChangedEventHandler(object sender, ZoomFactorChangedEventArgs e);

        public event ImagePositionChangedEventHandler ImagePositionChanged;
        public delegate void ImagePositionChangedEventHandler(object sender, ImagePositionChangedEventArgs e);

        private void RaiseZoomFactorChanged()
        {
            if (ZoomFactorChanged != null)
            {
                ZoomFactorChanged(this, new ZoomFactorChangedEventArgs(t_zoomFactor));
            }
        }

        private void RaiseImagePositionChanged()
        {
            if (ImagePositionChanged != null)
            {
                ImagePositionChanged(this, new ImagePositionChangedEventArgs(t_originPoint));
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

    public class ZoomFactorChangedEventArgs :EventArgs
    {
        public double ZoomFactor;

        public ZoomFactorChangedEventArgs(double zoomFactor)
        {
            ZoomFactor = zoomFactor;
        }
    }

    public class ImagePositionChangedEventArgs : EventArgs
    {
        public Engine.Point OriginPoint;

        public ImagePositionChangedEventArgs(Engine.Point originPoint)
        {
            OriginPoint = originPoint;
        }
    }
}
