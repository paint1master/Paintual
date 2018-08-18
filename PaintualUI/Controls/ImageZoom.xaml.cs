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

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for ImageZoom.xaml
    /// </summary>
    public partial class ImageZoom : UserControl
    {
        private double maxLimit = 100d;
        private double t_zoomFactor;

        public ImageZoom()
        {
            InitializeComponent();

            t_zoomFactor = maxLimit / 2;
            ZoomSlider.Minimum = 0d;
            ZoomSlider.Maximum = maxLimit;
            ZoomSlider.Value = t_zoomFactor;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double result = CalculateZoomFactorAsPercentage(e.NewValue);

            TxtZoom.Text = String.Format("{0} %", Math.Round(result, 0));

            if (result != t_zoomFactor)
            {
                t_zoomFactor = result;
                OnZoomFactorChanged(CalculateZoomPercentageToFactor(result));
            }
        }

        private double CalculateZoomFactorAsPercentage(double sliderValue)
        {
            double power = (((2 * sliderValue) - maxLimit) / maxLimit);
            double result = Math.Pow(15, power) * maxLimit;

            return result;
        }

        private double CalculateZoomPercentageToFactor(double percentage)
        {
            double result = percentage / maxLimit; 

            return result;
        }

        private void Btn100Percent_Click(object sender, RoutedEventArgs e)
        {
            double oneHundred = maxLimit / 2; 

            ZoomSlider.Value = oneHundred;

            double newZoom = CalculateZoomFactorAsPercentage(oneHundred);

            if (t_zoomFactor != newZoom)
            {
                t_zoomFactor = newZoom;
                OnZoomFactorChanged(CalculateZoomPercentageToFactor(t_zoomFactor));
            }
        }

        /// <summary>
        /// A value ranging from -1 to +1 which can be used to calculate zoomed sizes.
        /// </summary>
        public double ZoomFactor
        {
            get { return CalculateZoomPercentageToFactor(t_zoomFactor); }
        }

        private void BtnToFit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("not yet implemented.");
        }

        #region Events

        public event Engine.CoordinatesManager.ZoomFactorChangedEventHandler ZoomFactorChanged;

        private void OnZoomFactorChanged(double zoomFactor)
        {
            ZoomFactorChanged?.Invoke(this, new Engine.ZoomFactorChangedEventArgs(zoomFactor));
        }

        #endregion
    }
}
