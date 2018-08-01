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

namespace Engine.Effects.Particles
{
    public class PixelParticle : BaseParticle
    {
        protected Engine.Color.Cell t_cell; // the pixel;

        public PixelParticle() : base()
        {
            
        }

        public PixelParticle(Engine.Color.Cell c)
        {
            t_cell = c;
        }

        public PixelParticle(Engine.Color.Cell c, int x, int y) : this(c)
        {
            t_position = new Accord.Math.Vector3((float)x, (float)y, 0);
        }

        public void Draw(Engine.Surface.Canvas c, byte alpha)
        {
            Engine.Color.Cell source = Engine.Surface.Ops.GetPixel(c, (int)t_position.X, (int)t_position.Y);
            t_cell.Alpha = alpha;
            Engine.Color.Cell dest = Engine.Calc.Color.FastAlphaBlend(t_cell, source);
            Engine.Surface.Ops.SetPixel(dest, c, (int)t_position.X, (int)t_position.Y);
        }

        public Engine.Color.Cell Pixel
        {
            get { return t_cell; }
            set { t_cell = value; }
        }
    }
}
