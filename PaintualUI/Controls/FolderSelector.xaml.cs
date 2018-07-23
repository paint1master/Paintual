using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for FolderSelector.xaml
    /// </summary>
    public partial class FolderSelector : UserControl
    {
        public FolderSelector()
        {
            InitializeComponent();
        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            PaintualUI.Controls.OpenDialogView openDialog = new PaintualUI.Controls.OpenDialogView();
            PaintualUI.Controls.OpenDialogViewModel vm = (PaintualUI.Controls.OpenDialogViewModel)openDialog.DataContext;
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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            TextBox.Width = this.Width - (10 + 50); // 10 = marign left, 50 = margin right

            BtnFolder.SetValue(Canvas.LeftProperty, this.Width - 40);
        }
    }
}
