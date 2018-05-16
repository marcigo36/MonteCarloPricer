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

using PricerGUI.MainWindowComponents;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PricerCore;

namespace PricerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<PricingRequestWithResult> requests = new ObservableCollection<PricingRequestWithResult>();

        MonteCarloPricer pricer = new MonteCarloPricer(0.01, 200);

        public MainWindow()
        {
            InitializeComponent();
            requestsListBox.ItemsSource = requests;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //creating a new pricing request
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewInstrument();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var toDelete = (PricingRequestWithResult)requestsListBox.SelectedItem;

            requests.Remove(toDelete);
        }

        private void AddNewInstrument()
        {
            requests.Add(
                new PricingRequestWithResult(
                    new PricerCore.PricingRequest(
                        new PricerCore.Bond(),
                        new PricerCore.QuandlYieldCurveSource(DateTime.Today)
                        ),
                    null
                )
            );
        }

        private void PriceButton_Click(object sender, RoutedEventArgs e)
        {
            var toPrice = (PricingRequestWithResult)requestsListBox.SelectedItem;

            if (toPrice.IsPricingOngoing)
            {
                return;
            }

            var aaa = Task.Run(new Action(() => {

                toPrice.IsPricingOngoing = true;

                double result = pricer.Price(toPrice.Request);
                toPrice.Result = result;

                toPrice.IsPricingOngoing = false;

            }));
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //we dont want this to be null, even if it is hidden at the beginning
            pricingRequestEditor.Request = new PricingRequestWithResult(
                    new PricerCore.PricingRequest(
                        new PricerCore.Bond(),
                        new PricerCore.QuandlYieldCurveSource(DateTime.Today)
                        ),
                    null
                );
        }

        private void requestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var toEdit = (PricingRequestWithResult)requestsListBox.SelectedItem;
            pricingRequestEditor.Request = toEdit;
        }

        private void pricingRequestEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Request")
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    var toEdit = (PricingRequestWithResult)requestsListBox.SelectedItem;
                    if (toEdit == null) return;
                    toEdit.Request = pricingRequestEditor.Request.Request;
                }));
            }
        }

        private void pricingRequestEditor_KeyDown(object sender, KeyEventArgs e)
        {
            //var toEdit = (PricingRequestWithResult)requestsListBox.SelectedItem;
            //toEdit.Request = pricingRequestEditor.Request.Request;
        }

        private void pricingRequestEditor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var toEdit = (PricingRequestWithResult)requestsListBox.SelectedItem;
            //toEdit.Request = pricingRequestEditor.Request.Request;
        }

        private void visualizerButton_Click(object sender, RoutedEventArgs e)
        {
            YieldCurve yc = ((PricingRequestWithResult)requestsListBox.SelectedItem).Request.Ycs.get();

            DiscountCurve dc = (DiscountCurve)yc;
            //IProcess hwp = new HullWhiteCalibrator().Calibrate(dc);
            IProcess rsp = RSCalibrator.Calibrate(dc);
            //var visualizerWindow1 = new ProcessVisualizer(yc, hwp);
            var visualizerWindow2 = new ProcessVisualizer(yc, rsp);
            visualizerWindow2.Show();
        }
    }
}
