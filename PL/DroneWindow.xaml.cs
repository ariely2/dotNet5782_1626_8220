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
    public partial class DroneWindow : Window
    {
        IBL.BO.Drone drone;
        private IBL.IBL bl;
        public DroneWindow(IBL.IBL b)
        {
            InitializeComponent();
            bl = b;
            drone = new IBL.BO.Drone();
            drone.Location = new IBL.BO.Location();
            DataContext = drone;
            StationSelector.ItemsSource = bl.RequestList<IBL.BO.StationToList>();
            WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
        }

        public DroneWindow(IBL.IBL b, IBL.BO.Drone drone)
        {
            InitializeComponent();
            bl = b;
            this.drone = drone;
            DataContext = drone;
            StationSelector.ItemsSource = bl.RequestList<IBL.BO.StationToList>();
            WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            drone.Location = bl.Request<IBL.BO.Station>((int)StationSelector.SelectedItem).location;
            bl.Create<IBL.BO.Drone>(drone);
        }
    }
}
