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

        private int t_count;
        private byte t_alpha;
        private int t_scatter;

        private Utilities.Iterativ.Skipper t_skipper;

        public AttractorPen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties(Name, typeof(AttractorPen));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);
        }

        public override void BeforeDraw(int x, int y)
        {
            // in Initialize, it's too early to get UI values (!!??!!)
            t_skipper = new Utilities.Iterativ.Skipper(6);
            t_previousPoint = new MousePoint(x, y);

            CreateParticles(x, y);
            t_attractor = new Effects.Particles.Attractor(new Accord.Math.Vector3(x, y, 0), 0.000006);
        }

        internal override void Draw(MousePoint p)
        {
            if (!t_skipper.Skip())
                return;

            // atractor must follow the pen between each call to draw
            int[] delta = MousePoint.Delta(t_previousPoint, p);
            Accord.Math.Vector3 d = new Accord.Math.Vector3(delta[0], delta[1], 0);
            t_attractor.Move(d);

            int offset = t_imageSource.GetOffset(p.X, p.Y);

            if (offset == -1)
            { return; }

            Engine.Color.Cell foreground = Engine.Colors.Black;
            foreground.Alpha = t_alpha;

            foreach (Engine.Effects.Particles.ForceParticle fp in t_particles)
            {
                fp.Update(t_attractor);

                fp.Draw(t_imageSource, foreground);                
            }

            t_previousPoint = p;
        }

        private void CreateParticles(int x, int y)
        {
            t_particles = new Effects.Particles.ForceParticle[t_count];

            double variance = Engine.Calc.Math.Rand.NextDouble() + 1;

            for (int i = 0; i < t_particles.Length; i++)
            {
                t_particles[i] = new Effects.Particles.ForceParticle(x + Engine.Calc.Math.NextIntRandomInRange(t_scatter),
                    y + Engine.Calc.Math.NextIntRandomInRange(t_scatter),
                    2f + (float)variance,
                    2f + (float)variance,
                    8f);
            }
        }

        public override int AfterDraw(Point p)
        {
            throw new NotImplementedException();
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
        [Engine.Attributes.Meta.DefaultValue(200)]
        public int Count
        {
            get { return t_count; }
            set { t_count = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Spread")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(100)]
        public int Scatter
        {
            get { return t_scatter; }
            set { t_scatter = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Alpha")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(30)]
        [Engine.Attributes.Meta.Range(0, 255)]
        public int Alpha
        {
            get { return t_alpha; }
            set { t_alpha = (byte)value; }
        }
    }
}
