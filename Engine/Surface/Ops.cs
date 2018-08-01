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
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageMagick;

namespace Engine.Surface
{
    public enum ImageFileFormats
    {
        Undefined,
        PNG
    }

    public class Ops
    {
        public static double[] BGRA_To_HSV_Array(byte[] imageData)
        {
            int cells = imageData.Length / Engine.BytesPerPixel.BGRA;

            double[] hsv = new double[cells * Engine.BytesPerPixel.HSV];

            int offset = 0;
            int hsvOffset = 0;

            for (int i = 0; i < cells; i++)
            {
                Engine.Color.Models.HSV h = Engine.Color.Models.HSV.FromRGB(new Engine.Color.Models.RGBdouble(new Engine.Color.Cell(imageData, offset)));
                offset += Engine.BytesPerPixel.BGRA;

                hsv[hsvOffset++] = h.H;
                hsv[hsvOffset++] = h.S;
                hsv[hsvOffset++] = h.V;
            }

            return hsv;
        }

        public static unsafe Engine.Surface.Canvas Copy(Engine.Surface.Canvas source)
        {
            Engine.Surface.Canvas copy = new Canvas(source.Width, source.Height);

            fixed (byte* pSource = source.Array, pCopy = copy.Array)
            {
                byte* ps = pSource;
                byte* pc = pCopy;

                for (int i = 0; i < source.Array.Length; i++)
                {
                    *pc = *ps;
                    pc++;
                    ps++;
                }
            }

            return copy;
        }

