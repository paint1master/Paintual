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

using Engine.Surface;

namespace Engine.Tools
{
    public class Brightness : Engine.Tools.Tool
    {
        // int because need to accept negative values to darken portions of image
        private int t_brightnessAmount = 5;

        private Engine.Surface.Mask t_mask;

        public Brightness()
        {
            t_visualProperties = new Engine.Effects.VisualProperties(Name, typeof(Brightness));
        }


        public override void Initialize(Engine.Workflow w)
        {
            base.Initialize(w);

            t_mask = new Mask(t_imageSource.Width, t_imageSource.Height, MaskValue.WhiteHide);
        }

        public override void BeforeDraw(MousePoint p)
        {
            // in Initialize, it's too early to get UI values (!!??!!)
            t_mask = new Mask(t_imageSource.Width, t_imageSource.Height, MaskValue.WhiteHide);
        }

        internal override void Draw(MousePoint p)
        {
            for (int y = p.Y - 10; y < p.Y + 10; y++)
            {
                for (int x = p.X - 10; x < p.X + 10; x++)
                {
                    if (t_mask.GetPixel(x, y, PixelRetrievalOptions.ReturnEdgePixel) == MaskValue.BlackReveal)
                    {
                        continue;
                    }

                    Engine.Color.Cell c = t_imageSource.GetPixel(x, y, PixelRetrievalOptions.ReturnEdgePixel);
                    c.ChangeBrightness(t_brightnessAmount);
                    t_imageSource.SetPixel(c, x, y, PixelSetOptions.Ignore);

                    t_mask.SetPixel(MaskValue.BlackReveal, x, y, PixelSetOptions.Ignore);
                }
            }
        }

        public override void AfterDraw(MousePoint p)
        {
            ;
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            Brightness b = new Brightness();
            b.Initialize(w);

            return b;
        }

        public override string Name { get => "Brightness"; }

        [Engine.Attributes.Meta.DisplayName("Brightness")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(5)]
        [Engine.Attributes.Meta.Range(-255, 255)]
        public int BrightnessAmount
        {
            get { return t_brightnessAmount; }
            set { t_brightnessAmount = value; }
        }
    }
}
