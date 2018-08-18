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

using Engine.Tools;

namespace Engine.Effects
{
    public class VarianceGradientEffect : EffectBase
    {
        public VarianceGradientEffect()
        {
            t_visualProperties = new VisualProperties(Name, typeof(VarianceGradientEffect));
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            VarianceGradientEffect vge = new VarianceGradientEffect();
            vge.Initialize(w);

            return vge;
        }

        public override void Process()
        {
            t_imageProcessed = Engine.Surface.Ops.Copy(t_imageSource);
            t_workflow.Viome.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));
        }

        private void ThreadedProcess()
        {
            t_workflow.Viome.AllowInvalidate();

            Engine.Color.Cell c = new Engine.Color.Cell(210, 185, 65, Engine.ColorOpacity.Opaque);

            Engine.Color.ColorVariance cv = new Color.ColorVariance(c);
            cv.SetFrequencies(1000, 200, 800, 1);
            cv.SetRanges(10, 6, 20, 0);

            for (int x = 0; x < t_imageProcessed.Width; x++)
            {
                cv.Step();

                for (int y = 0; y < t_imageProcessed.Height; y++)
                {
                    int offset = t_imageProcessed.GetOffset(x, y);

                    if (y < t_imageProcessed.Height / 2)
                    {
                        Engine.Color.Cell variance = new Color.Cell(cv.Blue, cv.Green, cv.Red, cv.Alpha);
                        variance.WriteBytes(t_imageProcessed.Array, offset);
                    }
                    else
                    {
                        c.WriteBytes(t_imageProcessed.Array, offset);
                    }
                }
            }

            base.ProcessCompleted();
        }

        public override string Name { get => "Variance Gradient Effect"; }
    }
}
