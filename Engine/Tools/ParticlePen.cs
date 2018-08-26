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
        private Engine.Calc.Vector[,] t_flowField;

        private Engine.Utilities.Iterativ.Skipper t_skipper;
        private int t_skipperValue;

        private int steps = 3;
        private double t_frequency = 0.02;
        private int t_seed = 2;
        private int t_octaves = 4;
        private double t_particleLife = 200;
        private int t_expansion = 2;
        private byte t_alpha = 50;

        public ParticlePen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties("Particle Pen", typeof(ParticlePen));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);

            t_imagePerlin = Engine.Effects.Code.Noise.NoiseFactory_Static.CreatePerlinNoisePlane(t_imageSource, t_frequency, t_seed, t_octaves);

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

            Engine.Color.Cell c = Engine.Application.UISelectedValues.SelectedColor; //Engine.Surface.Ops.GetPixel(t_imageSource, p.X, p.Y);
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

        private void CreateParticles(Engine.Color.Cell c, int x, int y, byte alpha)
        {
            c.Alpha = alpha;

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
                CreateParticles(Engine.Application.UISelectedValues.SelectedColor, x, y, t_alpha);

                ParticleFlow();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private int ParticleFlow()
        {
            Engine.Color.ColorVariance cv = new Color.ColorVariance(t_particles[0].Pixel);
            cv.SetFrequencies(500);
            cv.SetRanges(30);

            double life = 0;
            do
            {
                cv.Step();

                life = 0;

                for (int y = 0; y < t_particles.Length; y++)
                {
                    t_particles[y].Pixel = cv.ColorVariation;
                    t_particles[y].Draw(t_imageSource);

                    if (t_particles[y].Life < 0)
                    {
                        continue;
                    }

                    Engine.Calc.Vector pos = t_particles[y].Position;

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
        [Engine.Attributes.Meta.DefaultValue(15)]
        public int Skipper
        {
            get { return t_skipperValue; }
            set { t_skipperValue = value; }
        }
    }
}
