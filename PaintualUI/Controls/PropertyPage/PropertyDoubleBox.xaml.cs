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

using System.Windows.Controls;



namespace PaintualUI.Controls.PropertyPage
{
    /// <summary>
    /// Interaction logic for PropertyDoubleBox.xaml
    /// </summary>
    public partial class PropertyDoubleBox : UserControl, IPropertyControl
    {
        private PropertyControlCommonContent t_pccc;

        public PropertyDoubleBox()
        {
            InitializeComponent();

            t_pccc = new PropertyControlCommonContent();
        }

        public void UpdateVisual()
        {
            this.Label.Content = t_pccc.LabelText;
        }

        public void SignalError(string message)
        {
            // TODO : code
        }

        public void ClearSignals()
        {
            /*this.InfoIcon.Status = Status.Normal;
            this.InfoIcon.SetMessageWindow("", "");
            this.InfoIcon.Visibility = Visibility.Hidden;*/
        }

        public PropertyControlCommonContent CommonContent
        {
            get { return t_pccc; }
        }

        public string EnteredValue
        {
            get { return this.TextBox.Text; }
        }
    }
}
