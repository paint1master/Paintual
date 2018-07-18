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
using System.Windows;
using System.Windows.Input;

namespace Engine
{
    public struct Size
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public struct MousePoint
    {
        public int X;
        public int Y;

        public Engine.MouseActionType MouseAction;

        public bool ActualMousePoint;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
            MouseAction = MouseActionType.Undefined;
            ActualMousePoint = false;
        }

        public MousePoint(int x, int y, MouseActionType actionType) : this(x, y)
        {
            MouseAction = actionType;
        }

        public MousePoint(double x, double y, MouseActionType actionType)
        {
            X = (int)x;
            Y = (int)y;
            MouseAction = actionType;
            ActualMousePoint = false;
        }

        public MousePoint(int x, int y, MouseActionType actionType, bool actualMousePoint) : this(x, y, actionType)
        {
            ActualMousePoint = actualMousePoint;
        }
    }

    public struct Rectangle
    {
        public Point Point;
        public Size Size;

        public Rectangle(int x, int y, int width, int height)
        {
            Point = new Engine.Point(x, y);
            Size = new Engine.Size(width, height);
        }

        public Rectangle(double x, double y, double width, double height)
        {
            Point = new Engine.Point(Convert.ToInt32(x), Convert.ToInt32(y));
            Size = new Engine.Size(Convert.ToInt32(width), Convert.ToInt32(height));
        }

        public System.Windows.Rect ToWindowsRectangle()
        {
            Rect r = new Rect(new System.Windows.Point(Point.X, Point.Y), new System.Windows.Size(Size.Width, Size.Height));

            return r;
        }
    }

    public enum MouseActionType
    {
        MouseDown,
        MouseMove,
        MouseUp,
        MouseClick,
        Undefined
    }

    /// <summary>
    /// for now it is identical to Engine.MousePoint struct
    /// </summary>
    public struct MouseEventArgs
    {
        public MousePoint MousePoint;

        public MouseEventArgs(int x, int y)
        {
            MousePoint = new MousePoint(x, y, MouseActionType.Undefined);
        }

        public MouseEventArgs(int x, int y, Engine.MouseActionType action)
        {
            MousePoint = new MousePoint(x, y, action);
        }
    }
}
