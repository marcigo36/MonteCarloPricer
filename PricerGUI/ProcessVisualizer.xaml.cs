using OxyPlot;
using PricerCore;
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
using System.Windows.Shapes;

namespace PricerGUI
{
    /// <summary>
    /// Interaction logic for ProcessVisualizer.xaml
    /// </summary>
    public partial class ProcessVisualizer : Window
    {
        public ProcessVisualizer(YieldCurve yc, IProcess process)
        {
            this.forwardRate = ((DiscountCurve)yc).ForwardRate;
            this.process = process;
            InitializeComponent();
        }

        Func<double,double> forwardRate;
        IProcess process;

        private void generatePlot()
        {
            double t = 0;
            double r0 = forwardRate(0.01);
            var pi1 = new List<DataPoint>(process.generatePathFrom(new RSProcess.State(r0, 0), 0, 0.01, 30).Select(fx => new DataPoint(t += 0.01, fx.r)));
             t = 0;
            var pi2 = new List<DataPoint>(process.generatePathFrom(new RSProcess.State(r0, 0), 0, 0.01, 30).Select(fx => new DataPoint(t += 0.01, fx.r)));
             t = 0;
            var pi3 = new List<DataPoint>(process.generatePathFrom(new RSProcess.State(r0, 0), 0, 0.01, 30).Select(fx => new DataPoint(t += 0.01, fx.r)));

            this.DataContext = new {
                fwrates = Utils.Plot(forwardRate, 0.0, 30.0, 0.01).ToList(),
                pi1 = pi1,
                pi2 = pi2,
                pi3 = pi3,
            };
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            generatePlot();
        }

        private void regenButton_Click(object sender, RoutedEventArgs e)
        {
            generatePlot();
        }
    }
}
