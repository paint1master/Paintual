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
        private Engine.Color.ColorVariance t_colorVariance;
        private bool t_useColorFromImage;

        private MousePoint t_previousPoint;

        public GrainyPen()
        {
            t_visualProperties = new Engine.Effects.VisualProperties(Name, typeof(GrainyPen));
        }

        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);
        }

        public override void BeforeDraw(MousePoint p)
        {
            // in Initialize, it's too early to get UI values (!!??!!)
            t_skipper = new Utilities.Iterativ.Skipper(t_skipperValue);
            t_previousPoint = new MousePoint(p.X, p.Y);

            CreateParticles(p.X, p.Y);
        }

        internal override void Draw(MousePoint p)
        {
            if (!t_skipper.Skip())
                return;

            if (t_imageSource.IsOutOfBounds(p.X, p.Y))
            {
                return;
            }

            Engine.Color.Cell col;
            t_colorVariance.Step();

            if (t_useColorFromImage)
            {
                col = t_imageSource.GetPixel(p.X, p.Y, PixelRetrievalOptions.ReturnEdgePixel);
                t_colorVariance.SetColor(col);
                col = t_colorVariance.ColorVariation;
                col.Alpha = t_alpha;
            }
            else
            {
                col = t_colorVariance.ColorVariation;
                col.Alpha = t_alpha;
            }

            // particles must follow the pen between each call to draw
            int[] delta = MousePoint.Delta(t_previousPoint, p);
            Engine.Calc.Vector d = new Engine.Calc.Vector(delta[0], delta[1]);

            foreach (Engine.Effects.Particles.Obsolete.PixelParticle_O px in t_particles)
            {
                px.Move(d);

                px.Draw(t_imageSource, col);                
            }

            t_previousPoint = p;
        }

        private void CreateParticles(int x, int y)
        {
            t_particles = new Effects.Particles.Obsolete.PixelParticle_O[t_count];

            for (int i = 0; i < t_particles.Length; i++)
            {
                Engine.Color.Cell cVar;

                if (t_useColorFromImage)
                {
                    cVar = t_imageSource.GetPixel(x, y, PixelRetrievalOptions.ReturnEdgePixel);
                    cVar.Alpha = t_alpha;
                }
                else
                {
                    t_colorVariance.Step();

                    // currently colorVariance is off
                    cVar = new Color.Cell(t_colorVariance.Blue, t_colorVariance.Green, t_colorVariance.Red, t_colorVariance.Alpha);
                    cVar.Alpha = t_alpha;
                }

                t_particles[i] = new Effects.Particles.Obsolete.PixelParticle_O(cVar,
                    x + Engine.Calc.Math.NextFloatRandomInRange((float)t_scatter),
                    y + Engine.Calc.Math.NextFloatRandomInRange((float)t_scatter));
            }
        }

        public override void AfterDraw(MousePoint p)
        {
            ;
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
        [Engine.Attributes.Meta.DefaultValue(300)]
        public int Count
        {
            get { return t_count; }
            set { t_count = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Alpha")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.Range(0, 255)]
        [Engine.Attributes.Meta.DefaultValue(15)]
        public int Alpha
        {
            get { return t_alpha; }
            set { t_alpha = (byte)value; }
        }

        [Engine.Attributes.Meta.DisplayName("Spread")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(100d)] /* 10d d is required otherwise compiler sets as int and cast in UI will fail */
        public double Scatter
        {
            get { return t_scatter; }
            set { t_scatter = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Skipper")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(5)]
        public int Skipper
        {
            get { return t_skipperValue; }
            set { t_skipperValue = value; }
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
    }
}
