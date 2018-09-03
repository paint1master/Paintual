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

namespace Engine.Surface
{
    public struct MaskValue
    {
        public static readonly byte BlackReveal = 0;
        public static readonly byte Gray = 128;
        public static readonly byte WhiteHide = 255;
    }


    public class Mask
    {
        private int t_width;
        private int t_height;

        private byte[][] t_grid;

        public Mask(int width, int height)
        {
            t_width = width;
            t_height = height;

            InitializeArrays();
        }

        public Mask(int width, int height, byte maskValue) : this(width, height)
        {
            InitializeArrays(maskValue);
        }

        private void InitializeArrays()
        {
            t_grid = new byte[Width][];

            for (int i = 0; i < t_grid.Length; i++)
            {
                t_grid[i] = new byte[Height];
            }
        }

        private void InitializeArrays(byte maskValue)
        {
            t_grid = new byte[Width][];

            for (int i = 0; i < t_grid.Length; i++)
            {
                t_grid[i] = new byte[Height];

                for (int j = 0; j < t_grid[i].Length; j++)
                {
                    t_grid[i][j] = maskValue;
                }
            }
        }

        public byte GetPixel(int x, int y, PixelRetrievalOptions option)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return t_grid[x][y];
            }

            switch (option)
            {
                case PixelRetrievalOptions.RaiseError:

                    throw new ArgumentOutOfRangeException(String.Format("In Mask.GetPixel(), requested pixel {0}:{1} is outside image bounds.", x, y));

                case PixelRetrievalOptions.ReturnDefaultBlack:
                    return 0;

                case PixelRetrievalOptions.ReturnNeutralGray:
                    return 128;

                case PixelRetrievalOptions.ReturnWhite:
                    return 255;

                case PixelRetrievalOptions.ReturnEdgePixel:
                    int tempX = 0, tempY = 0;

                    if (x < 0) { tempX = 0; }
                    if (x >= Width) { tempX = Width - 1; }
                    if (y < 0) { tempY = 0; }
                    if (y >= Height) { tempY = Height - 1; }

                    return t_grid[tempX][tempY];

                default:

                    throw new ArgumentOutOfRangeException(String.Format("In Mask.GetPixel(), PixelRetrievalOption {0} is not supported.", option.ToString()));
            }
        }

        public void SetPixel(byte c, int x, int y, PixelSetOptions option)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                t_grid[x][y] = c;
                return;
            }

            switch (option)
            {
                case PixelSetOptions.Ignore:
                    return;

                case PixelSetOptions.RaiseError:
                    throw new ArgumentOutOfRangeException(String.Format("In Mask.SetPixel(), requested pixel {0}{1} is outside image bounds.", x, y));

                default:

                    throw new ArgumentOutOfRangeException(String.Format("In Mask.SetPixel(), PixelSetOption {0} is not supported.", option.ToString()));
            }
        }

        public bool IsOutOfBounds(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return false;
            }

            return true;
        }

        public int Width { get => t_width; }
        public int Height { get => t_height; }
    }
}

