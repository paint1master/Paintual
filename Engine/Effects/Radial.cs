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
    public class Radial : Effect
    {
        private Engine.Surface.Canvas t_imagePerlin;
        private Engine.Effects.Particles.Obsolete.LivingPixelParticle_O[] t_particles;
        private Engine.Calc.Vector[,] t_flowField;
        private int t_particleCount = 200;

        private int steps = 10;
        private double t_frequency = 0.01;
        private int t_seed = 2;
        private int t_octaves = 2;
        private double t_particleLife = 100;
        private int t_expansion = 2;

        public Radial()
        {
            t_visualProperties = new VisualProperties(Name, typeof(Radial));
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            Radial r = new Radial();
            r.Initialize(w);

            return r;
        }

        public override void Process()
        {
            t_imageProcessed = Engine.Surface.Ops.Copy(t_imageSource);
            t_imagePerlin = new Engine.Surface.Canvas(t_imageSource.Width, t_imageSource.Height);

            t_workflow.Viome.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));
        }

        private void ThreadedProcess()
        {
            t_imagePerlin = Engine.Effects.Code.Noise.NoiseFactory_Static.CreatePerlinNoisePlane(t_imageSource, t_frequency, t_seed, t_octaves);
            CreateFlowField();

            // call this as late as possible, just before actual effect processing
            t_workflow.Viome.AllowInvalidate();
            Flow();

            base.ProcessCompleted();
        }

        private void CreateFlowField()
        {
            t_flowField = new Engine.Calc.Vector[t_imageSource.Width, t_imageSource.Height];


            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            Engine.Threading.ParamList paramList = new Threading.ParamList();

            loop.Loop(t_imageSource.Height, Threaded_CreateFlowField, paramList);
            loop.Dispose();
        }

        private int Threaded_CreateFlowField(int start, int end, Engine.Threading.ParamList paramList)
        {
            for (int y = start; y < end; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    // initialize vectors with basic to-the-right direction
                    t_flowField[x, y] = new Engine.Calc.Vector(1, 0);

                    Engine.Color.Cell c = t_imagePerlin.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
                    double lum = (double)Engine.Calc.Color.Luminance(c);
                    double angle = (lum * 360 / 255);

                    double rad = angle * Engine.Calc.Math.DEG_TO_RAD;

                    t_flowField[x, y].X = (float)System.Math.Cos(rad);
                    t_flowField[x, y].Y = (float)System.Math.Sin(rad);
                }
            }

            return 0;
        }

        private void CreateParticles(byte alpha)
        {
            //Engine.Color.Cell color = new Engine.Color.Cell(200, 220, 230, alpha); beige
            Engine.Color.Cell color = Engine.Application.UISelectedValues.SelectedColor;
            color.Alpha = alpha;

            t_particles = new Particles.Obsolete.LivingPixelParticle_O[t_particleCount];

            for (int i = 0; i < t_particles.Length; i++)
            {
                // particles are centered on screen
                Particles.Obsolete.LivingPixelParticle_O p = new Particles.Obsolete.LivingPixelParticle_O(color, t_imageSource.Width / 2, t_imageSource.Height / 2, t_particleLife, t_expansion);
                t_particles[i] = p;
            }
        }

        private void Flow()
        {
            // starting with step 0 only gives transparent horizontal lines
            // because it reduces the alpha value to 0
            for (int s = 1; s <= steps; s++)
            {
                byte alpha = (byte)(Engine.ColorOpacity.Opaque * s / steps);
                //byte alpha = 127;

                // particles need to be reset to left of image at each pass
                // otherwise they go off and only one layer of alpha particles are saved in image
                CreateParticles(alpha);

                
                Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();
                loop.Loop(t_particles.Length, ParticleFlow, null);
                loop.Dispose();
                // for some reason, last image refresh does not always work
                // bug exists only when threading Flow()
                System.Threading.Thread.Sleep(200);
                
                //ParticleFlow(0, t_particles.Length, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private int ParticleFlow(int start, int end, Engine.Threading.ParamList lst)
        {
            Engine.Color.ColorVariance cv = new Color.ColorVariance(t_particles[0].Pixel);
            cv.SetFrequencies(500);
            cv.SetRanges(30);

            double life = 0;

            do
            {
                life = 0;

                for (int i = start; i < end; i++)
                {
                    t_particles[i].Pixel = cv.ColorVariation;
                    t_particles[i].Draw(t_imageProcessed);

                    if (t_particles[i].Life < 0)
                    {
                        continue;
                    }

                    Engine.Calc.Vector pos = t_particles[i].Position;

                    if (pos.X < 0 || pos.X >= t_imageProcessed.Width)
                    {
                        t_particles[i].Die();
                        continue;
                    }

                    if (pos.Y < 0 || pos.Y >= t_imageProcessed.Height)
                    {
                        t_particles[i].Die();
                        continue;
                    }

                    t_particles[i].Move(t_flowField[(int)t_particles[i].Position.X, (int)t_particles[i].Position.Y]);
                    life += t_particles[i].Life;
                }

            } while (life > t_particleLife);

            return 0;
        }

        public override string Name { get => "Radial"; }

        [Engine.Attributes.Meta.DisplayName("Steps")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(50)]
        public int Steps
        {
            get { return steps; }
            set { steps = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Seed")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(8874)]
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
        [Engine.Attributes.Meta.DefaultValue(3)]
        public int Octaves
        {
            get { return t_octaves; }
            set { t_octaves = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Life")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(400d)]
        public double Life
        {
            get { return t_particleLife; }
            set { t_particleLife = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Expansion")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(4)]
        public int Expansion
        {
            get { return t_expansion; }
            set { t_expansion = value; }
        }
    }
}
