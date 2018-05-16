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

namespace PricerGUI.MainWindowComponents
{
    /// <summary>
    /// Interaction logic for PricingRequestEditor.xaml
    /// </summary>
    public partial class PricingRequestEditor : UserControl, INotifyPropertyChanged
    {
        public PricingRequestEditor()
        {
            InitializeComponent();
        }

        public PricingRequestWithResult Request
        {
            get {
                return new PricingRequestWithResult(
                    new PricerCore.PricingRequest(
                        instrumentSelector.Instrument,
                        ycSelector.CurveSource
                        ),
                    null
                );
            }
            set
            {
                
                if (value == null)
                {
                    //if the item just got deleted, we leave the request in the editor. it will become hidden beacuse f the datatriggers
                    return;
                }
                else
                {
                    instrumentSelector.Instrument = value.Request.Instrument;
                    ycSelector.CurveSource = value.Request.Ycs;
                    OnPropertyChanged("Request");
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void instrumentSelector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Instrument")
            {
                OnPropertyChanged("Request");
            }
        }

        private void ycSelector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurveSource")
            {
                OnPropertyChanged("Request");
            }
        }
    }
}
