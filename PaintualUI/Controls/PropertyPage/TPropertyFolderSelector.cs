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
    [TemplatePart(Name = "C_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "C_Label", Type = typeof(Label))]
    [TemplatePart(Name = "C_BtnFolder", Type = typeof(Button))]
    [TemplatePart(Name = "C_InfoIcon", Type = typeof(PaintualUI.Controls.PropertyPage.InfoIcon))]
    public class TPropertyFolderSelector : PaintualUI.Controls.PropertyPage.TPropertyTextBox
    {
        protected Button C_BtnFolder;

        static TPropertyFolderSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyFolderSelector), new FrameworkPropertyMetadata(typeof(TPropertyFolderSelector)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks> required to get the validators list instantiated in TPropertyControl parent class</remarks>
        public TPropertyFolderSelector() : base()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            C_TextBox = Template.FindName("C_TextBox", this) as TextBox;
            C_Label = Template.FindName("C_Label", this) as Label;
            C_BtnFolder = Template.FindName("C_BtnFolder", this) as Button;
            C_InfoIcon = Template.FindName("C_InfoIcon", this) as PaintualUI.Controls.PropertyPage.InfoIcon;

            // TODO : this should instead be CommandBinding thing, but what I tried so far didn't work. 
            // OnAplyTemplate happens after OnInitialize, so event handling goes here
            C_BtnFolder.Click += C_BtnFolder_Click;
        }

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
            base.UpdateVisual();

        }

        private void C_BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.OpenDialogView openDialog = new PaintualUI.Controls.OpenDialogView();
            PaintualUI.Controls.OpenDialogViewModel vm = (PaintualUI.Controls.OpenDialogViewModel)openDialog.DataContext;
            vm.IsDirectoryChooser = true;

            vm.StartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            bool? result = vm.Show();
            if (result == true)
            {
                C_TextBox.Text = vm.SelectedFilePath;
            }
            else
            {
                C_TextBox.Text = string.Empty;
            }
        }
    }
}
