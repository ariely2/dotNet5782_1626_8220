using System;
//using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using System.ComponentModel;
using BlApi;
namespace PL
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        BO.Drone drone;
        private IBL bl;
        internal BackgroundWorker worker;
        private bool close = false;
        public DroneWindow(IBL b)
        {
            InitializeComponent();
            DroneGrid.Visibility = Visibility.Hidden; //hiding other grid
            bl = b;
            drone = new BO.Drone();
            drone.Location = new BO.Location(); //do we need this line?
            DataContext = drone;
            StationSelector.ItemsSource = bl.RequestList<BO.StationToList>().ToList().FindAll(x => x.Available != 0); //getting all stations with available slots
            WeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
        }

        public DroneWindow(IBL b, BO.Drone d)
        {
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden; //hiding other grid
            bl = b;
            this.drone = d;
            DataContext = d;
            Parcel_Details();
            Charging.Items.Add("Send To Charge");
            Charging.Items.Add("Release From Charge");
            Delivery.Items.Add("Open Parcel Window");
            Delivery.Items.Add("Assign a Parcel");
            Delivery.Items.Add("Pick Up Parcel");
            Delivery.Items.Add("Deliver Parcel");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string errorMessage = "A new drone could not be created for the following reasons:\n";
            if (Validation.GetErrors(ID).Any()) //adding errors to error message
            {
                error = true;
                errorMessage += "ID needs to be an int!\n";
            }
            //if (Validation.GetErrors(Model).Any())//what if nothing was entered to id/model fields: for now it's ok cuz there's 0 and string...
            //{
            //    error = true;
            //    errorMessage += "Model needs to be a string!\n";
            //}
            if (WeightSelector.SelectedValue == null)
            {
                error = true;
                errorMessage += "A Max Weight must be selected!\n";
            }
            if (StationSelector.SelectedItem == null)//maybe selected value?
            {
                error = true;
                errorMessage += "An Initial Station must be Selected!\n"; 
            }
            errorMessage += "\nPlease Try Again.";
            if (error)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            drone.Location = bl.Request<BO.Station>(((BO.StationToList)StationSelector.SelectedValue).Id).Location; //getting initial station's location
            try
            {
                bl.Create<BO.Drone>(drone);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add Drone for the following reason: \n" + ex.Message + "Please try again\n");
                return;
            }
            MessageBox.Show("Added Drone!");
            close = true;
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            close = true;
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!close)
                e.Cancel = true;
        }

        private void Update(object sender, RoutedEventArgs e)//can't update model twice (doesn't really update?
        {
            bl.UpdateDrone(drone.Id, Model_d.Text); //can this ever return an exception?
            MessageBox.Show("Updated Drone Model!");
        }

        private void Charge_Update(object sender, SelectionChangedEventArgs e)
        {
            bool s = false;
            if (Charging.SelectedValue == "Send To Charge")
            {
                try
                {
                    bl.SendDroneToCharge(drone.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send Drone to charge for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Sent Drone To Charge!");
                s = true;
            }
            if (Charging.SelectedValue == "Release From Charge")
            {
                try
                {
                    bl.ReleaseDrone(drone.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Release Drone from charge for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Released Drone from Charge!");
                s = true;
            }
            if (s) //if the drone's data was changed within the window, get updated drone
            {
                int id = drone.Id;
                drone = bl.Request<BO.Drone>(id);
                DataContext = drone;
            }
        }
        private void Parcel_Details()
        {
            if (drone.Parcel != null)
            {
                Parcel.Text = drone.Parcel.Id.ToString();
                Priority.Text = drone.Parcel.Priority.ToString();
                Weight.Text = drone.Parcel.Weight.ToString();
                Distance.Text = Math.Round(drone.Parcel.Distance, 3).ToString();
            }
            else
            {
                Parcel.Text = "N/A";
                Priority.Text = "N/A";
                Weight.Text = "N/A";
                Distance.Text = "N/A";
            }
        }

        private void Delivery_Update(object sender, SelectionChangedEventArgs e)
        {
            bool s = false;
            if(Delivery.SelectedValue == "Open Parcel Window")
            {
                if (drone.Parcel != null) //if the drone is assigned to a parcel
                    new ParcelWindow(bl, bl.Request<BO.Parcel>(drone.Parcel.Id)).Show();
                else
                    MessageBox.Show("There's no parcel assigned to the drone");
            }
            if (Delivery.SelectedValue == "Assign a Parcel")
            {
                try
                {
                    bl.AssignDrone(drone.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Assign Parcel to Drone for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Assigned Parcel to Drone!");
                s = true;
            }
            if (Delivery.SelectedValue == "Pick Up Parcel")
            {
                try
                {
                    bl.PickUp(drone.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Pick Up Parcel for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Picked Up Parcel!");
                s = true;
            }
            if (Delivery.SelectedValue == "Deliver Parcel")
            {
                try
                {
                    bl.Deliver(drone.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Deliver Parcel for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Delivered Parcel!");
                s = true;
            }
            if(s) //if the drone's data was changed within the window, get updated drone
            { 
                int id = drone.Id;
                drone = bl.Request<BO.Drone>(id);
                DataContext = drone;
                Parcel_Details();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            close = true;
            this.Close();
        }

        private void Delivery_DropDownClosed(object sender, EventArgs e) //when combobox dropdown is closed, change the displayed selected item 
        {
            Delivery.SelectedItem = Delivery.Items.GetItemAt(0);
        }

        private void Charging_DropDownClosed(object sender, EventArgs e)
        {
            Charging.SelectedItem = Charging.Items.GetItemAt(0);
        }

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_Progress;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            Close.Visibility = Visibility.Hidden;
            Update_m.Visibility = Visibility.Hidden;
            Charging.Visibility = Visibility.Hidden;
            Delivery.Visibility = Visibility.Hidden;
            Simulator.Visibility = Visibility.Hidden;
            Normal.Visibility = Visibility.Visible;
            worker.RunWorkerAsync(); //maybe create simulator?
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bl.Simulator(drone.Id, ()=> worker.ReportProgress(1), ()=> worker.CancellationPending);
        }
        private void Worker_Progress(object sender, ProgressChangedEventArgs e)
        {
            int id = drone.Id;
            drone = bl.Request<BO.Drone>(id);
            DataContext = drone;
            Parcel_Details();
        }

        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
            Close.Visibility = Visibility.Visible;
            Update_m.Visibility = Visibility.Visible;
            Charging.Visibility = Visibility.Visible;
            Delivery.Visibility = Visibility.Visible;
            Simulator.Visibility = Visibility.Visible;
            Normal.Visibility = Visibility.Hidden;
        }
    }
}
