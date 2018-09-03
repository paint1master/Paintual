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
    public class VarianceGradientEffect : Effect
    {
        private Engine.Color.ColorVariance t_colorVariance;
        private int t_percent = 75;

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
            t_workflow.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));
        }

        private void ThreadedProcess()
        {
            t_workflow.AllowInvalidate = true;

            Engine.Color.Cell c = Engine.Application.UISelectedValues.SelectedColor;
            t_colorVariance.SetColor(c);

            for (int x = 0; x < t_imageProcessed.Width; x++)
            {
                t_colorVariance.Step();

                for (int y = 0; y < t_imageProcessed.Height; y++)
                {
                    int offset = t_imageProcessed.GetOffset(x, y);

                    if (y < t_imageProcessed.Height * t_percent / 100)
                    {
                        Engine.Color.Cell variance = t_colorVariance.ColorVariation;
                        variance.WriteBytes(t_imageProcessed.Array, offset);
                    }
                    else
                    {
                        c.WriteBytes(t_imageProcessed.Array, offset);
                    }
                }
            }

            base.PostProcess();
        }

        public override string Name { get => "Variance Gradient Effect"; }

        [Engine.Attributes.Meta.DisplayName("Color Variance")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.ColorVariance)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Object, typeof(Engine.Color.ColorVariance))]
        public Engine.Color.ColorVariance ColorVariance
        {
            get { return t_colorVariance; }
            set { t_colorVariance = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Percent")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(75)]
        [Engine.Attributes.Meta.Range(0, 100)]
        public int Percent
        {
            get { return t_percent; }
            set { t_percent = value; }
        }
    }
}
