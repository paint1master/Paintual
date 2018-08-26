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
    public enum DiffusionTypes
    {
        Velocity,
        Pressure,
        Ink,
        Heat
    }

    /// <summary>
    /// Specifically used with Fluid.cs, represents a plane onto which values are stored.
    /// This plane comprises a series of array which is deemed more efficient.
    /// </summary>
    /// <remarks>Code taken and adapted from : http://cowboyprogramming.com/2008/04/01/practical-fluid-mechanics/ </remarks>
    public class FluidPlane
    {
        private int t_width;
        private int t_height;
        private int t_size;

        private double m_dt;

        /// <summary>
        /// Array standing for vector
        /// </summary>
        private double[] mp_xv0;

        /// <summary>
        /// Array standing for vector
        /// </summary>
        private double[] mp_yv0;

        /// <summary>
        /// Array standing for vector
        /// </summary>
        private double[] mp_xv1;

        /// <summary>
        /// Array standing for vector
        /// </summary>
        private double[] mp_yv1;

        /// <summary>
        /// Array standing for vector
        /// </summary>
        private double[] mp_xv2;

        /// <summary>
        /// Array standing for vector
        /// </summary>
        private double[] mp_yv2;

        /// <summary>
        /// Array standing for vector : pressure
        /// </summary>
        private double[] mp_p0;

        /// <summary>
        /// Array standing for vector : pressure
        /// </summary>
        private double[] mp_p1;

        // temp accumulators of data for advection
        private int[] mp_sources;
        private double[] mp_source_fractions;
        private double[] mp_fraction;

        // Old and new ink
        private double[] mp_ink0;
        private double[] mp_ink1;

        private double[] mp_heat0;
        private double[] mp_heat1;

        // BFECC needs its own buffer
        // can't reuse other, since they might be used by the advection step
        private double[] mp_BFECC;

        private double[] mp_balance;

        public FluidPlane(int width, int height)
        {
            t_width = width;
            t_height = height;
            t_size = t_width * t_height;

            mp_xv0 = new double[Size];
            mp_yv0 = new double[Size];
            mp_xv1 = new double[Size];
            mp_yv1 = new double[Size];
            mp_xv2 = new double[Size];
            mp_yv2 = new double[Size];
            mp_p0 = new double[Size];
            mp_p1 = new double[Size];
            mp_ink0 = new double[Size];
            mp_ink1 = new double[Size];
            mp_heat0 = new double[Size];
            mp_heat1 = new double[Size];
            mp_sources = new int[Size];
            mp_source_fractions = new double[Size * 4];
            mp_fraction = new double[Size];
            mp_BFECC = new double[Size];
            mp_balance = new double[Size];
        }

        public void FillFluid(Engine.Surface.Canvas c)
        {
            for (int y = 0; y < c.Height; y++)
            {
                for (int x = 0; x < c.Width; x++)
                {
                    Engine.Color.Cell pix = c.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
                    double lum = (double)Engine.Calc.Color.Luminance(pix);
                    int offset = CellOffset(x, y, c.Width);
                    mp_p0[offset] = lum; // lum /255;
                    mp_xv0[offset] = lum;
                    mp_yv0[offset] = lum;
                }
            }
        }

        public Engine.Surface.Canvas ReadImage(int clarityFactor)
        {
            Engine.Surface.Canvas c = new Surface.Canvas(t_width, t_height);

            int offset = 0;

            double highestvalue = 0;
            double lowestvalue = 1000;

            for (int i = 0; i < mp_p0.Length; i++)
            {
                if (highestvalue < mp_p0[i])
                {
                    highestvalue = mp_p0[i];
                }

                if (lowestvalue > mp_p0[i])
                {
                    lowestvalue = mp_p0[i];
                }
            }

            for (int y = 0; y < t_height; y++)
            {
                for (int x = 0; x < t_width; x++)
                {
                    double lum = mp_p0[offset];
                    lum = Engine.Calc.Math.Map(lum, lowestvalue, highestvalue, clarityFactor, 255 - clarityFactor);

                    byte lumRGB = (byte)lum; // (byte)(lum * 255); //CosineInterpolation((byte)lum); //
                    Engine.Color.Cell pix = Engine.Color.Cell.ShadeOfGray(lumRGB);
                    c.SetPixel(pix, x, y, Surface.PixelSetOptions.Ignore);
                    offset++;
                }
            }

            return c;
        }

        public void Diffusion(DiffusionTypes dt,  Fluid f, double md_t, double scale)
        {
            scale /= 100d;

            switch(dt)
            {
                case DiffusionTypes.Velocity:
                    Diffusion(ref mp_xv0, ref mp_xv1, f, t_width, t_height, m_dt, scale);
                    Engine.Calc.Math.Swap(ref mp_xv0, ref mp_xv1);
                    Diffusion(ref mp_yv0, ref mp_yv1, f, t_width, t_height, m_dt, scale);
                    Engine.Calc.Math.Swap(ref mp_yv0, ref mp_yv1);
                    break;

                case DiffusionTypes.Pressure:
                    Diffusion(ref mp_p0, ref mp_p1, f, t_width, t_height, m_dt, scale);
                    Engine.Calc.Math.Swap(ref mp_p0, ref mp_p1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(String.Format("In FluidPlane.Diffusion() DiffusionType {0} is not supported.", dt.ToString()));
            }
        }

        private static void Diffusion(ref double[] p_in, ref double[] p_out, Fluid f, int width, int height, double m_dt, double scale)
        {
            // Build the new pressure values in p_out, then swap p_out and p_in

            //StringBuilder sb = new StringBuilder();

            double a = m_dt * scale;
            int cell;

            // top and bot edges
            for (int x = 1; x < width - 1; x++)
            {
                cell = CellOffset(x, 0, width);
                p_out[cell] = p_in[cell] + a * (p_in[cell - 1] + p_in[cell + 1] + p_in[cell + height] - 3.0d * p_in[cell]);
                //sb.Append(p_out[cell].ToString() + Environment.NewLine);
                cell = CellOffset(x, height - 1, width);
                p_out[cell] = p_in[cell] + a * (p_in[cell - 1] + p_in[cell + 1] + p_in[cell - height] - 3.0d * p_in[cell]);
                //sb.Append(p_out[cell].ToString() + Environment.NewLine);
            }
            // left and right edges
            for (int y = 1; y < height - 1; y++)
            {
                cell = CellOffset(0, y, width);
                p_out[cell] = p_in[cell] + a * (p_in[cell - width] + p_in[cell + width] + p_in[cell + 1] - 3.0d * p_in[cell]);
                //sb.Append(p_out[cell].ToString() + Environment.NewLine);
                cell = CellOffset(width - 1, y, width);
                p_out[cell] = p_in[cell] + a * (p_in[cell - width] + p_in[cell + width] + p_in[cell - 1] - 3.0d * p_in[cell]);
                //sb.Append(p_out[cell].ToString() + Environment.NewLine);
            }
            // corners
            cell = CellOffset(0, 0, width);
            p_out[cell] = p_in[cell] + a * (p_in[cell + 1] + p_in[cell + width] - 2.0d * p_in[cell]);
            //sb.Append(p_out[cell].ToString() + Environment.NewLine);
            cell = CellOffset(width - 1, 0, width);
            p_out[cell] = p_in[cell] + a * (p_in[cell - 1] + p_in[cell + width] - 2.0d * p_in[cell]);
            //sb.Append(p_out[cell].ToString() + Environment.NewLine);
            cell = CellOffset(0, height - 1, width);
            p_out[cell] = p_in[cell] + a * (p_in[cell + 1] + p_in[cell - width] - 2.0d * p_in[cell]);
            //sb.Append(p_out[cell].ToString() + Environment.NewLine);
            cell = CellOffset(width - 1, height - 1, width);
            p_out[cell] = p_in[cell] + a * (p_in[cell - 1] + p_in[cell - width] - 2.0d * p_in[cell]);
            //sb.Append(p_out[cell].ToString() + Environment.NewLine);

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    cell = CellOffset(x, y, width);
                    p_out[cell] = p_in[cell] + a * (p_in[CellOffset(x, y + 1, width)] + p_in[CellOffset(x, y - 1, width)] + p_in[CellOffset(x + 1, y, width)] + p_in[CellOffset(x - 1, y, width)] - 4.0d * p_in[cell]);
                    //sb.Append(p_out[cell].ToString() + Environment.NewLine);
                }
            }

            //string timestamp = DateTime.Now.ToLongDateString() + DateTime.Now.Millisecond.ToString();
            //Engine.Utilities.SFO.WriteFile(@"D:\Docs\Blog Belisssle\2018-08-22\" + timestamp, sb.ToString());
        }

        public void VorticityConfinement(double scale)
        {
            scale /= 100d;

            ZeroEdge(ref mp_p1);
            ZeroField(ref mp_xv1);
            ZeroField(ref mp_yv1);

            double[] p_abs_curl = new double[mp_p1.Length];

            CopyField(mp_p1, ref p_abs_curl);

            for (int i = 1; i < t_width - 1; i++)
            {
                for (int j = 1; j < t_height - 1; j++)
                {
                    p_abs_curl[CellOffset(i, j, t_width)] = System.Math.Abs(Curl(i, j));
                }
            }

            for (int x = 2; x < t_width - 2; x++)
            {
                for (int y = 2; y < t_height - 2; y++)
                {
                    int cell = CellOffset(x, y, t_width);

                    // get curl gradient across this cell, left right
                    double lr_curl = (p_abs_curl[cell + 1] - p_abs_curl[cell - 1]) * 0.5d;
                    // and up down
                    double ud_curl = (p_abs_curl[cell + t_width] - p_abs_curl[cell + t_height]) * 0.5d;

                    // Normalize the derivitive curl vector
                    double length = System.Math.Sqrt(lr_curl * lr_curl + ud_curl * ud_curl) + 0.000001d;
                    lr_curl /= length;
                    ud_curl /= length;

                    // get the magnitude of the curl
                    double v = Curl(x, y);

                    // (lr,ud) would be perpendicular, so (-ud,lr) is tangential? 
                    mp_xv1[CellOffset(x, y, t_width)] = -ud_curl * v;
                    mp_yv1[CellOffset(x, y, t_width)] = lr_curl * v;
                }
            }

            ForceFrom(ref mp_xv1, ref mp_xv0, scale);
            ForceFrom(ref mp_yv1, ref mp_yv0, scale);
        }

        public void VelocityFriction(double a, double b, double c)
        {
            // The force of pressure affects velocity

            for (int iterate = 0; iterate < 1; iterate++)
            {
                // NOTE: We include the border cells in global friction
                for (int x = 0; x < t_width; x++)
                {
                    for (int y = 0; y < t_height; y++)
                    {
                        int cell = CellOffset(x, y, t_width);

                        Engine.Calc.Vector v = new Engine.Calc.Vector(mp_xv0[cell], mp_yv0[cell]);

                        double len2 = v.MagnitudeSquared();
                        double len = System.Math.Sqrt(len2);

                        if (len < 0.0f)
                        {
                            len = -len;
                        }

                        len -= m_dt * (a * len2 + b * len + c);

                        if (len < 0.0d)
                            len = 0.0d;

                        if (len < 0.0f)
                            len = 0.0f;

                        v.Normalize();
                        v.SetMagnitude(len);
                        mp_xv0[cell] = v.X;
                        mp_yv0[cell] = v.Y;
                    }
                }
            }
        }

        public void ForwardAdvection(DiffusionTypes dt, double scale)
        {
            scale /= 100d;

            switch (dt)
            {
                case DiffusionTypes.Velocity:
                    ForwardAdvection(ref mp_xv0, ref mp_xv1, scale);
                    ForwardAdvection(ref mp_yv0, ref mp_yv1, scale);

                    // in original code, swapping for both x and y is done after x and y advection
                    Engine.Calc.Math.Swap(ref mp_xv0, ref mp_xv1);
                    Engine.Calc.Math.Swap(ref mp_yv0, ref mp_yv1);
                    break;

                case DiffusionTypes.Pressure:
                    ForwardAdvection(ref mp_p0, ref mp_p1, scale);
                    Engine.Calc.Math.Swap(ref mp_p0, ref mp_p1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(String.Format("In FluidPlane.Diffusion() DiffusionType {0} is not supported.", dt.ToString()));
            }
        }

        // Move scalar along the velocity field
        // Forward advection moves the value at a point forward along the vector from the same point
        // and dissipates it between four points as needed
        private void ForwardAdvection(ref double[] p_in, ref double[] p_out, double scale)
        {
            // Pressure differential between points  
            // to get an accelleration force.
            double a = m_dt * scale;

            int w = t_width;
            int h = t_height;

            // First copy the values
            CopyField(p_in, ref p_out);

            if (scale == 0.0d)
                return;

            // We advect all cells
            for (int x = 1; x < w - 1; x++)
            {
                for (int y = 1; y < h - 1; y++)
                {
                    int cell = CellOffset(x, y, t_width);
                    double vx = mp_xv0[cell];
                    double vy = mp_yv0[cell];
                    if (vx != 0.0f || vy != 0.0f)
                    {
                        // calculations to obtain values and cell coordinates for
                        // square A B C D
                        double x1 = System.Math.Abs( x + vx * a);
                        double y1 = System.Math.Abs(y + vy * a);

                        //Collide(x, y, ref x1, ref y1);

                        int cell1 = CellOffset((int)x1, (int)y1, t_width);
                        // get fractional parts
                        double fx = x1 - (int)x1;
                        double fy = y1 - (int)y1;

                        // we are taking pressure from four cells (one of which might be the target cell
                        // and adding it to the target cell
                        // hence we need to subtract the appropiate amounts from each cell
                        //
                        // Cells are like
                        // 
                        //    A----B
                        //    |    |
                        //    |    |
                        //    C----D
                        // 
                        // (Should be square)
                        // so we work out the bilinear fraction at each of a,b,c,d

                        double pin = p_in[cell];

                        double ia = (1.0f - fy) * (1.0f - fx) * pin;
                        double ib = (1.0f - fy) * (fx) * pin;
                        double ic = (fy) * (1.0f - fx) * pin;
                        double id = (fy) * (fx) * pin;

                        // Subtract them from the source cell
                        p_out[cell] -= (ia + ib + ic + id);
                        // Then add them to the four destination cells

                        if (cell1 < p_out.Length && cell1 >= 0)
                        {
                            p_out[cell1] += ia;
                        }

                        if (cell1 + 1 < p_out.Length && cell1 + 1 >= 0)
                        {
                            p_out[cell1 + 1] += ib;
                        }

                        if (cell1 + w < p_out.Length && cell1 + 1 >= 0)
                        {
                            p_out[cell1 + w] += ic;
                        }

                        if (cell1 + w + 1 < p_out.Length && cell1 + 1 >= 0)
                        {
                            p_out[cell1 + w + 1] += id;
                        }
                    }
                }
            }
        }

        // NEW - treat cells pairwise, which allows us to handle edge cells
        // updates ALL cells
        public void PressureAcceleration(double force)
        {
            if (force <= 0)
            {
                return;
            }

            // Pressure differential between points  
            // to get an accelleration force.
            double a = m_dt * force;

            // First copy the values
            for (int x = 0; x < t_width; x++)
            {
                for (int y = 0; y < t_height; y++)
                {
                    int cell = CellOffset(x, y, t_width);
                    mp_xv1[cell] = mp_xv0[cell];
                    mp_yv1[cell] = mp_yv0[cell];
                }
            }

            //	for (int iterate = 0; iterate<1;iterate++)
            {
                if (true)//(Fluid_statics.FluidWrapping)
                {
                    for (int x = 0; x < t_width - 1; x++)
                    {
                        for (int y = 0; y < t_height - 1; y++)
                        {
                            int cell = CellOffset(x, y, t_width);

                            double force_x = mp_p0[cell] - mp_p0[cell + 1];
                            double force_y = mp_p0[cell] - mp_p0[cell + t_width];

                            // forces act in same direction on both cells
                            /*if (1 || this == &g_fluid1)
                            {
                                mp_xv1[cell] += a * force_x;
                                mp_xv1[cell + 1] += a * force_x;

                                mp_yv1[cell] += a * force_y;
                                mp_yv1[cell + t_width] += a * force_y;
                            }
                            else
                            {*/
                                if (force_x > 0.0d)
                                {
                                    mp_xv1[cell] += a * force_x * force_x;
                                    mp_xv1[cell + 1] += a * force_x * force_x;
                                }
                                else
                                {
                                    mp_xv1[cell] -= a * force_x * force_x;
                                    mp_xv1[cell + 1] -= a * force_x * force_x;
                                }

                                if (force_y > 0)
                                {
                                    mp_yv1[cell] += a * force_y * force_y;
                                    mp_yv1[cell + t_width] += a * force_y * force_y;
                                }
                                else
                                {
                                    mp_yv1[cell] -= a * force_y * force_y;
                                    mp_yv1[cell + t_width] -= a * force_y * force_y;
                                }
                            //}

                        }
                    }
                }
                else
                {
                    for (int x = 0; x < t_width; x++)
                    {
                        for (int y = 0; y < t_height; y++)
                        {
                            int cell = CellOffset(x, y, t_width);

                            int nextx = cell + 1;
                            if (x == t_width - 1) nextx = CellOffset(0, y, t_width);

                            int nexty = cell + t_width;
                            if (y == t_height - 1) nexty = CellOffset(x, 0, t_width);

                            double force_x = mp_p0[cell] - mp_p0[nextx];
                            double force_y = mp_p0[cell] - mp_p0[nexty];

                            // forces act in same direction  on both cells
                            mp_xv1[cell] += a * force_x;
                            mp_xv1[nextx] += a * force_x;  // B-A

                            mp_yv1[cell] += a * force_y;
                            mp_yv1[nexty] += a * force_y;
                        }
                    }
                }

                Engine.Calc.Math.Swap(ref mp_xv1, ref mp_xv0);
                Engine.Calc.Math.Swap(ref mp_yv1, ref mp_yv0);
            }
        }

        /**
         * Calculate the curl at position (i, j) in the fluid grid.
         * Physically this represents the vortex strength at the
         * cell. Computed as follows: w = (del x U) where U is the
         * velocity vector at (i, j).
         *
         **/

        public double Curl(int x, int y)
        {
            // difference in XV of cells above and below
            // positive number is a counter-clockwise rotation
            double x_curl = (mp_xv0[CellOffset(x, y + 1, t_width)] - mp_xv0[CellOffset(x, y - 1, t_width)]) * 0.5f;

            // difference in YV of cells left and right
            // positive number is a counter-clockwise rotation
            double y_curl = (mp_yv0[CellOffset(x + 1, y, t_width)] - mp_yv0[CellOffset(x - 1, y, t_width)]) * 0.5f;

            return x_curl - y_curl;
        }


        // Given a field p_from, and a field p_to, then add f*p_from to p_to
        // can be used to apply a heat field to velocity
        public void ForceFrom(ref double[] p_from, ref double[] p_to, double f)
        {
            f *= m_dt;

            for (int i = 0; i < Size; i++)
            {
                p_to[i] += p_from[i] * f;
            }
        }


        public void CopyField(double[] p_in, ref double[] p_out)
        {
            for (int x = 0; x < Size; x++)
            {
                p_out[x] = p_in[x];
            }
        }

        public void ZeroEdge(ref double[] p_in)
        {
            for (int x = 0; x < t_width; x++)
            {
                p_in[x] = 0;
                p_in[t_width * (t_height - 1) + x] = 0;
            }

            for (int y = 1; y < t_height - 1; y++)
            {
                p_in[y * t_width] = 0;
                p_in[y * t_width + (t_width - 1)] = 0;
            }
        }

        public void ZeroField(ref double[] p_field)
        {
            for (int i = 0; i < Size; i++)
            {
                p_field[i] = 0;
            }
        }

        public int Size { get => t_size; }

        public double MDt { get => m_dt; set => m_dt = value; }

        private static int CellOffset(int x, int y, int width)
        {
            return x + (width * y);
        }

        /// <summary>
        /// Returns the computed value of b along the cosine curve
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte CosineInterpolation(byte b)
        {
            double result = (1 - System.Math.Cos(b * System.Math.PI / 255)) * (255 / 2);

            return (byte)result;
        }
    }
}
