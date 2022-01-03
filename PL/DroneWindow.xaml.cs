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
        private bool close = false;
        public DroneWindow(IBL.IBL b)
        {
            InitializeComponent();
            //DroneGrid.Visibility = Visibility.Hidden;
            bl = b;
            drone = new IBL.BO.Drone();
            drone.Location = new IBL.BO.Location();
            DataContext = drone;
            StationSelector.ItemsSource = bl.RequestList<IBL.BO.StationToList>().ToList().FindAll(x => x.Available != 0);
            WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
            //make default weight null!
        }

        public DroneWindow(IBL.IBL b, IBL.BO.Drone drone)
        {
            AddGrid.Visibility = Visibility.Hidden;//before or after initialize
            InitializeComponent();
            bl = b;
            this.drone = drone;
            DataContext = drone;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string errorMessage = "A new drone could not be created for the following reasons:\n";
            if (Validation.GetErrors(ID).Any())
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
            drone.Location = bl.Request<IBL.BO.Station>(((IBL.BO.StationToList)StationSelector.SelectedValue).Id).location;
            try
            {
                bl.Create<IBL.BO.Drone>(drone);
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
    }
}
