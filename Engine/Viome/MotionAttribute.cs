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
    public class MotionAttribute
    {
        private Engine.Viome t_viom;

        private List<Engine.MousePoint> t_mousePoints;

        public MotionAttribute(Engine.Viome w)
        {
            t_viom = w;
            t_mousePoints = new List<Engine.MousePoint>();
        }

        public int AddMousePoint(MousePoint p)
        {
            if (Engine.Application.UISelectedValues.InterpolateMouseMoves)
            {
                t_mousePoints.Add(p);
            }

            // required for BackgroundQueue
            return 0;
        }

        private void OnlyKeepLastMousePoint()
        {
            if (t_mousePoints.Count <= 1)
            {
                return;
            }

            MousePoint last = t_mousePoints[t_mousePoints.Count - 1];
            t_mousePoints.Clear();
            t_mousePoints.Add(last);
        }

        public void Clear()
        {
            t_mousePoints.Clear();
        }

        public List<MousePoint> LinearInterpolate()
        {
            List<MousePoint> points = new List<MousePoint>();

            if (Engine.Application.UISelectedValues.InterpolateMouseMoves == false)
            {
                OnlyKeepLastMousePoint();
                points.Add(t_mousePoints[0]);
                return points;
            }

            if (this.t_mousePoints.Count > 1)
            {
                int X1 = this.t_mousePoints[0].X;
                int Y1 = this.t_mousePoints[0].Y;
                int X2 = this.t_mousePoints[this.t_mousePoints.Count - 1].X;
                int Y2 = this.t_mousePoints[this.t_mousePoints.Count - 1].Y;

                int deltaX = Math.Abs(X2 - X1);
                int deltaY = Math.Abs(Y2 - Y1);

                int maxSteps = Math.Max(deltaX, deltaY);

                if (maxSteps <= 1)
                {
                    OnlyKeepLastMousePoint();
                    points.Add(t_mousePoints[0]);
                    return points;
                }

                deltaX = X2 - X1;
                deltaY = Y2 - Y1;

                float increaseX = (float)deltaX / (float)maxSteps;
                float increaseY = (float)deltaY / (float)maxSteps;

                points.Add(new Engine.MousePoint(X1, Y1, Engine.MouseActionType.MouseMove, true));

                for (int i = 0; i < maxSteps; i++)
                {
                    int stepX = (int)((i * increaseX) + (float)X1);
                    int stepY = (int)((i * increaseY) + (float)Y1);

                    // have these coordinates modified to take into account zoom and pan
                    MousePoint p2 = new Engine.MousePoint(stepX, stepY, Engine.MouseActionType.MouseMove, false);
                    points.Add(p2);
                }

                points.Add(new Engine.MousePoint(X2, Y2, Engine.MouseActionType.MouseMove, true));
            }
            else
            {
                points.Add(new Engine.MousePoint(t_mousePoints[0].X, t_mousePoints[0].Y));
            }

            OnlyKeepLastMousePoint();
            return points;
        }

        private void CubicSplineInterpolate(MousePoint p)
        {
            // for cubic spline interpolation
            /*if (this.mousePoints.Count == 3)
            {

                // we have three points, we can interpolate
                // but only if the distance between those three points is larger
                // than the size of the image brush

            }*/
        }

        public int PointCount
        {
            get { return t_mousePoints.Count; }
        }
    }
}
