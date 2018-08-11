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
    /// Interaction logic for PerformanceMonitor.xaml
    /// </summary>
    public partial class PerformanceMonitor : UserControl
    {
        private System.Windows.Threading.DispatcherTimer t_timer;
        private System.Diagnostics.PerformanceCounter t_perf;

        public PerformanceMonitor()
        {
            InitializeComponent();

            t_perf = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total", true);

            t_timer = new System.Windows.Threading.DispatcherTimer();
            t_timer.Tick += new EventHandler(UpdateVisual);
            t_timer.Interval = new TimeSpan(0, 0, 0, 0, 200); // below 35 not all draw points are shown on MouseUp // they will on next mouse down
            // timer runs continuously but VIOME has a flag that prevents unnecessary canvas refresh when there is no activity.
            t_timer.Start();

            progBar.Minimum = 0;
            progBar.Maximum = 100;
        }

        /// <summary>
        /// method signature to match required EventHandler of DispatcherTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateVisual(object sender, EventArgs e)
        {
            int nextValue = (int)t_perf.NextValue();

            string strValue = nextValue.ToString() + " %";

            this.LblValue.Content = strValue;
            this.progBar.Value = nextValue;
        }
    }
}
