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
using System.Runtime.InteropServices;
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

            double[] lums = new double[t_imageSource.Height * t_imageSource.Width];

            int[] reds = new int[t_imageSource.Height * t_imageSource.Width];
            int[] greens = new int[t_imageSource.Height * t_imageSource.Width];
            int[] blues = new int[t_imageSource.Height * t_imageSource.Width];

            int offset = 0;

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {
                    Engine.Color.Cell c = t_imageSource.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnNeutralGray);
                    c.InArray(ref blues, ref greens, ref reds, offset);
                    offset++;
                }
            }

            Engine.EngineCppLibrary.Luminance s_luminance = (Engine.EngineCppLibrary.Luminance)Marshal.GetDelegateForFunctionPointer(
                                                    Engine.EngineCppLibrary.Pointer_luminance,
                                                    typeof(Engine.EngineCppLibrary.Luminance));

            s_luminance(t_imageSource.Height * t_imageSource.Width, blues, greens, reds, lums);

            offset = 0;

            for (int y = 0; y < t_imageSource.Height; y++)
            {
                for (int x = 0; x < t_imageSource.Width; x++)
                {

                    double lum = lums[offset];

                    /*
                     * surrounding cell config
                     *     0  1  2
                     *     
                     *     7  x  3
                     *     
                     *     6  5  4
                     *     
                     */

                    double[] localLums = new double[8];

                    localLums[0] = GetLum(lums, x - 1, y - 1);
                    localLums[1] = GetLum(lums, x, y - 1);
                    localLums[2] = GetLum(lums, x + 1, y - 1);
                    localLums[3] = GetLum(lums, x + 1, y);
                    localLums[4] = GetLum(lums, x + 1, y + 1);
                    localLums[5] = GetLum(lums, x, y + 1);
                    localLums[6] = GetLum(lums, x - 1, y + 1);
                    localLums[7] = GetLum(lums, x - 1, y);

                    if (t_invertLuminance)
                    {
                        lum = 255 - lum;

                        localLums[0] = 255 - localLums[0];
                        localLums[1] = 255 - localLums[1];
                        localLums[2] = 255 - localLums[2];
                        localLums[3] = 255 - localLums[3];
                        localLums[4] = 255 - localLums[4];
                        localLums[5] = 255 - localLums[5];
                        localLums[6] = 255 - localLums[6];
                        localLums[7] = 255 - localLums[7];
                    }

                    double[] lumDiffs = new double[8];

                    for (int i = 0; i < lumDiffs.Length; i++)
                    {
                        lumDiffs[i] = lum - localLums[i];
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
                        vs[i].SetMagnitude(lumDiffs[i]);
                        v_this += vs[i];
                    }

                    //v_this.Normalize(Calc.CalculationStyles.Accord);

                    t_field[x].Add(v_this);

                    offset++;
                }
            }
        }

        private double GetLum(double[] lums, int x, int y)
        {
            int result = Engine.Surface.Ops.GetGridOffset(x, y, t_imageSource.Width, t_imageSource.Height);

            if (result == -1)
            {
                return 127;
            }

            return lums[result];
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
