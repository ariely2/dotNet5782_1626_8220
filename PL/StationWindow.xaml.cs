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
using BlApi;
namespace PL
{
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        BO.Station station;
        private IBL bl;
        public StationWindow(IBL b)
        {
            InitializeComponent();
            StationGrid.Visibility = Visibility.Hidden; //hiding other grid
            bl = b;
            station = new BO.Station();
            station.location = new BO.Location(); //initializing location so it won't be null
            DataContext = station;
        }
        public StationWindow(IBL b, BO.StationToList s)
        {
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden; //hiding other grid
            bl = b;
            station = bl.Request<BO.Station>(s.Id); 
            DataContext = station;
            DronesListView.ItemsSource = station.Charging; //getting list of drones to display
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string errorMessage = "A new station could not be created for the following reasons:\n";
            if (Validation.GetErrors(ID).Any()) //adding errors to error message
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
            if (error) //if there was an error, show error message
            {
                MessageBox.Show(errorMessage);
                return;
            }
            try
            {
                bl.Create<BO.Station>(station);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add Station for the following reason: \n" + ex.Message + "Please try again\n");
                return;
            }
            MessageBox.Show("Added Station!");
            this.Close();
        }

        private void DroneDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DronesListView.SelectedItem != null) //if a drone was selected
                new DroneWindow(bl, bl.Request<BO.Drone>(((BO.DroneCharge)DronesListView.SelectedItem).Id)).Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateStation(station.Id, Name_s.Text);
            MessageBox.Show("Updated Station Name!");
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (AddGrid.Visibility == Visibility.Hidden) //if we're in station display window
            {
                station = bl.Request<BO.Station>(station.Id); //getting updated version of station
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
