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
    public enum Status
    {
        Normal,
        Info,
        Error
    }


    /// <summary>
    /// Interaction logic for InfoIcon.xaml
    /// </summary>
    public partial class InfoIcon : UserControl
    {
        private Status t_status = Status.Normal;
        private PaintualUI.Controls.MessageWindow t_messageWindow;

        public InfoIcon()
        {
            InitializeComponent();
        }

        public Status Status
        {
            set {
                t_status = value;

                switch (t_status)
                {
                    case Status.Normal:
                        InfoImage.Visibility = Visibility.Hidden;
                        break;

                    case Status.Error:
                        InfoImage.Visibility = Visibility.Visible;
                        InfoImage.Source = new BitmapImage(new Uri("pack://application:,,,/PaintualUI;component/Resources/InfoIcon_Error.png"));
                        break;

                    case Status.Info:
                        InfoImage.Visibility = Visibility.Visible;
                        InfoImage.Source = new BitmapImage(new Uri("pack://application:,,,/PaintualUI;component/Resources/InfoIcon_Warning.png"));
                        break;

                    default:

                        throw new Exception("Unsupported status in InfoIcon control.");
                }
            }
        }

        internal void SetMessageWindow(string propertyName, string message)
        {
            t_messageWindow = new MessageWindow();
            t_messageWindow.ErrorTitle = propertyName;
            t_messageWindow.ErrorMessage = message;
        }

        private void InfoImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (t_status == Status.Normal)
            {
                return;
            }

            if (t_status == Status.Error)
            {
                t_messageWindow.Show();
            }
        }
    }
}
