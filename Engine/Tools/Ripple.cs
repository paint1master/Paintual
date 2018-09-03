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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Engine.Surface;

namespace Engine.Tools
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Inspired from this video : Coding Train, Coding Challenge 102 https://www.youtube.com/watch?v=BZUdGqeOD0w&t=923s </remarks>
    public class Ripple : Engine.Tools.Tool
    {
        private int t_steps = 5;

        private double[] sum;
        private double[] b1_1;
        private double[] b1_2;
        private double[] b1_3;
        private double[] b1_4;
         
        private double[] b2_1;

        private Engine.Effects.Particles.FluidField t_buffer_1;
        private Engine.Effects.Particles.FluidField t_buffer_2;

        private double t_dampening = 0.99d; // value between 0 and 1

        Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

        public Ripple()
        {
            t_visualProperties = new Engine.Effects.VisualProperties(Name, typeof(Ripple));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);

            // both have all values initialized at 0 by default;
            t_buffer_1 = new Engine.Effects.Particles.FluidField(t_imageSource.Width, t_imageSource.Height);
            t_buffer_2 = new Effects.Particles.FluidField(t_imageSource.Width, t_imageSource.Height);

            sum = new double[t_imageSource.Width * t_imageSource.Height];
            b1_1 = new double[t_imageSource.Width * t_imageSource.Height];
            b1_2 = new double[t_imageSource.Width * t_imageSource.Height];
            b1_3 = new double[t_imageSource.Width * t_imageSource.Height];
            b1_4 = new double[t_imageSource.Width * t_imageSource.Height];

            b2_1 = new double[t_imageSource.Width * t_imageSource.Height];
        }

        public override void BeforeDraw(MousePoint p)
        {
            ;
        }

        internal override void Draw(MousePoint p)
        {
            t_buffer_1.SetFluidAmount(5d, p.X, p.Y, PixelSetOptions.Ignore);

            int offset = 0;

            for (int i = 0; i < t_steps; i++)
            {
                offset = 0;
                for (int y = 0; y < t_imageSource.Height; y++)
                {
                    Engine.Threading.ParamList paramList = new Threading.ParamList();
                    paramList.Add("y", typeof(int), y);
                    loop.Loop(t_imageSource.Width, Threaded_Draw, paramList);
                    //Threaded_Draw(0, t_imageSource.Width, paramList);
                }

                Engine.EngineCppLibrary.CalculateRippleEffect s_calculateRippleEffect = (Engine.EngineCppLibrary.CalculateRippleEffect)Marshal.GetDelegateForFunctionPointer(
                                                        Engine.EngineCppLibrary.Pointer_calculateRippleEffect,
                                                        typeof(Engine.EngineCppLibrary.CalculateRippleEffect));

                s_calculateRippleEffect(t_imageSource.Width * t_imageSource.Height, sum, b1_1, b1_2, b1_3, b1_4, b2_1, t_dampening);

                offset = 0;
                for (int y = 0; y < t_imageSource.Height; y++)
                {
                    for (int x = 0; x < t_imageSource.Width; x++)
                    {
                        t_buffer_2.SetFluidAmount(sum[offset], x, y, PixelSetOptions.Ignore);
                        offset++;
                    }
                }

                Engine.Effects.Particles.FluidField temp = t_buffer_1;
                t_buffer_1 = t_buffer_2;
                t_buffer_2 = temp;
            }

            ShadeImage();
        }

        private int Threaded_Draw(int start, int end, Engine.Threading.ParamList paramList)
        {
            int y = (int)paramList.Get("y").Value;

            int offset = Engine.Surface.Ops.GetGridOffset(start, y, t_imageSource.Width, t_imageSource.Height);

            for (int x = start; x < end; x++)
            {
                b1_1[offset] = t_buffer_1.GetFluidAmount(x - 1, y, PixelRetrievalOptions.ReturnEdgePixel);
                b1_2[offset] = t_buffer_1.GetFluidAmount(x + 1, y, PixelRetrievalOptions.ReturnEdgePixel);
                b1_3[offset] = t_buffer_1.GetFluidAmount(x, y - 1, PixelRetrievalOptions.ReturnEdgePixel);
                b1_4[offset] = t_buffer_1.GetFluidAmount(x, y + 1, PixelRetrievalOptions.ReturnEdgePixel);
                b2_1[offset] = t_buffer_2.GetFluidAmount(x, y, PixelRetrievalOptions.ReturnEdgePixel);
                /*sum[offset] = t_buffer_1.GetFluidAmount(x - 1, y, PixelRetrievalOptions.ReturnEdgePixel) +
                    t_buffer_1.GetFluidAmount(x + 1, y, PixelRetrievalOptions.ReturnEdgePixel) +
                    t_buffer_1.GetFluidAmount(x, y - 1, PixelRetrievalOptions.ReturnEdgePixel) +
                    t_buffer_1.GetFluidAmount(x, y + 1, PixelRetrievalOptions.ReturnEdgePixel);
                sum[offset] /= 4d;
                sum[offset] -= t_buffer_2.GetFluidAmount(x, y, PixelRetrievalOptions.ReturnEdgePixel);

                t_buffer_2.SetFluidAmount(sum[offset] * t_dampening, x, y, PixelSetOptions.Ignore);*/
                offset++;
            }

            return 0;
        }

        public override void AfterDraw(MousePoint p)
        {
            ;
        }

        private void ShadeImage()
        {
            double min = 1000d;
            double max = -1000d;

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    double amount = t_buffer_2.GetFluidAmount(x, y, PixelRetrievalOptions.ReturnEdgePixel);

                    /*if (min > amount)
                    {
                        min = amount;
                    }

                    if (max < amount)
                    {
                        max = amount;
                    }*/

                    if (amount < 0.01d && amount > -0.01d )
                    {
                        continue;
                    }

                    //amount = Engine.Calc.Math.Map(amount, -3d, 3d, 0d, 5d);
                    amount *= 2d;

                    Engine.Color.Cell c = t_imageSource.GetPixel(x, y, PixelRetrievalOptions.ReturnEdgePixel);
                    //c.ChangeBrightness(amount);
                    c.Blue += (byte)amount;
                    c.Green += (byte)amount;
                    c.Red += (byte)amount;

                    t_imageSource.SetPixel(c, x, y, PixelSetOptions.Ignore);
                }
            }

            //System.Diagnostics.Debug.WriteLine(String.Format("min {0} max {1}", min, max));
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            Ripple r = new Ripple();
            r.Initialize(w);

            return r;
        }

        public override string Name { get => "Ripple"; }

        [Engine.Attributes.Meta.DisplayName("Steps")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(5)]
        public int Steps
        {
            get { return t_steps; }
            set { t_steps = value; }
        }

    }
}
