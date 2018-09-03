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

namespace PaintualUI.Controls
{
    /// <summary>
    /// Interaction logic for ImageZoom.xaml
    /// </summary>
    public partial class ImageZoom : UserControl
    {
        /// <summary>
        /// The highest value supported by the Slider control. 50 = 100%. 100 = 1500%;
        /// </summary>
        private double maxLimit = 100d;
        private double t_zoomPercentage = 100d; // not the same as maxLimit;

        #region control event coordination
        private bool t_ignoreZoomerEvent = false;
        private bool t_ignoreTextBoxEvent = false;
        private bool t_ignoreBtn100Event = false;

        #endregion

        public ImageZoom()
        {
            InitializeComponent();

            ZoomSlider.Minimum = 0d;
            ZoomSlider.Maximum = maxLimit;
            AdjustZoomSlider(t_zoomPercentage);
        }

        private void AdjustZoomSlider(double percentage)
        {
            // to prevent the slider from triggering a value changed event
            t_ignoreZoomerEvent = true;
            ZoomSlider.Value = PercentageToSliderValue(percentage);
            t_ignoreZoomerEvent = false;
        }

        private void AdjustTextBox(double percentage)
        {
            t_ignoreTextBoxEvent = true;
            TxtZoom.Text = String.Format("{0} %", Math.Round(percentage, 0));
            t_ignoreTextBoxEvent = false;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (t_ignoreZoomerEvent)
            {
                return;
            }

            double percent = SliderValueAsPercentage(e.NewValue);  

            if (percent != t_zoomPercentage)
            {
                t_zoomPercentage = percent;
                AdjustTextBox(t_zoomPercentage);
                OnZoomFactorChanged(CalculateZoomPercentageToFactor(percent));
            }
        }

        private double SliderValueAsPercentage(double sliderValue)
        {
            double power = (((2 * sliderValue) - maxLimit) / maxLimit);
            double result = Math.Pow(15, power) * maxLimit;

            return result;
        }

        private double PercentageToSliderValue(double percentage)
        {
            double inBase15 = System.Math.Log(percentage, 15);
            double result = (((inBase15 * maxLimit) - maxLimit) / 2) + 15;
            return System.Math.Round(result);
        }

        private double CalculateZoomPercentageToFactor(double percentage)
        {
            return percentage / 100d;
        }

        private void Btn100Percent_Click(object sender, RoutedEventArgs e)
        {
            double newZoom = 100d;

            if (t_zoomPercentage != newZoom)
            {
                t_zoomPercentage = newZoom;
                AdjustZoomSlider(PercentageToSliderValue(t_zoomPercentage));
                AdjustTextBox(t_zoomPercentage);
                OnZoomFactorChanged(CalculateZoomPercentageToFactor(t_zoomPercentage));
            }
        }

        internal void SetZoomFactor(double zoomFactor)
        {
            double newPercent = zoomFactor * 100d;

            if (newPercent != t_zoomPercentage)
            {
                t_zoomPercentage = newPercent;
                AdjustZoomSlider(PercentageToSliderValue(t_zoomPercentage));
                AdjustTextBox(t_zoomPercentage);

                // do not raise a factor change event, because it would cause an infinite loop.
                // this method being called from the drawing board.
            }
        }

        /// <summary>
        /// A value ranging from -1 to +1 which can be used to calculate zoomed sizes.
        /// </summary>
        public double ZoomFactor
        {
            get { return CalculateZoomPercentageToFactor(t_zoomPercentage); }
        }

        private void BtnToFit_Click(object sender, RoutedEventArgs e)
        {
            OnZoomFactorUpdateRequested();
            AdjustZoomSlider(PercentageToSliderValue(t_zoomPercentage));
            AdjustTextBox(t_zoomPercentage);
        }

        #region Events

        public event Engine.CoordinatesManager.ZoomFactorChangedEventHandler ZoomFactorChanged;
        public event Engine.CoordinatesManager.ZoomFactorUpdateRequestEventHandler ZoomFactorUpdateRequested;

        private void OnZoomFactorChanged(double zoomFactor)
        {
            ZoomFactorChanged?.Invoke(this, new Engine.ZoomFactorChangedEventArgs(zoomFactor));
        }

        private void OnZoomFactorUpdateRequested()
        {
            ZoomFactorUpdateRequested?.Invoke(this, new Engine.ZoomFactorUpdateRequestEventArgs());
        }

        #endregion
    }
}
