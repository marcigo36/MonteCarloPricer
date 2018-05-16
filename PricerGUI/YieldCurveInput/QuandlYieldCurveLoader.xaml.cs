using OxyPlot;
using PricerCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PricerGUI.YieldCurveInput
{
    /// <summary>
    /// Interaction logic for QuandlYieldCurveLoader.xaml
    /// </summary>
    public partial class QuandlYieldCurveLoader : UserControl, INotifyPropertyChanged
    {
        public QuandlYieldCurveLoader()
        {
            InitializeComponent();
            Curve = PricerCore.QuandlDataProvider.Instance.GetYieldCurve(DateTime.Now);

            UpdateCurve(Curve);

        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Curve = PricerCore.QuandlDataProvider.Instance.GetYieldCurve(datePicker.SelectedDate ?? DateTime.Now);
            OnPropertyChanged("Curve");

            UpdateCurve(Curve);
        }

        public YieldCurve Curve { get; private set; }

        public IYieldCurveSource CurveSource { get { return new QuandlYieldCurveSource(datePicker.SelectedDate ?? DateTime.Now); } }

        public void UpdateCurve(YieldCurve curve)
        {
            this.YieldAsOxyPlotCurve = Utils.Plot(curve, 0.0, 30.0, 0.01).ToList();
            OnPropertyChanged("YieldAsOxyPlotCurve");
        }

        public IList<DataPoint> YieldAsOxyPlotCurve { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
