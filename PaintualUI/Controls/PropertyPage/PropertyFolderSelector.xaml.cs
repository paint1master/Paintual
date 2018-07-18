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
    /// <summary>
    /// Interaction logic for PropertyFolderSelector.xaml
    /// </summary>
    public partial class PropertyFolderSelector : UserControl, IPropertyControl
    {
        private PropertyControlCommonContent t_pccc;

        public PropertyFolderSelector()
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

        public PropertyControlCommonContent CommonContent
        {
            get { return t_pccc; }
        }

        public string EnteredValue
        {
            get { return this.TextBox.Text; }
        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            Gat.Controls.OpenDialogView openDialog = new Gat.Controls.OpenDialogView();
            Gat.Controls.OpenDialogViewModel vm = (Gat.Controls.OpenDialogViewModel)openDialog.DataContext;
            vm.IsDirectoryChooser = true;
            //vm.Owner = this;
            vm.StartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            bool? result = vm.Show();
            if (result == true)
            {
                TextBox.Text = vm.SelectedFilePath;
            }
            else
            {
                TextBox.Text = string.Empty;
            }
        }
    }
}
