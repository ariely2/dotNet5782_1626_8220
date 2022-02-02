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
    /// Interaction logic for CustomerUIWindow.xaml
    /// the window show the customer personal info and deliveries from/to him
    /// </summary>
    public partial class CustomerUIWindow : Window
    {
        //bl layer
        private IBL bl;
        //custome
        private BO.Customer customer;
        public CustomerUIWindow(IBL b, BO.Customer c)
        {
            InitializeComponent();
            bl = b;
            customer = c;
            Title.Content = customer.Name + "'s Deliveries"; //creating title based on customer's name
            DataContext = customer;
            FromListView.ItemsSource = customer.From.ToList(); //getting list of parcels to display
            ToListView.ItemsSource = customer.To.ToList(); //getting list of parcels to display
        }

        /// <summary>
        /// if the user double click a parcel that was sent from him, the parcel get picked up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FromListView.SelectedItem == null) //if a parcel was not selected
                return;
            string message = "Do you want this parcel to get picked up?";
            string title = "Pick Up";
            MessageBoxButton buttons = MessageBoxButton.YesNo;// a window with yes/no options
            var p = bl.Request<BO.Parcel>(((BO.ParcelAtCustomer)FromListView.SelectedItem).Id);
            if (MessageBox.Show(message, title, buttons) == MessageBoxResult.Yes) //if "yes" was pressed
            {
                try
                {
                    bl.CustomerPickUp(p.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Pick Up Parcel for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Picked Up Parcel!");
            }
        }

        /// <summary>
        /// if the user double click a parcel that intendent to him, then the function deliver this parcel to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ToListView.SelectedItem == null)
                return;
            string message = "Do you want this parcel to be delivered to you?";
            string title = "Deliver";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            var p = bl.Request<BO.Parcel>(((BO.ParcelAtCustomer)ToListView.SelectedItem).Id);
            if (MessageBox.Show(message, title, buttons) == MessageBoxResult.Yes)
            {
                try
                {
                    bl.CustomerDeliver(p.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Deliver Parcel for the following reason: \n" + ex.Message);
                    return;
                }
                MessageBox.Show("Delivered Parcel!");
            }
        }
        /// <summary>
        /// when the window is activated, we refresh the data in the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            customer = bl.Request<BO.Customer>(customer.Id); //getting updated customer
            FromListView.ItemsSource = customer.From.ToList(); //getting list of parcels to display
            ToListView.ItemsSource = customer.To.ToList(); //getting list of parcels to display
        }

        /// <summary>
        /// fucntion that send a parcel from the customer to another customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddParcel(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bl).Show(); //opening window to add parcel
        }
    }
}
