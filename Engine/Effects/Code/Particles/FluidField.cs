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

using Engine.Surface;

namespace Engine.Effects.Particles
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>not used anymore ?</remarks>
    public class FluidField
    {
        private double[][] t_grid;

        public FluidField(int width, int height)
        {
            SetField(width, height);
        }

        private void SetField(int width, int height)
        {
            t_grid = new double[width][];

            for (int i = 0; i < t_grid.Length; i++)
            {
                t_grid[i] = new double[height];
            }

            for (int y = 0; y < t_grid[0].Length; y++)
            {
                for (int x = 0; x < t_grid.Length; x++)
                {
                    t_grid[x][y] = 0;
                }
            }
        }

        public double GetFluidAmount(int x, int y, PixelRetrievalOptions option)
        {
            if (x >= 0 && x < t_grid.Length && y >= 0 && y < t_grid[0].Length)
            {
                return t_grid[x][y];
            }

            switch (option)
            {
                case PixelRetrievalOptions.RaiseError:

                    throw new ArgumentOutOfRangeException(String.Format("In FluidField.GetFluidAmount(), requested pixel {0}{1} is outside image bounds.", x, y));

                case PixelRetrievalOptions.ReturnDefaultBlack:
                    return 0;

                case PixelRetrievalOptions.ReturnEdgePixel:
                    int tempX = 0, tempY = 0;

                    if (x < 0) { tempX = 0; }
                    if (x >= t_grid.Length) { tempX = t_grid.Length - 1; }
                    if (y < 0) { tempY = 0; }
                    if (y >= t_grid[0].Length) { tempY = t_grid[0].Length - 1; }

                    return t_grid[tempX][tempY];

                default:

                    throw new ArgumentOutOfRangeException(String.Format("In FluidField.GetFluidAmount(), PixelRetrievalOption {0} is not supported.", option.ToString()));
            }
        }

        public void SetFluidAmount(double amount, int x, int y, PixelSetOptions option)
        {
            if (x >= 0 && x < t_grid.Length && y >= 0 && y < t_grid[0].Length)
            {
                t_grid[x][y] = amount;
                return;
            }

            switch (option)
            {
                case PixelSetOptions.Ignore:
                    return;

                case PixelSetOptions.RaiseError:
                    throw new ArgumentOutOfRangeException(String.Format("In FluidField.SetFluidAmount(), location {0}:{1} is outside image bounds.", x, y));

                default:

                    throw new ArgumentOutOfRangeException(String.Format("In FluidField.SetFluidAmount(), PixelSetOption {0} is not supported.", option.ToString()));
            }
        }

        public bool IsOutOfBounds(int x, int y)
        {
            if (x >= 0 && x < t_grid.Length && y >= 0 && y < t_grid[0].Length)
            {
                return false;
            }

            return true;
        }
    }
}

