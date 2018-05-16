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
    /// Interaction logic for InstrumentSelector.xaml
    /// </summary>
    public partial class InstrumentSelector : UserControl, INotifyPropertyChanged
    {
        private Bond _bond;
        public Bond _Bond
        {
            get { return _bond; }
            set { _bond = value; OnPropertyChanged("_Bond"); OnPropertyChanged("Instrument"); }
        }

        private BondOption _bondOption;
        public BondOption _BondOption
        {
            get { return _bondOption; }
            set { _bondOption = value; OnPropertyChanged("_BondOption"); OnPropertyChanged("Instrument"); }
        }

        private Swap _swap;
        public Swap _Swap
        {
            get { return _swap; }
            set { _swap = value; OnPropertyChanged("_Swap"); OnPropertyChanged("Instrument"); }
        }

        private Swaption _swaption;
        public Swaption _Swaption
        {
            get { return _swaption; }
            set { _swaption = value; OnPropertyChanged("_Swaption"); OnPropertyChanged("Instrument"); }
        }

        //public static readonly DependencyProperty InstrumentProperty = DependencyProperty.RegisterAttached(
        //    "Instrument",
        //    typeof(IInstrument),
        //    typeof(InstrumentSelector),
        //    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        //);

        //public static void SetInstrument(System.Windows.UIElement element, IInstrument value)
        //{
        //    ((InstrumentSelector)element).__Instrument = value;
        //}
        //public static IInstrument GetInstrument(System.Windows.UIElement element)
        //{
        //    return ((InstrumentSelector)element).__Instrument;
        //}

        public IInstrument Instrument
        {
            get
            {
                var selectedTab = (TabItem)tabControl.SelectedItem;
                var selectedContentInTab = (UserControl)(selectedTab).Content;
                return (IInstrument)selectedContentInTab.DataContext;
            }

            set
            {
                Type type = value.GetType();

                TabItem toSelect = null;

                if (type == typeof(Bond))
                {
                    toSelect = bndTab;
                    _Bond = (Bond)value;
                    _BondOption = new BondOption();
                    _Swap = new Swap();
                    _Swaption = new Swaption();
                }
                else if (type == typeof(BondOption))
                {
                    toSelect = bndoTab;
                    _BondOption = (BondOption)value;
                    _Bond = new Bond();
                    _Swap = new Swap();
                    _Swaption = new Swaption();
                }
                else if (type == typeof(Swap))
                {
                    toSelect = swTab;
                    _Swap = (Swap)value;
                    _Bond = new Bond();
                    _BondOption = new BondOption();
                    _Swaption = new Swaption();
                }
                else if (type == typeof(Swaption))
                {
                    toSelect = swoTab;
                    _Swaption = (Swaption)value;
                    _Bond = new Bond();
                    _BondOption = new BondOption();
                    _Swap = new Swap();
                }

                Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedItem = toSelect));
                OnPropertyChanged("Instrument");
            }
        }
        public InstrumentSelector()
        {
            InitializeComponent();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //_bond = new Bond();
            //_bondOption = new BondOption();
            //_swap = new Swap();
            //_swaption = new Swaption();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("Instrument");
        }

        private void Input_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataContext")
            {
                OnPropertyChanged("Instrument");
            }
        }
    }
}
