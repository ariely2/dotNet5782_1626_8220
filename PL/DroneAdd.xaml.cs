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

namespace PL
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DroneAdd : Window
    {
        private IBL.IBL bl;
        public DroneAdd()
        {
            InitializeComponent();
        }
        public DroneAdd(IBL.IBL b)
        {
            InitializeComponent();
            bl = b;
            WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
