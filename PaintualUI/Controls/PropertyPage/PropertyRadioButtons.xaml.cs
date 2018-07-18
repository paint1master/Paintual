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
using System.Reflection;
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
    /// <summary>
    /// Interaction logic for PropertyRadioButtons.xaml
    /// </summary>
    public partial class PropertyRadioButtons : UserControl, IPropertyControl
    {
        private PropertyControlCommonContent t_pccc;

        public PropertyRadioButtons()
        {
            InitializeComponent();

            t_pccc = new PropertyControlCommonContent();
        }

        public void UpdateVisual()
        {
            this.Label.Content = t_pccc.LabelText;

            Dictionary<string, object> dict = t_pccc.ValueList.Dictionary;

            double position = 30d;

            // iterate through dictionary
            foreach (var v in dict)
            {
                // create a radio button with label being v.Key
                // v.Value is not absolutely necessary here because the user will select a radio button based on key
                // then when user clicks "Apply" code will get the key and look for the value in the dict and pass that value
                // as object to the Effect which, in turn, will know the exact type of the object (some sort of enum)

                if (v.Key == t_pccc.ValueList.HiddenValue)
                {
                    continue;
                }

                PaintualUI.Controls.PropertyPage.RadioButton rb = new RadioButton();
                rb.RadButton.Content = v.Key;
                rb.RadButton.GroupName = t_pccc.LabelText;
                rb.Margin = new Thickness(10d, position, 0d, 0d);

                this.canvas.Children.Add(rb);
                position += 40d;
            }

            this.canvas.Height = position;
        }

        public void SignalError(string message)
        {
            // TODO : code
        }

        public PropertyControlCommonContent CommonContent
        {
            get { return t_pccc; }
        }

        public string EnteredValue
        {
            get {

                string result = "";

                foreach (var rad in this.canvas.Children)
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

                return result; // "Property code not set"; /*this.TextBox.Text;*/

            }
        }
    }
}
