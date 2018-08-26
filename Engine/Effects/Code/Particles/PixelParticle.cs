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

namespace Engine.Effects.Particles.Obsolete
{
    [Obsolete("Kept for compatibility with Glitch, Radial effects and ThinLineTool and ParticlePen tools.")]
    public class PixelParticle_O : BaseParticle_O
    {
        protected Engine.Color.Cell t_cell; // the pixel;

        public PixelParticle_O() : base()
        {
            
        }

        public PixelParticle_O(Engine.Color.Cell c)
        {
            t_cell = c;
        }

        public PixelParticle_O(Engine.Color.Cell c, int x, int y) : this(c)
        {
            t_position = new Engine.Calc.Vector((float)x, (float)y);
        }

        public PixelParticle_O(Engine.Color.Cell c, float x, float y) : this(c)
        {
            t_position = new Engine.Calc.Vector(x, y);
        }

        public void Draw(Engine.Surface.Canvas c, byte alpha)
        {
            Engine.Color.Cell source = c.GetPixel((int)t_position.X, (int)t_position.Y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
            t_cell.Alpha = alpha;
            c.SetPixel(Engine.Calc.Color.FastAlphaBlend(t_cell, source), (int)t_position.X, (int)t_position.Y, Surface.PixelSetOptions.Ignore);
        }

        public void Draw(Engine.Surface.Canvas c, Engine.Color.Cell color)
        {
            Engine.Color.Cell source = c.Grid.GetPixel((int)t_position.X, (int)t_position.Y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
            c.SetPixel(Engine.Calc.Color.FastAlphaBlend(color, source), (int)t_position.X, (int)t_position.Y, Surface.PixelSetOptions.Ignore);
        }

        public Engine.Color.Cell Pixel
        {
            get { return t_cell; }
            set { t_cell = value; }
        }
    }
}
