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
    public class Radial :EffectBase
    {
        private Engine.Surface.Canvas t_imagePerlin;
        private Engine.Effects.Particles.LivingPixelParticle[] t_particles;
        private Accord.Math.Vector3[,] t_flowField;

        private int steps = 10;
        private double t_frequency = 0.01;
        private int t_seed = 2;
        private int t_octaves = 2;
        private double t_particleLife = 100;
        private int t_expansion = 2;

        public Radial()
        {
            t_visualProperties = new VisualProperties("Scanner Radial", typeof(Radial));
        }

        public override IGraphicActivity Duplicate(Viome w)
        {
            Radial r = new Radial();
            r.Initialize(w);

            return r;
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

                    Engine.Color.Cell c = Engine.Surface.Ops.GetPixel(t_imagePerlin, x, y);
                    double lum = (double)Engine.Calc.Color.Luminance(c);
                    double angle = (lum * 360 / 255);

                    // TODO : move Engine.Effects.Noise.Math to Engine.Calc.Math
                    double rad = angle * Engine.Effects.Noise.Math.DEG_TO_RAD;

                    t_flowField[x, y].X = (float)System.Math.Cos(rad);
                    t_flowField[x, y].Y = (float)System.Math.Sin(rad);
                }
            }
        }

        private void CreateParticles(byte alpha)
        {
            Engine.Color.Cell color = new Engine.Color.Cell(200, 220, 230, alpha);

            t_particles = new Particles.LivingPixelParticle[t_imageSource.Height];
            //t_particles = new Particles.LivingPixelParticle[5];

            for (int i = 0; i < t_particles.Length; i++)
            {
                // particles are centered on screen
                Particles.LivingPixelParticle p = new Particles.LivingPixelParticle(color, t_imageSource.Width / 2, t_imageSource.Height / 2, t_particleLife, t_expansion);
                t_particles[i] = p;

            }
        }

        private void Flow()
        {
            // starting with step 0 only gives transparent horizontal lines
            // because it reduces the alpha value to 0
            for (int s = 1; s <= steps; s++)
            {
                //byte alpha = (byte)(Engine.ColorOpacity.Opaque * s / steps);
                byte alpha = 127;

                // particles need to be reset to left of image at each pass
                // otherwise they go off and only one layer of alpha particles are saved in image
                CreateParticles(alpha);

                double life = 0;

                do
                {
                    life = 0;
                    ParticleDraw();
                    ParticleUpdateColors(ref life);
                    ParticleFlow(ref life);
                } while (life > t_particleLife); // instead of 0 because it takes too long to kill all particles.
            }
        }

        private void ParticleDraw()
        {
            for (int y = 0; y < t_particles.Length; y++)
            {
                t_particles[y].Draw(t_imageProcessed);
            }
        }

        private void ParticleUpdateColors(ref double life)
        {
            // life is used to limit the number of times the colors are being updated
            int mod = (int)life;
            if (mod % 10 != 0)
            {
                return;
            }

            int rd = Engine.Calc.Math.Rand.Next(2);
            rd = rd - 1; // to get positive and negative values to change the color

            foreach(var p in t_particles)
            {
                Engine.Color.Cell c = p.Pixel;
                int b = (int)c.Blue;
                b += rd;

                if (b < 0)
                    b = 0;

                if (b > 255)
                    b = 255;

                int g = (int)c.Green;
                g += rd;

                if (g < 0)
                    g = 0;

                if (g > 255)
                    g = 255;

                int r = (int)c.Red;
                r += rd;

                if (r < 0)
                    r = 0;

                if (r > 255)
                    r = 255;

                c.Blue = (byte)b;
                c.Green = (byte)g;
                c.Red = (byte)r;

                p.Pixel = c;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private void ParticleFlow(ref double life)
        {
            foreach (var p in t_particles)
            {
                if (p.Life < 0)
                {
                    continue;
                }

                Accord.Math.Vector3 pos = p.Position;

                if (pos.X < 0 || pos.X >= t_imageProcessed.Width)
                {
                    p.Die();
                    life += 0;
                    continue;
                }

                if (pos.Y < 0 || pos.Y >= t_imageProcessed.Height)
                {
                    p.Die();
                    life += 0;
                    continue;
                }

                p.Move(t_flowField[(int)p.Position.X, (int)p.Position.Y]);
                life += p.Life;
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

        [Engine.Attributes.Meta.DisplayName("Life")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        public double Life
        {
            get { return t_particleLife; }
            set { t_particleLife = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Expansion")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Expansion
        {
            get { return t_expansion; }
            set { t_expansion = value; }
        }
    }
}
