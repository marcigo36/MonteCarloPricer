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
    /// Interaction logic for YieldCurveEditor.xaml
    /// </summary>
    public partial class YieldCurveEditor : UserControl, INotifyPropertyChanged
    {
        public YieldCurveEditor()
        {
            InitializeComponent();
        }

        public YieldCurve Curve
        {
            get
            {
                return new YieldCurve(new CubicSpline(new List<PricerCore.Point>
                {
                    new PricerCore.Point(0.0, (2*slider1m.Value-slider3m.Value)),
                    new PricerCore.Point(1.0/12.0, slider1m.Value),
                    new PricerCore.Point(3.0/12.0, slider3m.Value),
                    new PricerCore.Point(6.0/12.0, slider6m.Value),
                    new PricerCore.Point(1.0, slider1y.Value),
                    new PricerCore.Point(2.0, slider2y.Value),
                    new PricerCore.Point(3.0, slider3y.Value),
                    new PricerCore.Point(5.0, slider5y.Value),
                    new PricerCore.Point(7.0, slider7y.Value),
                    new PricerCore.Point(10.0, slider10y.Value),
                    new PricerCore.Point(20.0, slider20y.Value),
                    new PricerCore.Point(30.0, slider30y.Value),
                },
                "CUSTOM/YIELD"));
            }
            set
            {
                slider1m.Value = value[1.0 / 12.0];
                slider3m.Value = value[3.0 / 12.0];
                slider6m.Value = value[6.0 / 12.0];
                slider1y.Value = value[1.0];
                slider2y.Value = value[2.0];
                slider3y.Value = value[3.0];
                slider5y.Value = value[5.0];
                slider7y.Value = value[7.0];
                slider10y.Value = value[10.0];
                slider20y.Value = value[20.0];
                slider30y.Value = value[30.0];
                OnPropertyChanged("Curve");
            }
        }

        public IYieldCurveSource CurveSource { get { return new CustomYieldCurveSource(Curve); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void sliders_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value") OnPropertyChanged("Curve");
        }
    }
}
