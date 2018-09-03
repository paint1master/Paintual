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

using Engine.Tools;
using Engine.Effects.Noise;

namespace Engine.Effects.Scanner
{
    public class Glitch : Effect
    {
        private Engine.Surface.Canvas t_imagePerlin;
        private Engine.Effects.Particles.Obsolete.PixelParticle_O[] t_particles;
        private Engine.Calc.Vector[,] t_flowField;

        private const string paramName_X = "x";
        private const string paramName_divSpread255 = "divSpread255";
        private const string paramName_divSpread2f = "divSpread2f";


        private int steps = 10;
        private int spread = 45;
        private double t_frequency = 0.01;
        private int t_seed = 2;
        private int t_octaves = 2;

        public Glitch()
        {
            t_visualProperties = new VisualProperties(Name, typeof(Glitch));
        }


        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            Glitch g = new Glitch();
            g.Initialize(w);

            return g;
        }

        public override void Process()
        {
            t_imageProcessed = Engine.Surface.Ops.Copy(t_imageSource);
            t_imagePerlin = new Engine.Surface.Canvas(t_imageSource.Width, t_imageSource.Height);

            t_workflow.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));
        }

        private void ThreadedProcess()
        {
            t_imagePerlin = Engine.Effects.Code.Noise.NoiseFactory_Static.CreatePerlinNoisePlane(t_imageSource, t_frequency, t_seed, t_octaves);
            CreateFlowField();

            t_workflow.AllowInvalidate = true;
            Flow();

            base.PostProcess();
        }

        private void CreateFlowField()
        {
            t_flowField = new Engine.Calc.Vector[t_imageSource.Width, t_imageSource.Height];

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    // initialize vectors with basic to-the-right direction
                    t_flowField[x, y] = new Engine.Calc.Vector(1, 0);
                }
            }
        }

        private void CreateParticles()
        {
            t_particles = new Particles.Obsolete.PixelParticle_O[t_imageSource.Height];

            for (int i = 0; i < t_particles.Length; i++)
            {
                //t_particles[i] = new Engine.Effects.Particles.Obsolete.PixelParticle_O(Engine.Surface.Ops.GetPixel(t_imageSource, 0, i), 0, i);
                t_particles[i] = new Engine.Effects.Particles.Obsolete.PixelParticle_O(t_imageSource.Grid.GetPixel(0, i, Surface.PixelRetrievalOptions.ReturnEdgePixel), 0, i);
            }
        }

        private void Flow()
        {
            // starting with step 0 only gives transparent horizontal lines
            // because it reduces the alpha value to 0
            for (int s = 1; s <= steps; s++)
            {
                float divSpread = spread * s / steps;

                byte alpha = (byte)(Engine.ColorOpacity.Opaque * s / steps);

                // particles need to be reset to left of image at each pass
                // otherwise they go off and only one layer of alpha particles are saved in image
                CreateParticles();

                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    ParticleDraw(alpha);
                    ParticlePickColors(x);

                    ParticleFlow(x, divSpread);
                }
            }
        }

        private void ParticleDraw(byte alpha)
        {
            for (int i = 0; i < t_particles.Length; i++)
            {
                t_particles[i].Draw(t_imageProcessed, alpha);
            }
        }

        private void ParticlePickColors(int x)
        {
            for (int i = 0; i < t_particles.Length; i++)
            {
                t_particles[i].Pixel = t_imageSource.Grid.GetPixel(x, i, Surface.PixelRetrievalOptions.ReturnEdgePixel); // Engine.Surface.Ops.GetPixel(t_imageSource, x, i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private void ParticleFlow(int x, float divSpread)
        {
            double divSpread255 = divSpread / 255;
            double divSpread2f = divSpread / 2f;

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            Engine.Threading.ParamList paramList = new Threading.ParamList();
            paramList.Add(paramName_X, typeof(int), x);
            paramList.Add(paramName_divSpread255, typeof(double), divSpread255);
            paramList.Add(paramName_divSpread2f, typeof(double), divSpread2f);

            loop.Loop(t_imageSource.Height, Threaded_ParticleFlow, paramList);
            loop.Dispose();
        }

        private int Threaded_ParticleFlow(int start, int end, Engine.Threading.ParamList paramList)
        {
            int x = (int) paramList.Get(paramName_X).Value;
            double divSpread255 = (double)paramList.Get(paramName_divSpread255).Value;
            double divSpread2f = (double)paramList.Get(paramName_divSpread2f).Value;

            for (int y = start; y < end; y++)
            {
                Engine.Color.Cell c = t_imagePerlin.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
                double lum = (double)Engine.Calc.Color.Luminance(c);
                double angle = (lum * divSpread255) - divSpread2f; // reduce to range from 0 to 90 (degrees) then shift to get -45 to 45 (degrees)

                double rad = angle * Engine.Calc.Math.DEG_TO_RAD;

                t_flowField[x, y].X = (float)System.Math.Cos(rad);
                t_flowField[x, y].Y = (float)System.Math.Sin(rad);

                t_particles[y].Move(t_flowField[x, y]);
            }

            return 0;
        }

        public override string Name { get => "Scanner Glitch"; }

        [Engine.Attributes.Meta.DisplayName("Steps")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(20)]
        public int Steps
        {
            get { return steps; }
            set { steps = value; }
        }


        [Engine.Attributes.Meta.DisplayName("Angular Spread")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(90)]
        public int Spread
        {
            get { return spread; }
            set { spread = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Seed")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(458)]
        public int Seed
        {
            get { return t_seed; }
            set { t_seed = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Frequency")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(0.05d)]
        public double Frequency
        {
            get { return t_frequency; }
            set { t_frequency = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Octaves")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(2)]
        public int Octaves
        {
            get { return t_octaves; }
            set { t_octaves = value; }
        }
    }
}
