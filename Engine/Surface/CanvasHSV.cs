﻿/**********************************************************

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

namespace Engine.Surface
{
    public class CanvasHSV
    {
        private double[] t_imageData; // because the HSV model is double
        private int t_width;
        private int t_height;
        private int t_stride;

        public CanvasHSV(int width, int height)
        {
            t_width = width;
            t_height = height;
            t_stride = (t_width * Engine.BytesPerPixel.HSV);

            t_imageData = new double[width * height * Engine.BytesPerPixel.HSV];
        }

        public CanvasHSV(Engine.Surface.Canvas canvas) : this(canvas.Width, canvas.Height)
        {
            t_imageData = Engine.Surface.Ops.BGRA_To_HSV_Array(canvas.Array);
        }

        public double[] Array
        {
            get { return t_imageData; }
        }

        public int Width
        {
            get { return t_width; }
        }

        public int Height
        {
            get { return t_height; }
        }
    }
}
