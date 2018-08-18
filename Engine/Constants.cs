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

namespace Engine
{
    /// <summary>
    /// Holds standardized index position of channel values inside a four position array.
    /// </summary>
    public enum Channel
    {
        Blue = 0,
        Green = 1,
        Red = 2,
        Alpha = 3
    }

    public static class ColorOpacity
    {
        /// <summary>
        /// An alpha channel value equals to 255;
        /// </summary>
        public static readonly byte Opaque = 255;
        public static readonly byte Transparent = 0;
        public static readonly byte MidRange = 128;
    }

    public static class BytesPerPixel
    {
        /// <summary>
        /// the number of bytes to form 
        /// </summary>
        public static readonly int BGRA = 4;
        public static readonly int HSV = 3;
    }

    public enum PropertyDataTypes
    {
        Undefined,
        Object,
        Int,
        Double,
        Text,
        Enum,
        Boolean
    }
}
