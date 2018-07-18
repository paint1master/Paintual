using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine;

namespace PaintualUI.Controls.ColorPicker
{
    public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs e);

    public class ColorChangedEventArgs : EventArgs
    {
        public Engine.Color.Cell NewColor;

        public ColorChangedEventArgs(Engine.Color.Cell newColor)
        {
            NewColor = newColor;
        }
    }
}
