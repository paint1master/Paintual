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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Provides default values for all tools and attributes. Prevents any null scenarios.
    /// </summary>
    public class DefaultValues
    {
        private Engine.Surface.Canvas defaultImageForBrush;
        private bool t_defaultImageForBrushLoaded = false;

        // motion related values
        private bool interpolateMouseMoves;

        internal DefaultValues()
        {
            interpolateMouseMoves = true;
        }

        /*public Engine.Surface.Canvas DefaultImageForBrush
        {
            get
            {
                if (this.t_defaultImageForBrushLoaded == false)
                {
                    string location2 = Engine.Application.Prefs.ApplicationDataPath + Engine.Application.Prefs.Brushes;

                    System.IO.DirectoryInfo di2 = new System.IO.DirectoryInfo(location2);

                    foreach (System.IO.FileInfo fi in di2.GetFiles("*.png"))
                    {
                        Bitmap b = new Bitmap(fi.FullName);
                        this.defaultImageForBrush = Engine.Surface.Ops.BitmapToCanvas(b);

                        break;
                    }
                }

                t_defaultImageForBrushLoaded = true;
                return this.defaultImageForBrush;
            }
        }*/

        /*public Engine.Color.Cell DefaultPaintColor
        {
            get
            {
                return Engine.Colors.Black;
            }
        }*/

        public System.Drawing.Color DefaultTint
        {
            get
            {
                return System.Drawing.Color.FromArgb(127, 0, 0, 0);
            }
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>The tool needs to be initialized with the current workflow.</remarks>
        public Engine.Tools.Tool DefaultTool
        {
            get
            {
                Engine.Tools.ImagesBrushTool tool = new Engine.Tools.ImagesBrushTool();
                return tool;
            }
        }*/

        public bool InterpolateMouseMoves
        {
            get { return this.interpolateMouseMoves; }
        }
    }
}

