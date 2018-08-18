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
using System.Windows;
using System.Windows.Controls;

namespace PaintualUI.Controls.PropertyPage
{
    [TemplatePart(Name = "C_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "C_Label", Type = typeof(Label))]
    [TemplatePart(Name = "C_InfoIcon", Type = typeof(PaintualUI.Controls.PropertyPage.InfoIcon))]
    public class TPropertyIntBox : PaintualUI.Controls.PropertyPage.TPropertyTextBox
    {
        static TPropertyIntBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyIntBox), new FrameworkPropertyMetadata(typeof(TPropertyIntBox)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> required to get the validators list instantiated in TPropertyControl parent class</remarks>
        public TPropertyIntBox() : base()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            C_TextBox = Template.FindName("C_TextBox", this) as TextBox;
            C_Label = Template.FindName("C_Label", this) as Label;
            C_InfoIcon = Template.FindName("C_InfoIcon", this) as PaintualUI.Controls.PropertyPage.InfoIcon;
        }

        #region TIPropertyControl implementation

        public override void BuildControl(Engine.Effects.VisualPropertyItem pi)
        {
            Name = pi.ActualPropertyName;
            PropertyName = pi.ActualPropertyName;
            LabelText = pi.DisplayName;
            DataType = pi.DataType;
            DefaultValue = pi.DefaultValue;

            if (pi.ValidatorType != Engine.Attributes.Meta.ValidatorTypes.Undefined)
            {
                switch (pi.ValidatorType)
                {
                    case Engine.Attributes.Meta.ValidatorTypes.Int:
                        Validators.Add(new Engine.Validators.IntValidator());

                        if (pi.RangeMinimumValue != null && pi.RangeMaximumValue != null)
                        {
                            Validators.Add(new Engine.Validators.RangeIntValidator(pi.RangeMinimumValue.Value, pi.RangeMaximumValue.Value));
                        }
                        break;
                    default:
                        throw new Exception(String.Format("In TPropertyIntBox, the validator type '{0}' is not supported.", pi.ValidatorType));
                }
            }
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
            C_Label.Content = LabelText;

            if (DefaultValue != null)
            {
                int defVal = (int)DefaultValue;
                C_TextBox.Text = defVal.ToString();
            }
        }

        #endregion
    }
}
