using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Effects.Noise;

namespace Engine.Tools
{
    public class ParticlePen : Engine.Tools.Tool
    {
        private Engine.Surface.Canvas t_imagePerlin;
        private Engine.Effects.Particles.Obsolete.LivingPixelParticle_O[] t_particles;
        private Accord.Math.Vector3[,] t_flowField;

        private Engine.Utilities.Iterativ.Skipper t_skipper;
        private int t_skipperValue;

        private int steps = 3;
        private double t_frequency = 0.02;
        private int t_seed = 2;
        private int t_octaves = 4;
        private double t_particleLife = 200;
        private int t_expansion = 2;
        private byte t_alpha = 100;

        public ParticlePen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties("Particle Pen", typeof(ParticlePen));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);

            t_imagePerlin = Engine.Effects.Noise.NoiseFactory.CreatePerlinNoisePlane(t_imageSource, t_frequency, t_seed, t_octaves);

            CreateFlowField();
        }

        public override void BeforeDraw(int x, int y)
        {
            // in Initialize, it's too early to get UI values (!!??!!)
            t_skipper = new Utilities.Iterativ.Skipper(t_skipperValue);
        }

        internal override void Draw(MousePoint p)
        {
            if (!t_skipper.Skip())
                return;

            Engine.Color.Cell c = Engine.Surface.Ops.GetPixel(t_imageSource, p.X, p.Y);
            CreateParticles(c, p.X, p.Y, t_alpha);

            Flow(p.X, p.Y);
        }

        public override int AfterDraw(Point p)
        {
            throw new NotImplementedException();
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            ParticlePen tlt = new ParticlePen();
            tlt.Initialize(w);

            return tlt;
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

                    double rad = angle * Engine.Calc.Math.DEG_TO_RAD;

                    t_flowField[x, y].X = (float)System.Math.Cos(rad);
                    t_flowField[x, y].Y = (float)System.Math.Sin(rad);
                }
            }
        }

        private void CreateParticles(Engine.Color.Cell c, int x, int y, byte alpha)
        {
            t_particles = new Engine.Effects.Particles.Obsolete.LivingPixelParticle_O[30];

            for (int i = 0; i < t_particles.Length; i++)
            {
                // particles are at pen location
                Engine.Effects.Particles.Obsolete.LivingPixelParticle_O p = new Engine.Effects.Particles.Obsolete.LivingPixelParticle_O(c, x, y, t_particleLife, t_expansion);
                t_particles[i] = p;
            }
        }

        private void Flow(int x, int y)
        {
            // starting with step 0 only gives transparent horizontal lines
            // because it reduces the alpha value to 0
            for (int s = 1; s <= steps; s++)
            {
                // particles need to be reset to left of image at each pass
                // otherwise they go off and only one layer of alpha particles are saved in image
                CreateParticles(Engine.Surface.Ops.GetPixel(t_imageSource, x, y), x, y, t_alpha);

                ParticleFlow();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private int ParticleFlow()
        {
            double life = 0;
            do
            {
                life = 0;

                for (int y = 0; y < t_particles.Length; y++)
                {
                    t_particles[y].Draw(t_imageSource);
                }

                // life is used to limit the number of times the colors are being updated
                int mod = (int)life;
                if (mod % 10 == 0)
                {
                    int rd = Engine.Calc.Math.Rand.Next(2);
                    rd = rd - 1; // to get positive and negative values to change the color

                    for (int y = 0; y < t_particles.Length; y++)
                    {
                        Engine.Color.Cell c = t_particles[y].Pixel;
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

                        t_particles[y].Pixel = c;
                    }
                }

                for (int y = 0; y < t_particles.Length; y++)
                {
                    if (t_particles[y].Life < 0)
                    {
                        continue;
                    }

                    Accord.Math.Vector3 pos = t_particles[y].Position;

                    if (pos.X < 0 || pos.X >= t_imageSource.Width)
                    {
                        t_particles[y].Die();
                        life += 0;
                        continue;
                    }

                    if (pos.Y < 0 || pos.Y >= t_imageSource.Height)
                    {
                        t_particles[y].Die();
                        life += 0;
                        continue;
                    }

                    t_particles[y].Move(t_flowField[(int)t_particles[y].Position.X, (int)t_particles[y].Position.Y]);
                    life += t_particles[y].Life;
                }

            } while (life > t_particleLife);

            return 0;
        }

        [Engine.Attributes.Meta.DisplayName("Skipper")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(3)]
        public int Skipper
        {
            get { return t_skipperValue; }
            set { t_skipperValue = value; }
        }
    }
}
