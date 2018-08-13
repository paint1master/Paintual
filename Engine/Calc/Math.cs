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

namespace Engine.Calc
{
    public enum SizeComparison
    {
        IsWider,
        IsHeigher,
        IsSmaller
    }

    public class Math
    {
        private static int lastColor = (int)(DateTime.Now.ToBinary() / 2);
        public static Random Rand = new Random(lastColor);

        /// <summary>
        /// Returns a random float that is comprised within the specified range. ie: if range is 3, float can be
        /// anything between -2.999... and 2.9... .
        /// </summary>
        /// <param name="range">An int representing a range. ie: 3 allows a range from -3 to 3</param>
        /// <returns></returns>
        public static float NextFloatRandomInRange(double range)
        {
            double d = Rand.NextDouble();

            float f = (float)((d * range) - (range / 2));

            //System.Diagnostics.Debug.WriteLine(String.Format("random : {0}", f));

            return f;
        }

        public static int NextIntRandomInRange(int range)
        {
            double d = Rand.NextDouble();

            int i = (int)((d * range) - (range / 2));

            return i;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">The value that is checked against the min and max values. If the values falls off the limits, the limit value is returned.</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int EnsureWithinRange(int value, int min, int max)
        {
            return System.Math.Min(max, System.Math.Max(min, value));
        }

        public static byte EnsureWithinRange(byte value, byte min, byte max)
        {
            return System.Math.Min(max, System.Math.Max(min, value));
        }

        /// <summary>
        /// Ensure that the provided value (param1) is within min and max.
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="min">The allowed minimum</param>
        /// <param name="max">The allowed maximum</param>
        /// <returns></returns>
        /// <remarks>Code taken from this project : https://www.codeproject.com/Articles/35436/Adobe-Color-Picker-Clone
        /// </remarks>
        public static double ClipValue(double value, double min, double max)
        {
            if (double.IsNaN(value) ||
                double.IsNegativeInfinity(value) ||
                value < min)
                return min;
            else if (double.IsPositiveInfinity(value) ||
                value > max)
                return max;
            else return value;
        }

        public static bool IsPercentageValue(int value)
        {
            if (value < 0 || value > 100)
            {
                return false;
               // throw new ArgumentOutOfRangeException(string.Format("In Engine.Calc IsPercentageValue(), the value '{0}' is not usable as a percentage.", value));
            }

            return true;
        }

        public static byte StringToByte(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentOutOfRangeException(string.Format("In Engine.Calc StringToByte(), a null or empty string cannot be parsed into a byte."));
            }

            byte result = byte.Parse(value);

            return result;
        }

        public static int StringToInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentOutOfRangeException(string.Format("In Engine.Calc StringToInt(), a null or empty string cannot be parsed into an int."));
            }

            int result = Int32.Parse(value);

