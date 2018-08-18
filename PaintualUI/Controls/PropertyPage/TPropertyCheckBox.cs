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
    [TemplatePart(Name = "C_ChkFromImage", Type = typeof(CheckBox))]
    [TemplatePart(Name = "C_Label", Type = typeof(Label))]
    public class TPropertyCheckBox : PaintualUI.Controls.PropertyPage.TPropertyControl
    {
        protected CheckBox C_ChkFromImage;
        protected Label C_Label;

        static TPropertyCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyCheckBox), new FrameworkPropertyMetadata(typeof(TPropertyCheckBox)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> required to get the validators list instantiated in TPropertyControl parent class</remarks>
        public TPropertyCheckBox() : base()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            C_ChkFromImage = Template.FindName("C_ChkFromImage", this) as CheckBox;
            C_Label = Template.FindName("C_Label", this) as Label;
        }

        #region TIPropertyControl implementation

        public override void BuildControl(Engine.Effects.VisualPropertyItem pi)
        {
            Name = pi.ActualPropertyName;
            PropertyName = pi.ActualPropertyName;
            LabelText = pi.DisplayName;
            DataType = pi.DataType;
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
                C_ChkFromImage.IsChecked = (bool)DefaultValue;
            }
        }

        public override void SignalError(string message)
        {

        }

        public override void ClearSignals()
        {

        }

        public override object EnteredValue
        {
            get { return C_ChkFromImage.IsChecked; }
        }
        #endregion



    }
}
