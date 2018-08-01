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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Engine.Color
{
    /// <summary>
    /// Represents color information of a pixel
    /// </summary>
    public struct Cell
    {
        #region fields
        private byte t_blue;
        private byte t_green;
        private byte t_red;
        private byte t_alpha;

        #endregion
       
        public Cell(int argb)
        {
            t_alpha = (byte)(argb >> 24);
            t_red = (byte)(argb >> 16);
            t_green = (byte)(argb >> 8);
            t_blue = (byte)(argb);
        }

        public Cell(int[] array, int offset) : this(array[offset])
        {

        }

        public Cell(System.Drawing.Color c) : this(c.B, c.G, c.R, c.A) {

        }

        public Cell(byte blue, byte green, byte red, byte alpha) {
            t_blue = blue;
            t_green = green;
            t_red = red;
            t_alpha = alpha;
        }

        public Cell(byte[] imageData, int offset) {
            t_blue = imageData[offset++];
            t_green = imageData[offset++];
            t_red = imageData[offset++];
            t_alpha = imageData[offset];
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="channel">use EngineB.Surface.ColorBitShift values</param>
        /// <returns></returns>
        private int NewColor(byte intensity, Engine.Channel channel)
        {
            // how many bits you should shift replaceByte to bring it "in position"
            int shiftBits = 8 * (int)channel;

            // bitwise AND this with value to clear the bits that should become replaceByte
            int mask = ~(0xff << shiftBits);

            // clear those bits and then set them to whatever replaceByte is

            return t_color & mask | (intensity << shiftBits);
        }*/

        /*
        public void WriteBytes(int[] imageData, int offset) {
            if (offset >= imageData.Length)
            {
                throw new ArgumentOutOfRangeException(String.Format("the offset provided {0} would get outside the limit of the imageData array length of {1}", offset, imageData.Length));
            }

            imageData[offset] = t_color;
        }*/

        public void WriteBytes(byte[] imageData, int offset)
        {
            if (offset >= imageData.Length)
            {
                throw new ArgumentOutOfRangeException(String.Format("the offset provided {0} would get outside the limit of the imageData array length of {1}", offset, imageData.Length));
            }

            imageData[offset++] = t_blue;
            imageData[offset++] = t_green;
            imageData[offset++] = t_red;
            imageData[offset] = t_alpha;
        }

        public void WriteBytesWithAlpha(byte[] imageData, byte alpha, int offset)
        {
            if (offset >= imageData.Length)
            {
                throw new ArgumentOutOfRangeException(String.Format("the offset provided {0} would get outside the limit of the imageData array length of {1}", offset, imageData.Length));
            }

            imageData[offset++] = t_blue;
            imageData[offset++] = t_green;
            imageData[offset++] = t_red;
            imageData[offset] = alpha;
        }

        public void WriteInt(int[] imageData, int offset)
        {
            if (offset >= imageData.Length)
            {
                throw new ArgumentOutOfRangeException(String.Format("the offset provided {0} would get outside the limit of the imageData array length of {1}", offset, imageData.Length));
            }

            imageData[offset] = Int;
        }

        public byte Blue
        {
            get
            {
                return t_blue;
                /*
                return BitConverter.GetBytes(t_color)[0];*/
            }
            set
            {
                t_blue = value;
                /*
                t_color = NewColor(value, Engine.Channel.Blue);*/
            }
        }

        public byte Green
        {
            get
            {
                return t_green;
                /*
                return BitConverter.GetBytes(t_color)[1];*/
            }
            set
            {
                t_green = value;
                /*
                t_color = NewColor(value, Engine.Channel.Green);*/
            }
        }

        public byte Red
        {
            get
            {
                return t_red;
                /*
                return BitConverter.GetBytes(t_color)[2];*/
            }
            set
            {
                t_red = value;
                /*
                t_color = NewColor(value, Engine.Channel.Red);*/
            }
        }

        public byte Alpha
        {
            get
            {
                return t_alpha;
                /*
                return BitConverter.GetBytes(t_color)[3];*/
            }
            set
            {
                t_alpha = value;
                /*
                t_color = NewColor(value, Engine.Channel.Alpha);*/
            }
        }

        /*
        /// <summary>
        /// Direct access to a specific channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public unsafe byte this[Engine.Channel channel]
        {
            get
            {
                return BitConverter.GetBytes(t_color)[(int)channel];
            }

            set
            {
                NewColor(value, channel);
            }
        }*/

        /// <summary>
        /// Returns a System.Drawing.Color from the BGRA values contained in the current cell
        /// </summary>
        public System.Drawing.Color Color {
            get
            {
                return System.Drawing.Color.FromArgb(t_alpha, t_red, t_green, t_blue);
                //return System.Drawing.Color.FromArgb(Int);
            }
        }

        /// <summary>
        /// Returns an int value representing the B G R A values. Individual byte values can be extracted using the Cell ctor.
        /// </summary>
        public int Int
        {
            get
            {
                int t_color = BitConverter.ToInt32(new byte[] { t_blue, t_green, t_red, t_alpha }, 0);

                return t_color;
            }
        }

        /// <summary>
        /// Checks for color equality without considering the alpha channel.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsColorEqual_NoAlpha(Engine.Color.Cell c) {
            return (this.Blue == c.Blue && this.Green == c.Green && this.Red == c.Red);
        }

        /// <summary>
        /// Compares two Cell objects including their alpha values
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>Returns also false if 2 objects are null</returns>
        public static bool operator ==(Engine.Color.Cell c1, Engine.Color.Cell c2) {
            if (object.ReferenceEquals(c1, null)) {
                if (object.ReferenceEquals(c2, null)) {
                    return true;
                }
            }

            if (object.ReferenceEquals(c2, null)) {
                // well, the other too is null
                return false;
            }

            return (c1.Int == c2.Int);
        }

        /// <summary>
        /// Compares two Cell objects including their alpha values
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Engine.Color.Cell c1, Engine.Color.Cell c2)
        {
            if (object.ReferenceEquals(c1, null))
            {
                if (object.ReferenceEquals(c2, null))
                {
                    return false;
                }
            }

            if (object.ReferenceEquals(c2, null))
            {
                // well, the other too is null
                return true;
            }

            return (c1.Int != c2.Int);
        }

        public override bool Equals(object obj)
        {
            return (Cell)obj == this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Int;
            }
        }

        public static Engine.Color.Cell ShadeOfGray(byte intensity)
        {
            return new Color.Cell(intensity, intensity, intensity, Engine.ColorOpacity.Opaque);
        }

        public static Engine.Color.Cell Noise()
        {
            byte level = (byte)Engine.Calc.Math.Rand.Next(255);

            return Engine.Color.Cell.ShadeOfGray(level);
        }

        public static Engine.Color.Cell RandomColorRandomAlpha()
        {
            return new Cell((byte)Engine.Calc.Math.Rand.Next(255), (byte)Engine.Calc.Math.Rand.Next(255), (byte)Engine.Calc.Math.Rand.Next(255), (byte)Engine.Calc.Math.Rand.Next(255));
        }

        public static Engine.Color.Cell RandomColorOpaque()
        {
            return new Cell((byte)Engine.Calc.Math.Rand.Next(255), (byte)Engine.Calc.Math.Rand.Next(255), (byte)Engine.Calc.Math.Rand.Next(255), Engine.ColorOpacity.Opaque);
        }
    }
}
