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

using Engine.Color;

namespace Engine
{
    public static class Colors
    {
        #region static_standard_colors
        public static Engine.Color.Cell Black
        {
            get { return new Cell(0, 0, 0, Engine.ColorOpacity.Opaque); }
        }

        public static Engine.Color.Cell Gray
        {
            get { return new Cell(128, 128, 128, Engine.ColorOpacity.Opaque); }
        }

        public static Engine.Color.Cell White
        {
            get { return new Cell(255, 255, 255, Engine.ColorOpacity.Opaque); }
        }

        public static Engine.Color.Cell Yellow
        {
            get { return new Cell(0, 255, 255, Engine.ColorOpacity.Opaque); }
        }

        public static Engine.Color.Cell Purple
        {
            get { return new Cell(128, 0, 128, Engine.ColorOpacity.Opaque); }
        }

        public static Engine.Color.Cell Teal
        {
            get { return new Cell(128, 128, 0, Engine.ColorOpacity.Opaque); }
        }

        /// <summary>
        /// not the true orange, but a nice one
        /// </summary>
        public static Engine.Color.Cell Orange
        {
            get { return new Cell(65, 176, 245, Engine.ColorOpacity.Opaque); }
        }
        #endregion
    }
}