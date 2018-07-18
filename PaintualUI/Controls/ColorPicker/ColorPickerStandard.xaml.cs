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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintualUI.Controls.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPickerStandard.xaml
    /// </summary>
    public partial class ColorPickerStandard : UserControl
    {
        public ColorPickerStandard()
        {
            InitializeComponent();

            this.Fader.ColorChanged += Fader_ColorChanged;
        }

        private void Fader_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            this.Plane.UpdatePlaneImage(e.NewColor);

            Engine.Color.Models.HSV hsv = Engine.Color.Models.HSV.FromRGB(new Engine.Color.Models.RGBdouble(e.NewColor));

            this.TxtH.Text = Engine.Calc.Math.Double_0_1_ToDegree(hsv.H).ToString();
            this.TxtS.Text = hsv.S.ToString();
            this.TxtV.Text = hsv.V.ToString();

            this.TxtR.Text = e.NewColor.Red.ToString();
            this.TxtG.Text = e.NewColor.Green.ToString();
            this.TxtB.Text = e.NewColor.Blue.ToString();
        }
    }
}
