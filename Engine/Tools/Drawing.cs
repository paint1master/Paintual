using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Tools
{
    public static class Drawing
    {

        public static void DrawLine(Engine.Surface.Canvas canvas, Point start, Point end, Engine.Color.Cell color)
        {
            Engine.MousePoint pStart = new MousePoint(start.X, start.Y, MouseActionType.Undefined);
            Engine.MousePoint pEnd = new MousePoint(end.X, end.Y, MouseActionType.Undefined);
            List<MousePoint> lst =  Engine.Calc.Math.LinearInterpolate(pStart, pEnd);

            foreach (MousePoint p in lst)
            {
                canvas.SetPixel(color, p.X, p.Y, Surface.PixelSetOptions.Ignore);
            }
        }

        /// <summary>
        /// Draws a filled circle at the specified coordinates using the specified color.
        /// </summary>
        /// <param name="canvas">the target image onto which to draw a circle</param>
        /// <param name="position">where on the target image the center of the cicle is to be located.</param>
        /// <param name="size">the diameter of the circle to draw</param>
        /// <param name="color">the color of the circle line</param>
        /// <remarks>Implements the Bresenham's circle argorithm, most of the code found at : https://rosettacode.org/wiki/Bitmap/Midpoint_circle_algorithm#C.23 </remarks>
        public static void DrawCircle(Engine.Surface.Canvas canvas, Engine.Calc.Vector position, int size, Engine.Color.Cell color)
        {
            int centerX = (int)System.Math.Round(position.X);
            int centerY = (int)System.Math.Round(position.Y);

            int radius = (int)System.Math.Round((double)size / 2);

            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {
                // ensure index is in range before setting (depends on your image implementation)
                // in this case we check if the pixel location is within the bounds of the image before setting the pixel

                canvas.SetPixel(color, centerX + x, centerY + y, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX + x, centerY - y, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX - x, centerY + y, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX - x, centerY - y, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX + y, centerY + x, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX + y, centerY - x, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX - y, centerY + x, Surface.PixelSetOptions.Ignore);
                canvas.SetPixel(color, centerX - y, centerY - x, Surface.PixelSetOptions.Ignore);

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);
        }
    }
}
