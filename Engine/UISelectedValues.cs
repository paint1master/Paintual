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
    /// Holds various selections made by the UI
    /// </summary>
    public class UISelectedValues
    {
        //private Engine.Color.Cell t_selectedColor;
        //private Engine.Tools.Attributes.IAttribute m_selectedBrushImage;
        //private Type m_selectedTool;

        // motion related values
        private bool interpolateMouseMoves;

        internal UISelectedValues()
        {
            interpolateMouseMoves = Engine.Application.DefaultValues.InterpolateMouseMoves; // default value is true
        }

        public bool InterpolateMouseMoves
        {
            get { return this.interpolateMouseMoves; }
        }

        /*public Type SelectedTool
        {
            get { return this.m_selectedTool; }
            set
            {
                this.m_selectedTool = value;

                RaiseToolSelected(new UISelectedValuesEventArgs(this.m_selectedTool));
            }
        }*/

        /*public Engine.Tools.Attributes.IAttribute SelectedBrushImage
        {
            get
            {
                if (this.m_selectedBrushImage == null)
                {
                    Engine.Surface.Canvas img = Engine.Application.DefaultValues.DefaultImageForBrush;

                    Engine.Tools.Attributes.ImageAttribute aImg = new Tools.Attributes.ImageAttribute(img);
                    this.m_selectedBrushImage = aImg;
                }
                return this.m_selectedBrushImage;
            }
            set
            {
                this.m_selectedBrushImage = value;

                RaiseBrushImageSelected(new UISelectedValuesEventArgs(this.m_selectedBrushImage));
            }
        }*/

        /*public Engine.Color.Cell SelectedColor
        {
            get
            {
                // the default values for System.Drawing.Color is 0 0 0 0 so the A channel makes it invisible
                if (this.t_selectedColor == null)
                {
                    this.t_selectedColor = Engine.Colors.Black;
                    return this.t_selectedColor;
                }

                if (this.t_selectedColor.Red == 0 &&
                    this.t_selectedColor.Green == 0 &&
                    this.t_selectedColor.Blue == 0 &&
                    this.t_selectedColor.Alpha == 0)
                {
                    this.t_selectedColor = Engine.Colors.Black;
                }

                return this.t_selectedColor;
            }

            set { this.t_selectedColor = value; }
        }*/

        /*public event UISelectedValuesEventHandler BrushImageSelected;

        public void RaiseBrushImageSelected(UISelectedValuesEventArgs e)
        {
            if (BrushImageSelected != null)
                BrushImageSelected(e);
        }

        public event UISelectedValuesEventHandler ToolSelected;

        public void RaiseToolSelected(UISelectedValuesEventArgs e)
        {
            if (ToolSelected != null)
            {
                ToolSelected(e);
            }
        }*/
    }

    /*public delegate void UISelectedValuesEventHandler(UISelectedValuesEventArgs e);

    public class UISelectedValuesEventArgs
    {

        private Engine.Tools.Attributes.IAttribute m_brushImage;
        private Type m_selectedTool;

        public UISelectedValuesEventArgs()
        {

        }

        public UISelectedValuesEventArgs(Engine.Tools.Attributes.IAttribute brushImage)
        {
            this.m_brushImage = brushImage;
        }

        public UISelectedValuesEventArgs(Type selectedTool)
        {
            this.m_selectedTool = selectedTool;
        }

        public Engine.Tools.Attributes.IAttribute BrushImage
        {
            get { return this.m_brushImage; }
        }

        public Type Tool
        {
            get { return this.m_selectedTool; }
        }
    }*/
}
