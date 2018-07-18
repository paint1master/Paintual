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
    public class Color
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        /// <param name="steps">The number of color cell to generate a gradient starting with the startColor and ending with the endColor.</param>
        /// <remarks>The returned buffer will have a length of steps 8 * 4 (argb).
        /// Also the alpha channel is affected by the calculations.</remarks>
        /// <returns></returns>
        public static int[] GenerateLinearGradient(Engine.Color.Cell startColor, Engine.Color.Cell endColor, int steps)
        {
            int[] stride = new int[steps];

            int deltaRed = 0, deltaGreen = 0, deltaBlue = 0, deltaAlpha = 0;
            byte newRed = 0, newGreen = 0, newBlue = 0, newAlpha = 0;

            for (int i = 0; i < steps; i++)
            {
                deltaBlue = (endColor.Blue - startColor.Blue) * i / steps;
                deltaGreen = (endColor.Green - startColor.Green) * i / steps;
                deltaRed = (endColor.Red - startColor.Red) * i / steps;
                deltaAlpha = (endColor.Alpha - startColor.Alpha) * i / steps;

                newBlue = (byte)(deltaBlue + startColor.Blue % 256);
                newGreen = (byte)(deltaGreen + startColor.Green % 256);
                newRed = (byte)(deltaRed + startColor.Red % 256);
                newAlpha = (byte)(deltaAlpha + startColor.Alpha % 256);

                Engine.Color.Cell c = new Engine.Color.Cell(newBlue, newGreen, newRed, newAlpha);
                c.WriteInt(stride, i);
            }

            return stride;
        }

        public static Engine.Color.Models.HSV[] GenerateLinearGradient(Engine.Color.Models.HSV startColor, Engine.Color.Models.HSV endColor, int steps)
        {
            Engine.Color.Models.HSV[] range = new Engine.Color.Models.HSV[steps];

            double deltaH = endColor.H - startColor.H;
            double deltaS = endColor.S - startColor.S;
            double deltaV = endColor.V - startColor.V;

            for (int i = 0; i < steps; i++)
            {
                double div = (double)i / (double)steps;

                double h = startColor.H + deltaH * div;
                double s = startColor.S + deltaS * div;
                double v = startColor.V + deltaV * div;

                range[i] = new Engine.Color.Models.HSV(h, s, v);
            }

            return range;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="upperLeft"></param>
        /// <param name="upperRight"></param>
        /// <param name="lowerLeft"></param>
        /// <param name="lowerRight"></param>
        /// <param name="width">The width of the resulting grid including the provided corners.</param>
        /// <param name="height">The height of the resulting grid including the provided corners.</param>
        /// <returns></returns>
        public static int[,] GenerateQuadGradient(Engine.Color.Cell upperLeft, Engine.Color.Cell upperRight, Engine.Color.Cell lowerLeft, Engine.Color.Cell lowerRight, int width, int height)
        {
            int[,] grid = new int[width, height];

            // first row
            int[] firstRow = GenerateLinearGradient(upperLeft, upperRight, width);

            for (int i = 0; i < width; i++)
            {
                Engine.Color.Cell c = new Engine.Color.Cell(firstRow, i);
                grid[i, 0] = c.Int;
            }


            // last row
            int[] lastRow = GenerateLinearGradient(lowerLeft, lowerRight, width);

            for (int i = 0; i < width; i++)
            {
                Engine.Color.Cell c = new Engine.Color.Cell(lastRow, i);
                grid[i, height - 1] = c.Int;
            }


            // rows in between : first column
            int[] firstColumn = GenerateLinearGradient(upperLeft, lowerLeft, height);

            // rows in between : last column
            int[] lastColumn = GenerateLinearGradient(upperRight, lowerRight, height);


            // rows in between : fill
            byte[,] middle = new byte[width, (height - 2)];

            for (int y = 1; y < height - 1; y++)
            {
                int[] middleRow = GenerateLinearGradient(new Engine.Color.Cell(firstColumn, y), new Engine.Color.Cell(lastColumn, y), width);
                for (int x = 0; x < width; x++)
                {
                    Engine.Color.Cell c = new Engine.Color.Cell(middleRow, x);
                    grid[x, y] = c.Int;
                }
            }

            return grid;
        }
    }
}
