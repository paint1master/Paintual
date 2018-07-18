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

    public static class Math
    {
        private static int lastColor = (int)(DateTime.Now.ToBinary() / 2);
        public static Random Rand = new Random(lastColor);

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
                divide = height / 16;
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
    }
}
