using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Surface
{
    public enum PixelRetrievalOptions
    {
        ReturnDefaultBlack,
        ReturnEdgePixel,
        RaiseError
    }

    public enum PixelSetOptions
    {
        Ignore,
        RaiseError        
    }

    public class ImageDataGrid
    {
        private ImageData t_imageData;

        private int[][]t_grid;

        public ImageDataGrid(ref ImageData idata)
        {
            t_imageData = idata;

            MapOffsets();
        }

        private void MapOffsets()
        {
            t_grid = new int[Width][];

            for (int i = 0; i < t_grid.Length; i++)
            {
                t_grid[i] = new int[Height];
            }

            int offset = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    t_grid[x][y] = offset;

                    offset += Engine.BytesPerPixel.BGRA;
                }
            }
        }

        public Engine.Color.Cell GetPixel(int x, int y, PixelRetrievalOptions option)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return new Engine.Color.Cell(t_imageData.Array, t_grid[x][y]);
            }

            switch (option)
            {
                case PixelRetrievalOptions.RaiseError:

                    throw new ArgumentOutOfRangeException(String.Format("In ImageDataGrid.GetPixel(), requested pixel {0}{1} is outside image bounds.", x, y));

                case PixelRetrievalOptions.ReturnDefaultBlack:
                    return Engine.Colors.Black;

                case PixelRetrievalOptions.ReturnEdgePixel:
                    int tempX = 0, tempY = 0;

                    if (x < 0) { tempX = 0; }
                    if (x >= Width) { tempX = Width - 1; }
                    if (y < 0) { tempY = 0; }
                    if (y >= Height) { tempY = Height - 1; }

                    return new Engine.Color.Cell(t_imageData.Array, t_grid[tempX][tempY]);

                default:

                    throw new ArgumentOutOfRangeException(String.Format("In ImageDataGrid.GetPixel(), PixelRetrievalOption {0} is not supported.", option.ToString()));
            }
        }

        public void SetPixel(Engine.Color.Cell c, int x, int y, PixelSetOptions option)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                c.WriteBytes(t_imageData.Array, t_grid[x][y]);
                return;
            }

            switch (option)
            {
                case PixelSetOptions.Ignore:
                    return;

                case PixelSetOptions.RaiseError:
                    throw new ArgumentOutOfRangeException(String.Format("In ImageDataGrid.SetPixel(), requested pixel {0}{1} is outside image bounds.", x, y));

                default:

                    throw new ArgumentOutOfRangeException(String.Format("In ImageDataGrid.SetPixel(), PixelSetOption {0} is not supported.", option.ToString()));
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

        public int Width { get => t_imageData.Width; }
        public int Height { get => t_imageData.Height; }
    }
}
