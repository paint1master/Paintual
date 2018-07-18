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
using Engine.Surface;

namespace Engine.Tools
{
    public class ThinLineTool : Engine.Tools.Tool
    {
        public ThinLineTool()
        {
            t_visualProperties = new Engine.Effects.VisualProperties("Thin Line Tool", typeof(ThinLineTool));
        }

        public override void BeforeDraw(int x, int y)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(MousePoint p)
        {
            int offset = t_imageSource.GetOffset(p.X, p.Y);

            if (offset == -1)
            { return; }

            for (int i = 0; i < 10; i++)
            {
                byte intensity = (byte)(i * 10);
                t_imageSource.Array[offset++] = intensity;
                t_imageSource.Array[offset++] = intensity;
                t_imageSource.Array[offset++] = intensity;
                t_imageSource.Array[offset++] = 255;
            }            
        }

        public override int AfterDraw(Point p)
        {
            throw new NotImplementedException();
        }

        public override IGraphicActivity Duplicate(Viome w)
        {
            ThinLineTool tlt = new ThinLineTool();
            tlt.Initialize(w);

            return tlt;
        }
    }
}
