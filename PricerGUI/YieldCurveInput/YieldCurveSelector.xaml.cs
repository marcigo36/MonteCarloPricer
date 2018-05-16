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
    /// Interaction logic for YieldCurveSelector.xaml
    /// </summary>
    public partial class YieldCurveSelector : UserControl, INotifyPropertyChanged
    {
        public YieldCurveSelector()
        {
            InitializeComponent();

            //we initially set the current curve in the editor
            ycEditor.Curve = ycLoader.Curve;

            qndlTab.SetBinding(YieldCurveSelector.CurveSourceProperty, new Binding() { Source = ycLoader, Path = new PropertyPath("Curve") });
            customTab.SetBinding(YieldCurveSelector.CurveSourceProperty, new Binding() { Source = ycEditor, Path = new PropertyPath("Curve") });
        }

        YieldCurve yc;

        public YieldCurve Curve
        {
            get => yc;
            private set
            {
                yc = value;
                OnPropertyChanged("Curve");
            }
        }


        private IYieldCurveSource ycSource;

        public IYieldCurveSource CurveSource
        {
            get { return ycSource; }
            set {
                //setting all ui elemnts to the appropriate state
                ycSource = value;
                OnPropertyChanged("CurveSource");

                Type t = value.GetType();

                if (t == typeof(QuandlYieldCurveSource))
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        tabcontrol.SelectedIndex = 0;
                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        tabcontrol.SelectedIndex = 1;
                    }));
                }

                Curve = value.get();
            }
        }



        private void UpdateCurve()
        {
            //getting the attached yield curve from the active tab
            var source = ((TabItem)(tabcontrol.SelectedItem))?.GetValue(YieldCurveSelector.CurveSourceProperty);

            if (source == null) return;

            Curve = source as YieldCurve;
            UpdateDiscountCurve((DiscountCurve)yc);

            if ((string)((TabItem)(tabcontrol.SelectedItem))?.Header == "Quandl")
            {
                CurveSource = ycLoader.CurveSource;
            }
            else
            {
                CurveSource = ycEditor.CurveSource;
            }
        }

        protected void ycSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Curve") UpdateCurve();
        }

        public static readonly DependencyProperty CurveSourceProperty = DependencyProperty.RegisterAttached(
            "CurveSource",
            typeof(YieldCurve),
            typeof(YieldCurveSelector),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static void SetCurveSource(System.Windows.UIElement element, YieldCurve value)
        {
            element.SetValue(CurveSourceProperty, value);
        }
        public static YieldCurve GetCurveSource(System.Windows.UIElement element)
        {
            return (YieldCurve)element.GetValue(CurveSourceProperty);
        }

        private void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == tabcontrol) UpdateCurve();
        }

        public void UpdateDiscountCurve(DiscountCurve curve)
        {
            this.DiscountCurveAsOxyPlotCurve = Utils.Plot(curve, 0.0, 30.0, 0.01).ToList();
            OnPropertyChanged("DiscountCurveAsOxyPlotCurve");
        }

        public IList<DataPoint> DiscountCurveAsOxyPlotCurve { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
