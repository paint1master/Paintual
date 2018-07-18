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

namespace Engine.Surface
{
    public class ColorPickerPlane
    {
        private Engine.Surface.Canvas t_plane;

        public ColorPickerPlane(int width, int height)
        {
            t_plane = new Engine.Surface.Canvas(width, height);
        }

        public void SetColors(Engine.Color.Cell upperLeft, Engine.Color.Cell upperRight, Engine.Color.Cell lowerLeft, Engine.Color.Cell lowerRight)
        {
            int[,] plane = Engine.Calc.Color.GenerateQuadGradient(upperLeft, upperRight, lowerLeft, lowerRight, Width, Height);

            t_plane = Engine.Surface.Ops.ToCanvas(plane, Width, Height);
        }

        public Engine.Surface.Canvas Canvas
        {
            get { return t_plane; }
        }

        public int Width
        {
            get { return t_plane.Width; }
        }

        public int Height
        {
            get { return t_plane.Height; }
        }
    }
}
