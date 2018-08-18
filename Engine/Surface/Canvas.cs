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

using ImageMagick;

namespace Engine.Surface
{
    public class Canvas
    {
        private ImageData t_imageData;
        private ImageDataGrid t_grid;

        private int t_width;
        private int t_height;
        private int t_stride;      

        public Canvas(int width, int height)
        {
            t_width = width;
            t_height = height;
            t_stride = (t_width * Engine.BytesPerPixel.BGRA);

            t_imageData = new ImageData(width, height);
            t_grid = new ImageDataGrid(ref t_imageData);
        }

        public Canvas(byte[] imageData, int width, int height) : this(width, height)
        {
            t_imageData = new ImageData(imageData, width, height);
            t_grid = new ImageDataGrid(ref t_imageData);
        }

        public Canvas(int width, int height, Engine.Color.Cell c) : this(width, height)
        {
            Engine.Surface.Ops.FillWithColor(this.Array, c);
        }

        public Canvas(string fileName)
        {
            using (MagickImage image = new MagickImage(fileName))
            {
                t_width = image.Width;
                t_height = image.Height;
                t_stride = (t_width * Engine.BytesPerPixel.BGRA);

                t_imageData = new ImageData(image.Width, image.Height);
                t_imageData.Array = image.ToByteArray(MagickFormat.Bgra);
                t_grid = new ImageDataGrid(ref t_imageData);
            }
        }

        /// <summary>
        /// Returns the first byte location of a four-byte pixel info within an array
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1 if it is out of range (ie either x or y are &lt; 0 or the resulting offset is beyond the length of imageData</returns>
        public int GetOffset(int x, int y)
        {
            int result = ((t_width * y) + (x)) * Engine.BytesPerPixel.BGRA;

            if (result >= Array.Length || result < 0)
            {
                return -1;
            }

            return result;
        }

        public Engine.Color.Cell GetPixel(int x, int y, Engine.Surface.PixelRetrievalOptions option)
        {
            return Grid.GetPixel(x, y, option);
        }

        public void SetPixel(Engine.Color.Cell c, int x, int y, Engine.Surface.PixelSetOptions option)
        {
            Grid.SetPixel(c, x, y, option);
        }

        public bool IsOutOfBounds(int x, int y)
        {
            return Grid.IsOutOfBounds(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Was .ImageData</remarks>
        public byte[] Array
        {
            get { return t_imageData.Array; }
        }

        public ImageDataGrid Grid { get => t_grid; }

        public int Width
        {
            get { return t_width; }
        }

        public int Height
        {
            get { return t_height; }
        }

        public int Stride
        {
            get { return t_stride; }
        }
    }
}