            return result;
        }

        public static bool StringToBool(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentOutOfRangeException(string.Format("In Engine.Calc StringToBool(), a null or empty string cannot be parsed into a bool value."));
            }

            bool result = false;
            string lower = value.ToLower();

            if (lower == "true")
            {
                result = true;
            }
            else if (lower == "false")
            {
                result = false;
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("In Engine.Calc StringToBool(), the value '{0}' cannot be parsed into a bool value.", value));
            }

            return result;
        }

        public static SizeComparison IsWiderOrHigher(int canvasWidth, int boardWidth, int canvasHeight, int boardHeight)
        {
            bool isWider = false;
            bool isHeigher = false;

            int deltaWidth = canvasWidth - boardWidth;
            int deltaHeight = canvasHeight - boardHeight;

            if (deltaWidth > 0)
            {
                isWider = true;
            }

            if (deltaHeight > 0)
            {
                isHeigher = true;
            }

            if ((isWider == true && isHeigher == false) || (isWider == true && isHeigher == true))
            {
                return SizeComparison.IsWider;
            }

            if (isWider == false && isHeigher == true)
            {
                return SizeComparison.IsHeigher;
            }

            return SizeComparison.IsSmaller;
        }

        public static SizeComparison IsWiderOrHigher(double width, double height)
        {
            if (width >= height)
            {
                return SizeComparison.IsWider;
            }

            return SizeComparison.IsHeigher;
        }

        /// <summary>
        /// Divides a value so that it doesn't create too many threads with too little work to do
        /// </summary>
        /// <returns></returns>
        public static int Division_Threading(int height)
        {
            int divide = 1;

            if (height > 8 && height <= 16)
            {
                divide = 2;
            }
            else if (height > 16 && height <= 32)
            {
                divide = 4;
            }
            else if (height > 32)
            {
                divide = 16;
            }

            return divide;
        }

        public static int Double_0_1_ToDegree(double d)
        {
            if (d < 0 || d > 1)
            {
                throw new ArgumentOutOfRangeException(String.Format("In Engine.Calc.Math Double_0_1_ToDegree() the value {0} cannot be converted to a degree because it is outside of the 0-1 boundaries", d));
            }

            double mult = d * 360;

            return (int)System.Math.Round(mult);
        }

        // below code comes from : https://libnoisedotnet.codeplex.com/downloads/get/720936
        // and http://libnoise.sourceforge.net/tutorials/tutorial8.html

        /// <summary>
        /// Returns the given value clamped between the given lower and upper bounds.
        /// </summary>
        public static int ClampValue(int value, int lowerBound, int upperBound)
        {
            if (value < lowerBound)
            {
                return lowerBound;
            }
            else if (value > upperBound)
            {
                return upperBound;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the cubic interpolation of two values bound between two other values.
        /// </summary>
        /// <param name="n0">The value before the first value.</param>
        /// <param name="n1">The first value.</param>
        /// <param name="n2">The second value.</param>
        /// <param name="n3">The value after the second value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns></returns>
        public static double CubicInterpolate(double n0, double n1, double n2, double n3, double a)
        {
            double p = (n3 - n2) - (n0 - n1);
            double q = (n0 - n1) - p;
            double r = n2 - n0;
            double s = n1;
            return p * a * a * a + q * a * a + r * a + s;
        }

        /// <summary>
        /// Returns the smaller of the two given numbers.
        /// </summary>
        public static double GetSmaller(double a, double b)
        {
            return (a < b ? a : b);
        }

        /// <summary>
        /// Returns the larger of the two given numbers.
        /// </summary>
        public static double GetLarger(double a, double b)
        {
            return (a > b ? a : b);
        }

        /// <summary>
        /// Swaps the values contained by the two given variables.
        /// </summary>
        public static void SwapValues(ref double a, ref double b)
        {
            double c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Returns the linear interpolation of two values with the given alpha.
        /// </summary>
        public static double LinearInterpolate(double n0, double n1, double a)
        {
            return ((1.0 - a) * n0) + (a * n1);
        }

        /// <summary>
        /// Returns the given value, modified to be able to fit into a 32-bit integer.
        /// </summary>
        /*public double MakeInt32Range(double n)
        {
            if (n >= 1073741824.0)
            {
                return ((2.0 * System.Math.IEEERemainder(n, 1073741824.0)) - 1073741824.0);
            }
            else if (n <= -1073741824.0)
            {
                return ((2.0 * System.Math.IEEERemainder(n, 1073741824.0)) + 1073741824.0);
            }
            else
            {
                return n;
            }
        }*/

        /// <summary>
        /// Returns the given value mapped onto a cubic S-curve.
        /// </summary>
        public static double SCurve3(double a)
        {
            return (a * a * (3.0 - 2.0 * a));
        }

        /// <summary>
        /// Returns the given value mapped onto a quintic S-curve.
        /// </summary>
        public static double SCurve5(double a)
        {
            double a3 = a * a * a;
            double a4 = a3 * a;
            double a5 = a4 * a;
            return (6.0 * a5) - (15.0 * a4) + (10.0 * a3);
        }

        /// <summary>
        /// Returns the value of the mathematical constant PI.
        /// </summary>
        public static readonly double PI = 3.1415926535897932385;

        /// <summary>
        /// Returns the square root of 2.
        /// </summary>
        public static readonly double Sqrt2 = 1.4142135623730950488;

        /// <summary>
        /// Returns the square root of 3.
        /// </summary>
        public static readonly double Sqrt3 = 1.7320508075688772935;

        /// <summary>
        /// Returns PI/180.0, used for converting degrees to radians.
        /// </summary>
        public static readonly double DEG_TO_RAD = PI / 180.0;

        /// <summary>
        /// Provides the X, Y, and Z coordinates on the surface of a sphere 
        /// cooresponding to the given latitude and longitude.
        /// </summary>
        protected void LatLonToXYZ(double lat, double lon, ref double x, ref double y, ref double z)
        {
            double r = System.Math.Cos(DEG_TO_RAD * lat);
            x = r * System.Math.Cos(DEG_TO_RAD * lon);
            y = System.Math.Sin(DEG_TO_RAD * lat);
            z = r * System.Math.Sin(DEG_TO_RAD * lon);
        }

        public static float Sigmoid(float f)
        {
            float result = (float)(1d / (1d + System.Math.Exp(f * -1)));

            return result;
        }

        public static List<MousePoint> LinearInterpolate(MousePoint start, MousePoint end)
        {
            List<MousePoint> points = new List<MousePoint>();

            int X1 = start.X;
            int Y1 = start.Y;
            int X2 = end.X;
            int Y2 = end.Y;

            int deltaX = System.Math.Abs(X2 - X1);
            int deltaY = System.Math.Abs(Y2 - Y1);

            int maxSteps = System.Math.Max(deltaX, deltaY);

            if (maxSteps <= 1)
            {
                points.Add(start);
                return points;
            }

            deltaX = X2 - X1;
            deltaY = Y2 - Y1;

            float increaseX = (float)deltaX / (float)maxSteps;
            float increaseY = (float)deltaY / (float)maxSteps;

            points.Add(new Engine.MousePoint(X1, Y1, Engine.MouseActionType.MouseMove, true));

            for (int i = 0; i < maxSteps; i++)
            {
                int stepX = (int)((i * increaseX) + (float)X1);
                int stepY = (int)((i * increaseY) + (float)Y1);

                MousePoint p2 = new Engine.MousePoint(stepX, stepY, Engine.MouseActionType.MouseMove, false);
                points.Add(p2);
            }

            points.Add(new Engine.MousePoint(X2, Y2, Engine.MouseActionType.MouseMove, true));

            return points;
        }
    }
}
