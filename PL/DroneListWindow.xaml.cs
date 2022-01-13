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
using System.ComponentModel;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        private IBL bl;
        private bool close = false;
        public DroneListWindow(IBL b) //initializing all the tabs, lists, and controls
        {
            bl = b;
            InitializeComponent();
            StatusSelector.Items.Add("All"); //adding all status options. do we need this line?
            var statuses  = Enum.GetValues(typeof(BO.DroneStatuses));
            foreach (var a in statuses)
                StatusSelector.Items.Add(a);
            WeightSelector.Items.Add("All");//do we need this line?
            var weights = Enum.GetValues(typeof(BO.WeightCategories));
            foreach (var a in weights)
                WeightSelector.Items.Add(a);
            Priority_p.Items.Add("All");
            Status_p.Items.Add("All");
            Weight_p.Items.Add("All");
            var priorities = Enum.GetValues(typeof(BO.Priorities));
            foreach (var a in priorities)
                Priority_p.Items.Add(a);
            statuses = Enum.GetValues(typeof(BO.ParcelStatuses));
            foreach (var a in statuses)
                Status_p.Items.Add(a);
            weights = Enum.GetValues(typeof(BO.WeightCategories));
            foreach (var a in weights)
                Weight_p.Items.Add(a);
            DronesListView.ItemsSource = bl.RequestList<BO.DroneToList>(); //getting list of drones to display
            StationListView.ItemsSource = bl.RequestList<BO.StationToList>(); //getting list of stations to display
            CustomerListView.ItemsSource = bl.RequestList<BO.CustomerToList>(); //getting list of customers to display
            ParcelListView.ItemsSource = bl.RequestList<BO.ParcelToList>(); //getting list of parcels to display
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            close = true;
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!close)
                e.Cancel = true; //if "X" was clicked, don't close window.
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (Tabs.SelectedIndex == 0)
                Drone_Filter();
            else if (Tabs.SelectedIndex == 1)
                Filter_Stations();
            else if (Tabs.SelectedIndex == 2)
                CustomerListView.ItemsSource = bl.RequestList<BO.CustomerToList>();
            else if (Tabs.SelectedIndex == 3)
                Parcel_Filter();
        }

        private void Tab_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (Tabs.SelectedIndex == 0)
                    Drone_Filter();
                else if (Tabs.SelectedIndex == 1)
                    Filter_Stations();
                else if (Tabs.SelectedIndex == 2)
                    CustomerListView.ItemsSource = bl.RequestList<BO.CustomerToList>();
                else if (Tabs.SelectedIndex == 3)
                    Parcel_Filter();
            }
        }
        #region Drone
        private void AddDrone(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
            //DroneWindow d = new DroneWindow(bl);
            //d.Show();
            //while (!this.IsActive) ;
            //DronesListView.Items.Refresh();
        }
        private void Drone_Filter(object sender = null, SelectionChangedEventArgs e = null)//remove/grey out options that don't exist? //move selection to here with null
        {
            var a = StatusSelector.SelectedItem; //selected status
            var b = WeightSelector.SelectedItem; //selected max weight
            var c = bl.RequestList<BO.DroneToList>().ToList();
            // if no status was selected, or "all" was selected, don't remove any drone.
            // if a status was selected, remove drones whose status isn't the selected status:
            c.RemoveAll(x => (a != null && a.GetType().IsEnum) ? x.Status != (BO.DroneStatuses)a : false);
            // do the same for maxweight selection:
            c.RemoveAll(x => (b != null && b.GetType().IsEnum) ? x.MaxWeight != (BO.WeightCategories)b : false);
            DronesListView.ItemsSource = c;
        }
        private void DroneDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DronesListView.SelectedItem != null)
                new DroneWindow(bl, bl.Request<BO.Drone>(((BO.DroneToList)DronesListView.SelectedItem).Id)).Show();
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            DronesListView.Items.GroupDescriptions.Clear();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
            view.GroupDescriptions.Add(groupDescription);
        }
        #endregion Drone
        #region Station
        private void SortStationList()
        {
            if (Sort_Stations.IsChecked == true)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(StationListView.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Available", ListSortDirection.Descending));
            }
        }

        private void StationDoubleClick(object sender, MouseButtonEventArgs e) //make regions
        {
            if (StationListView.SelectedItem != null)
                new StationWindow(bl, (BO.StationToList)StationListView.SelectedItem).Show();
        }


        private void Filter_Stations(object sender = null, RoutedEventArgs e = null)
        {
            var a = bl.RequestList<BO.StationToList>().ToList();
            if (Only_Available.IsChecked == true)
            {
                a.Clear();
                foreach (var s in StationListView.Items)
                {
                    if (((BO.StationToList)s).Available != 0)
                        a.Add((BO.StationToList)s);
                }
            }
            StationListView.ItemsSource = a;
            SortStationList();
        }
        private void All_Stations(object sender, RoutedEventArgs e)
        {
            StationListView.ItemsSource = bl.RequestList<BO.StationToList>(); //getting list of stations to display
            SortStationList();
        }

        private void AddStation(object sender, RoutedEventArgs e)
        {
            new StationWindow(bl).Show();
        }
        #endregion Station
        #region Customer
        private void CustomerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CustomerListView.SelectedItem != null)
                new CustomerWindow(bl, bl.Request<BO.Customer>(((BO.CustomerToList)CustomerListView.SelectedItem).Id)).Show();
        }
        private void AddCustomer(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl).Show();
        }
        #endregion Customer
        #region Parcel
        private void ParcelDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParcelListView.SelectedItem != null)
                new ParcelWindow(bl, bl.Request<BO.Parcel>(((BO.ParcelToList)ParcelListView.SelectedItem).Id)).Show();
        }

        private void AddParcel(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bl).Show();
        }

        private void Group_Sender(object sender, RoutedEventArgs e)
        {
            ParcelListView.Items.GroupDescriptions.Clear();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SenderName");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void Group_Receiver(object sender, RoutedEventArgs e)
        {
            ParcelListView.Items.GroupDescriptions.Clear();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ReceiverName");
            view.GroupDescriptions.Add(groupDescription);
        }
        #endregion Parcel

        private void Parcel_Filter(object sender = null, SelectionChangedEventArgs e = null)
        {
            var a = Priority_p.SelectedItem; //selected priority
            var b = Status_p.SelectedItem; //selected  status
            var c = Weight_p.SelectedItem; //selected weight
            var d = bl.RequestList<BO.ParcelToList>().ToList();
            // if no priority was selected, or "all" was selected, don't remove any parcel.
            // if a priority was selected, remove parcels whose priority isn't the selected priority:
            d.RemoveAll(x => (a != null && a.GetType().IsEnum) ? x.Priority != (BO.Priorities)a : false);
            // do the same for weight and status selection:
            d.RemoveAll(x => (b != null && b.GetType().IsEnum) ? x.Status != (BO.ParcelStatuses)b : false);
            d.RemoveAll(x => (c != null && c.GetType().IsEnum) ? x.Weight != (BO.WeightCategories)c : false);
            ParcelListView.ItemsSource = d;
        }
    }
}
