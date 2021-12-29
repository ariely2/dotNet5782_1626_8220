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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "A new drone could not be created for the following reasons:";
            if(Validation.GetErrors(ID).Count()!=0)
            {
                errorMessage += "ID needs to be a string!\n";
                if (Validation.GetErrors(Model).Count() != 0)
                {
                    errorMessage += "Model needs to be a string!\n";
                    if (WeightSelector.SelectedItem == null)
                    {
                        errorMessage += "A Max Weight must be selected!\n";
                        if (StationSelector.SelectedItem == null)//maybe selected value?
                            errorMessage += "An Initial Station Must be Selected!\n";
                    }
                }
                errorMessage += "\nPlease Try Again.";
                MessageBox.Show(errorMessage);
            }
            drone.Location = bl.Request<IBL.BO.Station>((int)StationSelector.SelectedValue).location;
            try
            {
                bl.Create<IBL.BO.Drone>(drone);
            }
            catch (Exception ex)//consolewriteline or messagebox?
            {
                Console.WriteLine("Failed to add Drone for the following reason: \n" +
                ex.Message +
                "Try again\n");
                return;//clear textboxes?
            }
            Console.WriteLine("Added Drone!");
            DialogResult = true;//?
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
