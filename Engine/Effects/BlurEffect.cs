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
using System.Drawing;

using Engine.Tools;


namespace Engine.Effects
{
    public class BlurEffect : Effect
    {
        private double t_radius;
        private double t_sigma;

        public BlurEffect()
        {
            t_visualProperties = new VisualProperties(Name, typeof(BlurEffect));
        }


        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            BlurEffect be = new BlurEffect();
            be.Initialize(w);

            return be;
        }



        public override void Process()
        {
            t_imageProcessed = Engine.Surface.Ops.Copy(t_imageSource);

            t_workflow.Viome.AllowInvalidate();

            t_imageProcessed = Engine.Surface.Ops.Blur(t_imageSource, t_radius, t_sigma);

            base.ProcessCompleted();
        }

        public override string Name { get => "Blur"; }

        [Engine.Attributes.Meta.DisplayName("Radius")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(5d)]
        public double Radius
        {
            get { return t_radius; }
            set { t_radius = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Sigma")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(5d)]
        public double Sigma
        {
            get { return t_sigma; }
            set { t_sigma = value; }
        }
    }
}
