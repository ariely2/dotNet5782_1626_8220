﻿using System;
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
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        BO.Parcel parcel;
        private IBL bl;

        public ParcelWindow(IBL b, BO.Parcel p)
        {
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden;
            bl = b;
            parcel = p;
            DataContext = parcel;
        }
        public ParcelWindow(IBL b)
        {
            InitializeComponent();
            ParcelGrid.Visibility = Visibility.Hidden;
            bl = b;
            parcel = new BO.Parcel();
            parcel.Sender = new BO.CustomerParcel();
            parcel.Receiver = new BO.CustomerParcel();
            DataContext = parcel;
            Weight.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
            Priority.ItemsSource = Enum.GetValues(typeof(BO.Priorities));
        }
        private void Erase(object sender, RoutedEventArgs e)
        {
            if (parcel.Scheduled == null)
            {
                bl.DeleteParcel(parcel.Id);
                MessageBox.Show("Deleted Parcel!");
            }
        }

        private void Open_Drone(object sender, RoutedEventArgs e)
        {
            if (parcel.Scheduled != null && parcel.Delivered == null)
                new DroneWindow(bl, bl.Request<BO.Drone>(parcel.Drone.Id)).Show();
        }

        private void Open_Sender(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, bl.Request<BO.Customer>(parcel.Sender.Id)).Show();
        }

        private void Open_Receiver(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl, bl.Request<BO.Customer>(parcel.Receiver.Id)).Show();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string errorMessage = "A new parcel could not be created for the following reasons:\n";
            if (Validation.GetErrors(Sender).Any())
            {
                error = true;
                errorMessage += "Sender ID needs to be an int!\n";
            }
            if (Validation.GetErrors(Receiver).Any())
            {
                error = true;
                errorMessage += "Receiver ID needs to be an int!\n";
            }
            if (Weight.SelectedValue == null)
            {
                error = true;
                errorMessage += "A Weight must be selected!\n";
            }
            if (Priority.SelectedItem == null)//maybe selected value?
            {
                error = true;
                errorMessage += "A Priority must be Selected!\n";
            }
            errorMessage += "\nPlease Try Again.";
            if (error)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            try
            {
                bl.Create<BO.Parcel>(parcel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add parcel for the following reason: \n" + ex.Message + "Please try again\n");
                return;
            }
            MessageBox.Show("Added Parcel!");
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
