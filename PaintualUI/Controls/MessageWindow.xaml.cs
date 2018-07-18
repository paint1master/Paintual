﻿using System;
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
using System.Windows.Shapes;

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string ErrorTitle
        {
            get { return (string)this.lblTitle.Content; }
            set { this.lblTitle.Content = value; }
        }

        public string ErrorMessage
        {
            get { return (string)this.lblMessage.Content; }
            set { this.lblMessage.Content = value; }
        }
    }
}
