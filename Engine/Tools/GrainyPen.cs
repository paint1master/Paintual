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
using Engine.Surface;

namespace Engine.Tools
{
    public class GrainyPen : Engine.Tools.Tool
    {
        private Engine.Effects.Particles.Obsolete.PixelParticle_O[] t_particles;
        private int t_count;
        private byte t_alpha;
        private double t_scatter;
        private int t_skipperValue;

        private Engine.Utilities.Iterativ.Skipper t_skipper;

        private MousePoint t_previousPoint;

        public GrainyPen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties(Name, typeof(GrainyPen));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);
        }

        public override void BeforeDraw(int x, int y)
        {
            // in Initialize, it's too early to get UI values (!!??!!)
            t_skipper = new Utilities.Iterativ.Skipper(t_skipperValue);
            t_previousPoint = new MousePoint(x, y);

            CreateParticles(x, y);
        }

        internal override void Draw(MousePoint p)
        {
            if (!t_skipper.Skip())
                return;

            int offset = t_imageSource.GetOffset(p.X, p.Y);

            if (offset == -1)
            { return; }

            // particles must follow the pen between each call to draw
            int[] delta = MousePoint.Delta(t_previousPoint, p);
            Accord.Math.Vector3 d = new Accord.Math.Vector3(delta[0], delta[1], 0);

            foreach (Engine.Effects.Particles.Obsolete.PixelParticle_O px in t_particles)
            {
                px.Move(d);
                px.Draw(t_imageSource, t_alpha);
            }

            t_previousPoint = p;
        }

        private void CreateParticles(int x, int y)
        {
            t_particles = new Effects.Particles.Obsolete.PixelParticle_O[t_count];

            for (int i = 0; i < t_particles.Length; i++)
            {
                t_particles[i] = new Effects.Particles.Obsolete.PixelParticle_O(Engine.Colors.Black,
                    x + Engine.Calc.Math.NextFloatRandomInRange((float)t_scatter),
                    y + Engine.Calc.Math.NextFloatRandomInRange((float)t_scatter));
            }
        }

        public override int AfterDraw(Point p)
        {
            throw new NotImplementedException();
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            GrainyPen tlt = new GrainyPen();
            tlt.Initialize(w);

            return tlt;
        }

        public override string Name { get => "Grainy Pen"; }

        [Engine.Attributes.Meta.DisplayName("Particle Count")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(30)]
        public int Count
        {
            get { return t_count; }
            set { t_count = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Alpha")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(30)]
        public int Alpha
        {
            get { return t_alpha; }
            set { t_alpha = (byte)value; }
        }

        [Engine.Attributes.Meta.DisplayName("Spread")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(10d)] /* 10d d is required otherwise compiler sets as int and cast in UI will fail */
        public double Scatter
        {
            get { return t_scatter; }
            set { t_scatter = value; }
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
