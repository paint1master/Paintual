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
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
        private string t_propertyName;
        private string t_errorMessage;

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
            // cannot create instace of MessageWindow here because when closed is cannot be reopened with another message
            t_propertyName = propertyName;
            t_errorMessage = message;
        }

        private void InfoImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (t_status == Status.Normal)
            {
                return;
            }

            if (t_status == Status.Error)
            {
                t_messageWindow = new MessageWindow();
                t_messageWindow.ErrorTitle = t_propertyName;
                t_messageWindow.ErrorMessage = t_errorMessage;
                t_messageWindow.Show();
            }
        }
    }
}
