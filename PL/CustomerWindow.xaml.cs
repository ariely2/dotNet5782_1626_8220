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
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        BO.Customer customer;
        private IBL bl;
        public CustomerWindow(IBL b, BO.Customer c)
        {
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden; //hiding other grid
            bl = b;
            customer = c;
            DataContext = customer;
            FromListView.ItemsSource = customer.From.ToList(); //getting list of parcels to display
            ToListView.ItemsSource = customer.To.ToList(); //getting list of parcels to display
        }
        public CustomerWindow(IBL b)
        {
            InitializeComponent();
            CustomerGrid.Visibility = Visibility.Hidden; //hiding other grid
            bl = b;
            customer = new BO.Customer() { location = new BO.Location() };
            DataContext = customer;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string errorMessage = "A new customer could not be created for the following reasons:\n";
            if (Validation.GetErrors(ID).Any()) //adding errors to error message
            {
                error = true;
                errorMessage += "ID needs to be an int!\n";
            }
            if (Validation.GetErrors(Latitude).Any())
            {
                error = true;
                errorMessage += "Latitude needs to be a number!\n";
            }
            if (Validation.GetErrors(Longitude).Any())
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
                bl.Create<BO.Customer>(customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add Customer for the following reason: \n" + ex.Message + "Please try again\n");
                return;
            }
            MessageBox.Show("Added Customer!");
            this.Close();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            bl.UpdateCustomer(customer.Id, Name_c.Text); //can this ever return an exception?
            MessageBox.Show("Updated Customer Name!");
        }
        private void FromDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FromListView.SelectedItem != null)
                new ParcelWindow(bl, bl.Request<BO.Parcel>(((BO.ParcelAtCustomer)FromListView.SelectedItem).Id)).Show();
        }

        private void ToDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(ToListView.SelectedItem != null)
                new ParcelWindow(bl, bl.Request<BO.Parcel>(((BO.CustomerParcel)ToListView.SelectedItem).Id)).Show();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //unhandled bug, cannot add customer
        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                customer = bl.Request<BO.Customer>(customer.Id); //getting updated customer
                FromListView.ItemsSource = customer.From.ToList(); //getting list of parcels to display
                ToListView.ItemsSource = customer.To.ToList(); //getting list of parcels to display
            }
            catch(Exception ex)
            {

            }
        }
    }
}
