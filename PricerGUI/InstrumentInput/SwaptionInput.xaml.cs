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

namespace PricerGUI.InstrumentInput
{
    /// <summary>
    /// Interaction logic for SwaptionInput.xaml
    /// </summary>
    public partial class SwaptionInput : UserControl, INotifyPropertyChanged
    {
        public SwaptionInput()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("DataContext");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertyChanged("DataContext");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
