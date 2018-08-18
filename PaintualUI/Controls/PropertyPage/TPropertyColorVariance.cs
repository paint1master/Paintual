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

namespace PaintualUI.Controls.PropertyPage
{
    [TemplatePart(Name = "C_SliderFrequency", Type = typeof(Slider))]
    [TemplatePart(Name = "C_SliderRange", Type = typeof(Slider))]
    [TemplatePart(Name = "C_LblFrequency", Type = typeof(Label))]
    [TemplatePart(Name = "C_LblRange", Type = typeof(Label))]
    [TemplatePart(Name = "C_LblFrequencyValue", Type = typeof(Label))]
    [TemplatePart(Name = "C_LblRangeValue", Type = typeof(Label))]
    public class TPropertyColorVariance : PaintualUI.Controls.PropertyPage.TPropertyControl
    {
        protected Slider C_SliderFrequency;
        protected Slider C_SliderRange;
        protected Label C_LblFrequency;
        protected Label C_LblRange;
        protected Label C_LblFrequencyValue;
        protected Label C_LblRangeValue;

        static TPropertyColorVariance()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyColorVariance), new FrameworkPropertyMetadata(typeof(TPropertyColorVariance)));
        }

        private Engine.Color.ColorVariance t_colorVariance;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> required to get the validators list instantiated in TPropertyControl parent class</remarks>
        public TPropertyColorVariance() : base()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            C_SliderFrequency = Template.FindName("C_SliderFrequency", this) as Slider;
            C_SliderRange = Template.FindName("C_SliderRange", this) as Slider;
            C_LblFrequency = Template.FindName("C_LblFrequency", this) as Label;
            C_LblRange = Template.FindName("C_LblRange", this) as Label;
            C_LblFrequencyValue = Template.FindName("C_LblFrequencyValue", this) as Label;
            C_LblRangeValue = Template.FindName("C_LblRangeValue", this) as Label;

            Engine.Application.UISelectedValues.ColorSelected += UISelectedValues_ColorSelected;

            C_SliderFrequency.ValueChanged += C_SliderFrequency_ValueChanged;
            C_SliderRange.ValueChanged += C_SliderRange_ValueChanged;

            C_LblRangeValue.Content = C_SliderRange.Value;
            C_LblFrequencyValue.Content = C_SliderFrequency.Value;
        }

        private void C_SliderRange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (t_colorVariance != null)
            {
                t_colorVariance.SetRanges((byte)e.NewValue);
                C_LblRangeValue.Content = (byte)e.NewValue;
            }
        }

        private void C_SliderFrequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (t_colorVariance != null)
            {
                // if double value is, for example, 0.56, casting to int gives 0 which is not allowed. Adding 1 solves the problem.
                t_colorVariance.SetFrequencies((int)e.NewValue + 1);
                C_LblFrequencyValue.Content = (int)e.NewValue;
            }
        }

        private void UISelectedValues_ColorSelected(Engine.UISelectedValuesEventArgs e)
        {
            if (t_colorVariance != null)
            {
                t_colorVariance.SetColor(e.Color);
            }
        }

        #region TIPropertyControl implementation

        public override void BuildControl(Engine.Effects.VisualPropertyItem pi)
        {
            Name = pi.ActualPropertyName;
            PropertyName = pi.ActualPropertyName;
            LabelText = pi.DisplayName;
            DataType = pi.DataType;

            t_colorVariance = new Engine.Color.ColorVariance(Engine.Application.UISelectedValues.SelectedColor);
        }

        /// <summary>
        /// Use this method to build controls dynamically (ie radio button list based on a property).
        /// </summary>
        public override void BuildVisual()
        {
            /// visual building of controls automatically performed by XAML code from template, see OnApplyTemplate().
            base.BuildVisual();
        }

        /// <summary>
        /// Updates the content of visual controls (textbox, drop lists, etc) to display values set by the engine.
        /// </summary>
        public override void UpdateVisual()
        {


            //C_Label.Content = LabelText;

            /*if (DefaultValue != null)
            {
                string defVal = (string)DefaultValue;
                C_TextBox.Text = defVal;
            }*/
        }

        public override void SignalError(string message)
        {

        }

        public override void ClearSignals()
        {

        }

        public override object EnteredValue
        {
            get { return t_colorVariance; }
        }
        #endregion
    }
}