        public unsafe static void FillWithColor(byte[] array, Engine.Color.Cell c)
        {
            fixed (byte* pCanvas = array)
            {
                byte* pc = pCanvas;

                for (int i = 0; i < array.Length; i += Engine.BytesPerPixel.BGRA)
                {
                    *pc = c.Blue; pc++;
                    *pc = c.Green; pc++;
                    *pc = c.Red; pc++;
                    *pc = c.Alpha; pc++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <remarks>If X and Y are outside the boundaries of the image, it returns a black opaque pixel</remarks>
        public static Engine.Color.Cell GetPixel(Engine.Surface.Canvas c, int x, int y)
        {
            int offset = c.GetOffset(x, y);

            if (offset == -1)
            {
                // out of range
                return new Engine.Color.Cell(0, 0, 0, Engine.ColorOpacity.Opaque);
            }

            Engine.Color.Cell pix = new Color.Cell(c.Array, offset);
            return pix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="error">An out boolean parameter that tells if the calculated offset was out of range.</param>
        /// <returns></returns>
        /// <remarks>If X and Y are outside the boundaries of the image, it returns a black opaque pixel</remarks>
        public static Engine.Color.Cell GetPixel(Engine.Surface.Canvas c, int x, int y, out bool error)
        {
            int offset = c.GetOffset(x, y);

            if (offset == -1)
            {
                // out of range
                error = true;
                return new Engine.Color.Cell(0, 0, 0, Engine.ColorOpacity.Opaque);
            }

            Engine.Color.Cell pix = new Color.Cell(c.Array, offset);
            error = false;
            return pix;
        }

        public static void SetPixel(Engine.Color.Cell c, Engine.Surface.Canvas canvas, int x, int y)
        {
            // TODO: make bounds checks more performant and less repetitive
            if (x < 0 || y < 0 || x >= canvas.Width || y >= canvas.Height)
            {
                return;
            }

            int offset = canvas.GetOffset(x, y);
            c.WriteBytes(canvas.Array, offset);
        }

        public static void SetPixel(Engine.Color.Cell c, byte alpha, Engine.Surface.Canvas canvas, int x, int y)
        {
            // TODO: make bounds checks more performant and less repetitive
            if (x < 0 || y < 0 || x >= canvas.Width || y >= canvas.Height)
            {
                return;
            }

            int offset = canvas.GetOffset(x, y);
            c.WriteBytesWithAlpha(canvas.Array, alpha, offset);
        }

        public static Engine.Surface.Canvas ToCanvas(int[,] cellGrid, int width, int height)
        {
            // TODO : needs optimization
            int[] imageData = ToArgbIntArray(cellGrid, width, height);
            byte[] bytes = IntArrayToByteArray(imageData);

            Engine.Surface.Canvas bi = new Canvas(bytes, width, height);

            return bi;
        }

        public static int[] ToArgbIntArray(int[,] cellGrid, int width, int height)
        {
            int[] imageData = new int[cellGrid.Length];

            int offset = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    new Engine.Color.Cell(cellGrid[x, y]).WriteInt(imageData, offset);
                    offset++;
                }
            }

            return imageData;
        }

        public static byte[] IntArrayToByteArray(int[] array)
        {
            byte[] bytes = new byte[array.Length * Engine.BytesPerPixel.BGRA];

            int offset = 0;

            for (int i = 0; i < array.Length; i++)
            {
                new Engine.Color.Cell(array[i]).WriteBytes(bytes, offset);
                offset += Engine.BytesPerPixel.BGRA;
            }

            return bytes;
        }

        public static void Save(Engine.Surface.Canvas canvas, string fileName, Engine.Surface.ImageFileFormats format)
        {
            if (format == ImageFileFormats.Undefined)
            {
                throw new ArgumentOutOfRangeException(String.Format("In Engine.Surface.Ops.Save(), the '{0}' is not supported.", format.ToString()));
            }

            using (MagickImage image = new MagickImage(Engine.Surface.Ops.ToBitmap(canvas)))
            {
                image.Format = MagickFormat.Png;
                image.Write(fileName);
            }
        }

        public static Bitmap ToBitmap(Engine.Surface.Canvas canvas)
        {
            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, canvas.Width, canvas.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Copy the BGRA values into the array.
                System.Runtime.InteropServices.Marshal.Copy(canvas.Array, 0, ptr, canvas.Array.Length);

                // Unlock the bits. VERY IMPORTANT
                bmp.UnlockBits(bmpData);
            }
            catch (AccessViolationException e)
            {
                MessageBox.Show(e.Message + "\\nAn empty image is provided instead.");
            }
            finally
            {
                ;
            }

            return bmp;
        }

        public static Engine.Surface.Canvas CopyFromImage(Engine.Surface.Canvas source, Engine.Rectangle region)
        {
            // if rectangle goes outside of the boundaries of the source, some pixels will not be set,
            // therefore fill the result first to ensure complete byte data
            Engine.Surface.Canvas dest = new Canvas(region.Size.Width, region.Size.Height, Engine.Colors.Teal);

            // calc origin of new image on source to start copy
            // originX originY are coords of source image
            // destX and destY are coords of new image
            int originX = 0;
            int destX = 0;
            int width = 0;

            int originY = 0;
            int destY = 0;
            int height = 0;

            //.....................
            if (region.Point.X < 0)
            {
                originX = 0;
                destX = Math.Abs(region.Point.X);
                width = dest.Width - destX;
            }
            else if (dest.Width + region.Point.X > source.Width)
            {
                originX = region.Point.X;
                destX = 0;
                width = source.Width - region.Point.X;
            }
            else
            {
                originX = region.Point.X;
                width = dest.Width;
            }

            //.....................
            if (region.Point.Y < 0)
            {
                originY = 0;
                destY = Math.Abs(region.Point.Y);
                height = dest.Height - destY;
            }
            else if (dest.Height + region.Point.Y > source.Height)
            {
                originY = region.Point.Y;
                destY = 0;
                height = source.Height - region.Point.Y;
            }
            else
            {
                originY = region.Point.Y;
                height = dest.Height;
            }

            del_CopyFromImage a = thread_CopyFromImage;
            del_CopyFromImage b = thread_CopyFromImage;
            del_CopyFromImage c = thread_CopyFromImage;
            del_CopyFromImage d = thread_CopyFromImage;

            int quarterHeight = height / 4;
            int destY_a = destY, height_0 = quarterHeight;
            int destY_b = quarterHeight, height_1 = (quarterHeight * 2);
            int destY_c = (quarterHeight * 2), height_2 = (quarterHeight * 3);
            int destY_d = (quarterHeight * 3), height_3 = height;


            IAsyncResult cookie_a = a.BeginInvoke(source, dest, originX, originY, destX, destY_a, width, height_0, null, null);
            IAsyncResult cookie_b = b.BeginInvoke(source, dest, originX, originY, destX, destY_b, width, height_1, null, null);
            IAsyncResult cookie_c = c.BeginInvoke(source, dest, originX, originY, destX, destY_c, width, height_2, null, null);
            IAsyncResult cookie_d = d.BeginInvoke(source, dest, originX, originY, destX, destY_d, width, height_3, null, null);

            a.EndInvoke(cookie_a);
            b.EndInvoke(cookie_b);
            c.EndInvoke(cookie_c);
            d.EndInvoke(cookie_d);

            return dest;
        }

        private delegate void del_CopyFromImage(Engine.Surface.Canvas source, Engine.Surface.Canvas dest, int originX, int originY, int destX, int destY, int width, int height);

        private static void thread_CopyFromImage(Engine.Surface.Canvas source, Engine.Surface.Canvas dest, int originX, int originY, int destX, int destY, int width, int height)
        {
            for (int y = destY; y < height; y++)
            {
                int offset = source.GetOffset(originX, originY + y);
                int offsetNewImage = dest.GetOffset(destX, y);
                for (int x = destX; x < width; x++)
                {
                    Engine.Color.Cell c = new Color.Cell(source.Array, offset);
                    c.WriteBytes(dest.Array, offsetNewImage);
                    offset += Engine.BytesPerPixel.BGRA;
                    offsetNewImage += Engine.BytesPerPixel.BGRA;
                }
            }
        }
    }
}
