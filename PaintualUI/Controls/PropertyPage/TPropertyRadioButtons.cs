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
    [TemplatePart(Name = "C_Label", Type = typeof(Label))]
    [TemplatePart(Name = "C_InfoIcon", Type = typeof(PaintualUI.Controls.PropertyPage.InfoIcon))]
    public class TPropertyRadioButtons : PaintualUI.Controls.PropertyPage.TPropertyTextBox
    {
        static TPropertyRadioButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyRadioButtons), new FrameworkPropertyMetadata(typeof(TPropertyRadioButtons)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> required to get the validators list instantiated in TPropertyControl parent class</remarks>
        public TPropertyRadioButtons() : base()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            C_Label = Template.FindName("C_Label", this) as Label;
            C_InfoIcon = Template.FindName("C_InfoIcon", this) as PaintualUI.Controls.PropertyPage.InfoIcon;

            Loaded += TPropertyRadioButtons_Loaded;
        }

        private void TPropertyRadioButtons_Loaded(object sender, RoutedEventArgs e)
        {
            BuildVisual();
        }

        #region ITPropertyControl implementation
        /// <summary>
        /// Builds the children controls.
        /// </summary>
        public override void BuildVisual()
        {
            base.BuildVisual();

            //  this has become unattainable since transformed into a templated control => this.canvas.Children.Add(rb);
            DependencyObject dobj = (DependencyObject)this.GetTemplateChild("C_Canvas");
            System.Windows.Controls.Canvas c = (System.Windows.Controls.Canvas)dobj;

            Dictionary<string, object> dict = ValueList.Dictionary;

            double position = 30d;

            // iterate through dictionary
            foreach (var v in dict)
            {
                // create a radio button with label being v.Key
                // v.Value is not absolutely necessary here because the user will select a radio button based on key
                // then when user clicks "Apply" code will get the key and look for the value in the dict and pass that value
                // as object to the Effect which, in turn, will know the exact type of the object (some sort of enum)

                if (v.Key == ValueList.HiddenValue)
                {
                    continue;
                }

                PaintualUI.Controls.PropertyPage.RadioButton rb = new RadioButton();
                rb.RadButton.Content = v.Key;
                rb.RadButton.GroupName = LabelText;
                rb.Margin = new Thickness(10d, position, 0d, 0d);

                c.Children.Add(rb);
                position += 40d;
            }

            c.Height = position;
        }

        /// <summary>
        /// Updates the content of visual controls (textbox, drop lists, etc) to display values set by the engine.
        /// </summary>
        public override void UpdateVisual()
        {
            C_Label.Content = LabelText;
        }


        public override string EnteredValue
        {
            get
            {
                string result = String.Empty;

                DependencyObject dobj = (DependencyObject)this.GetTemplateChild("C_Canvas");
                System.Windows.Controls.Canvas c = (System.Windows.Controls.Canvas)dobj;

                foreach (var rad in c.Children)
                {
                    if (rad.GetType().Name != "RadioButton")
                    {
                        continue;
                    }

                    PaintualUI.Controls.PropertyPage.RadioButton rb = (PaintualUI.Controls.PropertyPage.RadioButton)rad;

                    if (rb.RadButton.IsChecked.HasValue)
                    {
                        if (rb.RadButton.IsChecked.Value)
                        {
                            result = rb.RadButton.Content.ToString();
                            break;
                        }
                    }
                }

                return result;
            }
        }
        #endregion
    }
}
