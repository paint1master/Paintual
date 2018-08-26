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

namespace Engine.Effects.Particles
{
    public class FlowField
    {
        private Engine.Surface.Canvas t_imageSource;
        private List<List<Engine.Calc.Vector>> t_field;
        private double t_variance = 12d;

        private bool t_invertLuminance;

        public FlowField(Engine.Surface.Canvas c)
        {
            t_imageSource = c;
            SetField();
        }

        public FlowField(Engine.Surface.Canvas c, bool invertLuminance)
        {
            t_imageSource = c;
            t_invertLuminance = invertLuminance;
            SetField();
        }

        private void SetField()
        {
            t_field = new List<List<Engine.Calc.Vector>>();

            for (int i = 0; i < t_imageSource.Width; i++)
            {
                t_field.Add(new List<Engine.Calc.Vector>());
            }

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    Engine.Color.Cell c = t_imageSource.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnNeutralGray);

                    double lum = Engine.Calc.Color.Luminance(c);

                    /*
                     * surrounding cell config
                     *     0  1  2
                     *     
                     *     7  x  3
                     *     
                     *     6  5  4
                     *     
                     */

                    double[] lums = new double[8];

                    lums[0] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x - 1, y - 1, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[1] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x, y - 1, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[2] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x + 1, y - 1, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[3] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x + 1, y, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[4] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x + 1, y + 1, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[5] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x, y + 1, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[6] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x - 1, y + 1, Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    lums[7] = Engine.Calc.Color.Luminance(t_imageSource.GetPixel(x - 1, y, Surface.PixelRetrievalOptions.ReturnNeutralGray));

                    if (t_invertLuminance)
                    {
                        lum = 255 - lum;

                        lums[0] = 255 - lums[0];
                        lums[1] = 255 - lums[1];
                        lums[2] = 255 - lums[2];
                        lums[3] = 255 - lums[3];
                        lums[4] = 255 - lums[4];
                        lums[5] = 255 - lums[5];
                        lums[6] = 255 - lums[6];
                        lums[7] = 255 - lums[7];
                    }

                    double[] lumDiffs = new double[8];

                    for (int i = 0; i < lumDiffs.Length; i++)
                    {
                        lumDiffs[i] = lum - lums[i];
                    }

                    Engine.Calc.Vector[] vs = new Calc.Vector[lumDiffs.Length];

                    vs[0] = new Engine.Calc.Vector(-1, -1);
                    vs[1] = new Engine.Calc.Vector(0, -1);
                    vs[2] = new Engine.Calc.Vector(1, -1);
                    vs[3] = new Engine.Calc.Vector(1, 0);
                    vs[4] = new Engine.Calc.Vector(1, 1);
                    vs[5] = new Engine.Calc.Vector(0, 1);
                    vs[6] = new Engine.Calc.Vector(-1, 1);
                    vs[7] = new Engine.Calc.Vector(-1, 0);

                    Engine.Calc.Vector v_this = new Engine.Calc.Vector(0, 0);

                    for (int i = 0; i < vs.Length; i++)
                    {
                        vs[i].SetMagnitude(lums[i]);
                        v_this += vs[i];
                    }

                    //v_this.Normalize(Calc.CalculationStyles.Accord);

                    t_field[x].Add(v_this);
                }
            }
        }

        public Engine.Calc.Vector GetVector(int x, int y)
        {
            return t_field[x][y];
        }

        public double Variance
        {
            get { return t_variance; }
            set { t_variance = value; }
        }

        public bool InvertLuminance { get => t_invertLuminance; set => t_invertLuminance = value; }
    }
}
