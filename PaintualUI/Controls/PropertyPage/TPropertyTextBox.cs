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
    [TemplatePart(Name="C_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "C_Label", Type = typeof(Label))]
    [TemplatePart(Name = "C_InfoIcon", Type = typeof(PaintualUI.Controls.PropertyPage.InfoIcon))]
    public class TPropertyTextBox : PaintualUI.Controls.PropertyPage.TPropertyControl
    {
        protected TextBox C_TextBox;
        protected Label C_Label;
        protected PaintualUI.Controls.PropertyPage.InfoIcon C_InfoIcon;

        static TPropertyTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyTextBox), new FrameworkPropertyMetadata(typeof(TPropertyTextBox)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> required to get the validators list instantiated in TPropertyControl parent class</remarks>
        public TPropertyTextBox() : base()
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

        /// <summary>
        /// Builds the children controls.
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
                string defVal = (string)DefaultValue;
                C_TextBox.Text = defVal;
            }
        }

        public override void SignalError(string message)
        {
            C_InfoIcon.Status = Status.Error;
            C_InfoIcon.SetMessageWindow(PropertyName, message);
            C_InfoIcon.Visibility = Visibility.Visible;
        }

        public override void ClearSignals()
        {
            C_InfoIcon.Status = Status.Normal;
            C_InfoIcon.SetMessageWindow("", "");
            C_InfoIcon.Visibility = Visibility.Hidden;
        }

        public override string EnteredValue
        {
            get { return C_TextBox.Text; }
        }
        #endregion
    }
}
