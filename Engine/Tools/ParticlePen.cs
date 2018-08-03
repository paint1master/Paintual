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
        private Engine.Effects.Particles.LivingPixelParticle[] t_particles;
        private Accord.Math.Vector3[,] t_flowField;

        IAsyncResult[] cookies;

        private int steps = 3;
        private double t_frequency = 0.001;
        private int t_seed = 2;
        private int t_octaves = 4;
        private double t_particleLife = 30;
        private int t_expansion = 2;
        private byte t_alpha = 100;

        int t_reduceCounter = 0;
        int t_reduce = 7;

        public ParticlePen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties("Particle Pen", typeof(ParticlePen));
        }

        public override void Initialize(Viome w)
        {
            base.Initialize(w);

            t_imagePerlin = new Engine.Surface.Canvas(t_imageSource.Width, t_imageSource.Height);

            CreatePerlinNoisePlane();
            CreateFlowField();
        }

        public override void BeforeDraw(int x, int y)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(MousePoint p)
        {
            if (t_reduceCounter < t_reduce)
            {
                t_reduceCounter++;
                return;
            }

            Engine.Color.Cell c = Engine.Surface.Ops.GetPixel(t_imageSource, p.X, p.Y);
            CreateParticles(c, p.X, p.Y, t_alpha);

            Flow(p.X, p.Y);

            t_reduceCounter = 0;
        }

        public override int AfterDraw(Point p)
        {
            throw new NotImplementedException();
        }

        public override IGraphicActivity Duplicate(Viome w)
        {
            ParticlePen tlt = new ParticlePen();
            tlt.Initialize(w);

            return tlt;
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

                    double rad = angle * Engine.Calc.Math.DEG_TO_RAD;

                    t_flowField[x, y].X = (float)System.Math.Cos(rad);
                    t_flowField[x, y].Y = (float)System.Math.Sin(rad);
                }
            }
        }

        private void CreateParticles(Engine.Color.Cell c, int x, int y, byte alpha)
        {
            t_particles = new Engine.Effects.Particles.LivingPixelParticle[30];

            for (int i = 0; i < t_particles.Length; i++)
            {
                // particles are at pen location
                Engine.Effects.Particles.LivingPixelParticle p = new Engine.Effects.Particles.LivingPixelParticle(c, x, y, t_particleLife, t_expansion);
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

        private void ParticleFlow()
        {
            int height = t_particles.Length;

            int divide = Engine.Calc.Math.Division_Threading(height);

            Engine.Threading.ProcessThreading.ScannerRadial_Del_Process[] dels = new Engine.Threading.ProcessThreading.ScannerRadial_Del_Process[divide];
            cookies = new IAsyncResult[divide];

            int quarterHeight = height / divide;

            int[] ys = new int[divide];
            int[] heights = new int[divide];

            for (int i = 0; i < divide; i++)
            {
                dels[i] = ParticleFlow_Threading;
                ys[i] = (quarterHeight * i);
                heights[i] = quarterHeight * (i + 1);
            }

            int workerThreads = 0;
            int completionPortsThreads = 0;

            System.Threading.ThreadPool.GetMinThreads(out workerThreads, out completionPortsThreads);
            System.Threading.ThreadPool.SetMinThreads(divide, divide);

            for (int i = 0; i < divide; i++)
            {
                cookies[i] = dels[i].BeginInvoke(ys[i], heights[i], null, null);
            }

            for (int i = 0; i < divide; i++)
            {
                cookies[i].AsyncWaitHandle.WaitOne();
            }

            for (int i = 0; i < divide; i++)
            {
                int result = dels[i].EndInvoke(cookies[i]);
            }

            for (int i = 0; i < divide; i++)
            {
                cookies[i].AsyncWaitHandle.Close();
            }

            System.Threading.ThreadPool.SetMinThreads(workerThreads, completionPortsThreads);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private int ParticleFlow_Threading(int yStart, int yEnd)
        {
            double life = 0;
            do
            {
                life = 0;

                for (int y = yStart; y < yEnd; y++)
                {
                    t_particles[y].Draw(t_imageSource);
                }

                // life is used to limit the number of times the colors are being updated
                int mod = (int)life;
                if (mod % 10 == 0)
                {
                    int rd = Engine.Calc.Math.Rand.Next(2);
                    rd = rd - 1; // to get positive and negative values to change the color

                    for (int y = yStart; y < yEnd; y++)
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

                for (int y = yStart; y < yEnd; y++)
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
    }
}
