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
    [TemplatePart(Name = "C_RadH"  , Type= typeof(RadioButton))]
    [TemplatePart(Name = "C_RadS"  , Type= typeof(RadioButton))]
    [TemplatePart(Name = "C_RadV"  , Type= typeof(RadioButton))]
    [TemplatePart(Name = "C_TxtH"  , Type= typeof(TextBox))]
    [TemplatePart(Name = "C_TxtS"  , Type= typeof(TextBox))]
    [TemplatePart(Name = "C_TxtV"  , Type= typeof(TextBox))]
    [TemplatePart(Name = "C_RadR"  , Type= typeof(RadioButton))]
    [TemplatePart(Name = "C_RadG"  , Type= typeof(RadioButton))]
    [TemplatePart(Name = "C_RadB"  , Type= typeof(RadioButton))]
    [TemplatePart(Name = "C_TxtR"  , Type= typeof(TextBox))]
    [TemplatePart(Name = "C_TxtG"  , Type= typeof(TextBox))]
    [TemplatePart(Name = "C_TxtB"  , Type= typeof(TextBox))]
    [TemplatePart(Name = "C_Plane" , Type= typeof(PaintualUI.Controls.ColorPicker.ColorPlane))]
    [TemplatePart(Name = "C_Fader" , Type= typeof(PaintualUI.Controls.ColorPicker.ColorFader))]
    [TemplatePart(Name = "C_RectSample", Type = typeof(System.Windows.Shapes.Rectangle))]
    public class TColorPickerStandard : Control
    {
        static TColorPickerStandard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TColorPickerStandard), new FrameworkPropertyMetadata(typeof(TColorPickerStandard)));
        }

        protected RadioButton C_RadH;
        protected RadioButton C_RadS;
        protected RadioButton C_RadV;
        protected TextBox C_TxtH;
        protected TextBox C_TxtS;
        protected TextBox C_TxtV;
        protected RadioButton C_RadR;
        protected RadioButton C_RadG;
        protected RadioButton C_RadB;
        protected TextBox C_TxtR;
        protected TextBox C_TxtG;
        protected TextBox C_TxtB;
        protected PaintualUI.Controls.ColorPicker.ColorPlane C_Plane;
        protected PaintualUI.Controls.ColorPicker.ColorFader C_Fader;
        protected System.Windows.Shapes.Rectangle C_RectSample;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            C_RadH = Template.FindName("C_RadH", this) as RadioButton;
            C_RadS = Template.FindName("C_RadS", this) as RadioButton;
            C_RadV = Template.FindName("C_RadV", this) as RadioButton;
            C_TxtH = Template.FindName("C_TxtH", this) as TextBox;
            C_TxtS = Template.FindName("C_TxtS", this) as TextBox;
            C_TxtV = Template.FindName("C_TxtV", this) as TextBox;
            C_RadR = Template.FindName("C_RadR", this) as RadioButton;
            C_RadG = Template.FindName("C_RadG", this) as RadioButton;
            C_RadB = Template.FindName("C_RadB", this) as RadioButton;
            C_TxtR = Template.FindName("C_TxtR", this) as TextBox;
            C_TxtG = Template.FindName("C_TxtG", this) as TextBox;
            C_TxtB = Template.FindName("C_TxtB", this) as TextBox;
            C_Plane = Template.FindName("C_Plane", this) as PaintualUI.Controls.ColorPicker.ColorPlane;
            C_Fader = Template.FindName("C_Fader", this) as PaintualUI.Controls.ColorPicker.ColorFader;
            C_RectSample = Template.FindName("C_RectSample", this) as System.Windows.Shapes.Rectangle;

            C_Fader.ColorChanged += Fader_ColorChanged;
            C_Plane.ColorChanged += C_Plane_ColorChanged;
        }

        private void C_Plane_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            C_RectSample.Fill = new SolidColorBrush(Color.FromArgb(Engine.ColorOpacity.Opaque, e.NewColor.Red, e.NewColor.Green, e.NewColor.Blue));
            Engine.Application.UISelectedValues.SelectedColor = e.NewColor;
        }

        private void Fader_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            C_Plane.UpdatePlaneImage(e.NewColor);

            Engine.Color.Models.HSV hsv = Engine.Color.Models.HSV.FromRGB(new Engine.Color.Models.RGBdouble(e.NewColor));

            C_TxtH.Text = Engine.Calc.Math.Double_0_1_ToDegree(hsv.H).ToString();
            C_TxtS.Text = hsv.S.ToString();
            C_TxtV.Text = hsv.V.ToString();
            
            C_TxtR.Text = e.NewColor.Red.ToString();
            C_TxtG.Text = e.NewColor.Green.ToString();
            C_TxtB.Text = e.NewColor.Blue.ToString();
        }
    }
}
