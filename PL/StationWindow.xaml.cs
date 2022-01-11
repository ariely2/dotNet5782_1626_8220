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
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        IBL.BO.Station station;
        private IBL.IBL bl;
        public StationWindow(IBL.IBL b)
        {
            InitializeComponent();
            StationGrid.Visibility = Visibility.Hidden;
            bl = b;
            station = new IBL.BO.Station();
            station.location = new IBL.BO.Location();
            DataContext = station;
        }
        public StationWindow(IBL.IBL b, IBL.BO.StationToList s)
        {
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden;
            bl = b;
            station = bl.Request<IBL.BO.Station>(s.Id);
            station.location = new IBL.BO.Location();
            DataContext = station;
            DronesListView.ItemsSource = station.Charging; //getting list of drones to display
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string errorMessage = "A new station could not be created for the following reasons:\n";
            if (Validation.GetErrors(ID).Any())
            {
                error = true;
                errorMessage += "ID needs to be an int!\n";
            }
            if (Validation.GetErrors(Name).Any())
            {
                error = true;
                errorMessage += "ID needs to be an int!\n";
            }
            if (Validation.GetErrors(StationLatitude).Any())
            {
                error = true;
                errorMessage += "Latitude needs to be a number!\n";
            }
            if (Validation.GetErrors(StationLongitude).Any())
            {
                error = true;
                errorMessage += "Longitude needs to be a number!\n";
            }
            errorMessage += "\nPlease Try Again.";
            if (error)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            try
            {
                bl.Create<IBL.BO.Station>(station);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add Station for the following reason: \n" + ex.Message + "Please try again\n");
                return;
            }
            MessageBox.Show("Added Station!");
            this.Close();
        }

        private void DroneDoubleClick(object sender, MouseButtonEventArgs e) //refresh because drone might leave
        {
            if (DronesListView.SelectedItem != null)
                new DroneWindow(bl, bl.Request<IBL.BO.Drone>(((IBL.BO.DroneCharge)DronesListView.SelectedItem).Id)).Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateStation(station.Id, Name_s.Text); //can this ever return an exception?
            MessageBox.Show("Updated Station Name!");
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (station.Equals(default(IBL.BO.Station)))
            {
                station = bl.Request<IBL.BO.Station>(station.Id);
                DronesListView.ItemsSource = station.Charging; //getting list of drones to display
                DataContext = station;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
