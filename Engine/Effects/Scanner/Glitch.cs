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
    public class Glitch : EffectBase
    {
        private Engine.Surface.Canvas t_imagePerlin;
        private Engine.Effects.Particles.PixelParticle[] t_particles;
        private Accord.Math.Vector3[,] t_flowField;


        private int steps = 10;
        private int spread = 45;
        private double t_frequency = 0.01;
        private int t_seed = 2;
        private int t_octaves = 2;

        public Glitch()
        {
            t_visualProperties = new VisualProperties("Scanner Glitch", typeof(Glitch));
        }


        public override IGraphicActivity Duplicate(Viome w)
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
            CreatePerlinNoisePlane();
            CreateFlowField();

            t_workflow.AllowInvalidate();
            Flow();

            base.ProcessCompleted();
        }

        private void CreatePerlinNoisePlane()
        {
            Engine.Effects.Noise.IModule module = new Engine.Effects.Noise.Perlin();

            ((Perlin)module).Frequency = t_frequency;
            ((Perlin)module).NoiseQuality = NoiseQuality.Standard;
            ((Perlin)module).Seed = t_seed;
            ((Perlin)module).OctaveCount = t_octaves;
            ((Perlin)module).Lacunarity = 2.0;
            ((Perlin)module).Persistence = 0.5;

            double value = 0;

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width - 1; x++)
                {
                    value = (module.GetValue(x, y, 0) + 1) / 2.0;

                    if (value < 0) value = 0;
                    if (value > 1.0) value = 1.0;
                    byte intensity = (byte)(value * 255.0);
                    Engine.Color.Cell c = Engine.Color.Cell.ShadeOfGray(intensity);
                    Engine.Surface.Ops.SetPixel(c, t_imagePerlin, x, y);
                }
            }
        }

        private void CreateFlowField()
        {
            t_flowField = new Accord.Math.Vector3[t_imageSource.Width, t_imageSource.Height];

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    // initialize vectors with basic to-the-right direction
                    t_flowField[x, y] = new Accord.Math.Vector3(1, 0, 0);
                }
            }
        }

        private void CreateParticles()
        {
            t_particles = new Particles.PixelParticle[t_imageSource.Height];

            for (int i = 0; i < t_particles.Length; i++)
            {
                Particles.PixelParticle p = new Particles.PixelParticle(Engine.Surface.Ops.GetPixel(t_imageSource, 0, i), 0, i);
                t_particles[i] = p;
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
            for (int y = 0; y < t_imageSource.Height; y++)
            {
                t_particles[y].Draw(t_imageProcessed, alpha);
            }
        }

        private void ParticlePickColors(int x)
        {
            for (int y = 0; y < t_imageSource.Height; y++)
            {
                t_particles[y].Pixel = Engine.Surface.Ops.GetPixel(t_imageSource, x, y);
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

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                Engine.Color.Cell c = Engine.Surface.Ops.GetPixel(t_imagePerlin, x, y);
                //Engine.Color.Cell c = Engine.Surface.Ops.GetPixel(t_imageSource, x, y);
                double lum = (double)Engine.Calc.Color.Luminance(c);
                double angle = (lum * divSpread255) - divSpread2f; // reduce to range from 0 to 90 (degrees) then shift to get -45 to 45 (degrees)

                // TODO : move Engine.Effects.Noise.Math to Engine.Calc.Math
                double rad = angle * Engine.Effects.Noise.Math.DEG_TO_RAD;

                t_flowField[x, y].X = (float)System.Math.Cos(rad);
                t_flowField[x, y].Y = (float)System.Math.Sin(rad);

                t_particles[y].Move(t_flowField[x, y]);
            }
        }


        [Engine.Attributes.Meta.DisplayName("Steps")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Steps
        {
            get { return steps; }
            set { steps = value; }
        }


        [Engine.Attributes.Meta.DisplayName("Angular Spread")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Spread
        {
            get { return spread; }
            set { spread = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Seed")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Seed
        {
            get { return t_seed; }
            set { t_seed = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Frequency")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        public double Frequency
        {
            get { return t_frequency; }
            set { t_frequency = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Octaves")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Octaves
        {
            get { return t_octaves; }
            set { t_octaves = value; }
        }
    }
}
