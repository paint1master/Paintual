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

using Engine.Effects.Particles;

namespace Engine.Tools
{
    public class AttractorPen : Engine.Tools.Tool
    {
        private MousePoint t_previousPoint;
        private Engine.Effects.Particles.ForceParticle[] t_particles;
        private Engine.Effects.Particles.Attractor t_attractor;
        private Engine.Color.ColorVariance t_colorVariance;

        private double t_force;
        private double t_intensity;
        private double t_expression;
        private double t_maxMagnitude;

        private int t_count;
        private byte t_alpha;
        private int t_scatter;
        private bool t_useColorFromImage;

        private Utilities.Iterativ.Skipper t_skipper;

        public AttractorPen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties(Name, typeof(AttractorPen));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);
        }

        public override void BeforeDraw(MousePoint p)
        {
            // in Initialize, it's too early to get UI values (!!??!!)
            t_skipper = new Utilities.Iterativ.Skipper(6);
            t_previousPoint = new MousePoint(p.X, p.Y);

            CreateParticles(p.X, p.Y);
            t_attractor = new Effects.Particles.Attractor(new Engine.Calc.Vector(p.X, p.Y), t_force, t_expression, t_intensity);
        }

        internal override void Draw(MousePoint p)
        {
            if (t_particles.Length == 0)
            {
                return;
            }

            if (!t_skipper.Skip())
                return;

            // atractor must follow the pen between each call to draw
            int[] delta = MousePoint.Delta(t_previousPoint, p);
            Engine.Calc.Vector d = new Engine.Calc.Vector(delta[0], delta[1]);
            t_attractor.Move(d);

            if (t_imageSource.IsOutOfBounds(p.X, p.Y))
            {
                return;
            }

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            Engine.Threading.ParamList paramList = new Threading.ParamList();
            paramList.Add("x", typeof(int), p.X);
            paramList.Add("y", typeof(int), p.Y);

            loop.Loop(t_particles.Length, Threaded_Draw, paramList);
            loop.Dispose();

            t_previousPoint = p;
        }

        private int Threaded_Draw(int start, int end, Engine.Threading.ParamList paramList)
        {
            t_colorVariance.Step();

            int x = (int)paramList.Get("x").Value;
            int y = (int)paramList.Get("y").Value;

            for (int i = start; i < end; i++)
            {
                t_particles[i].Update(t_attractor);

                Engine.Color.Cell px;

                if (t_useColorFromImage)
                {
                    px = t_imageSource.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
                }
                else
                {
                    px = t_colorVariance.ColorVariation;
                }

                px.Alpha = t_alpha;
                t_particles[i].Draw(t_imageSource, px);
            }

            return 0;
        }

        private void CreateParticles(int x, int y)
        {
            t_particles = new Effects.Particles.ForceParticle[t_count];

            double variance = Engine.Calc.Math.Rand.NextDouble() + 1;

            for (int i = 0; i < t_particles.Length; i++)
            {
                t_particles[i] = new Effects.Particles.ForceParticle(x + Engine.Calc.Math.NextIntRandomInRange(t_scatter),
                    y + Engine.Calc.Math.NextIntRandomInRange(t_scatter),
                    1f + (float)variance,
                    1f + (float)variance,
                    (float)t_maxMagnitude);
            }
        }

        public override void AfterDraw(MousePoint p)
        {
            ;
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            AttractorPen ap = new AttractorPen();
            ap.Initialize(w);

            return ap;
        }

        public override string Name { get => "Attractor Pen"; }

        [Engine.Attributes.Meta.DisplayName("Particle Count")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(500)]
        public int Count
        {
            get { return t_count; }
            set { t_count = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Spread")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(200)]
        public int Scatter
        {
            get { return t_scatter; }
            set { t_scatter = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Alpha")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(200)]
        [Engine.Attributes.Meta.Range(0, 255)]
        public int Alpha
        {
            get { return t_alpha; }
            set { t_alpha = (byte)value; }
        }

        [Engine.Attributes.Meta.DisplayName("Color Variance")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.ColorVariance)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Object, typeof(Engine.Color.ColorVariance))]
        public Engine.Color.ColorVariance ColorVariance
        {
            get { return t_colorVariance; }
            set { t_colorVariance = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Color From Image")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Checkbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Boolean, typeof(bool))]
        public bool UseColorFromImage
        {
            get { return t_useColorFromImage; }
            set { t_useColorFromImage = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Force")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(1.2d)]
        public double Force
        {
            get { return t_force; }
            set { t_force = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Intensity")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(0.01d)]
        public double Intensity
        {
            get { return t_intensity; }
            set { t_intensity = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Expression")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(0.8d)]
        public double Frequency
        {
            get { return t_expression; }
            set { t_expression = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Particle Max Magnitude")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(10d)]
        public double MaxMagnitude
        {
            get { return t_maxMagnitude; }
            set { t_maxMagnitude = value; }
        }
    }
}
