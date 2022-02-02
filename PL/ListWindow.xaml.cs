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
using Mapsui.Utilities;
using Mapsui.Layers;
using Mapsui.UI.Wpf;
using Mapsui.Providers;
using Mapsui.Styles;

namespace PL
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : Window
    {
        private IBL bl;
        private bool close = false;
        public ListWindow(IBL b) //initializing all the tabs, lists, and controls
        {
            bl = b;
            InitializeComponent();
            //initialize the map
            InitializeMap();
            
            StatusSelector.Items.Add("All"); //adding all status and weight options for drone list. do we need this line?
            var statuses = Enum.GetValues(typeof(BO.DroneStatuses));
            foreach (var a in statuses)
                StatusSelector.Items.Add(a);
            WeightSelector.Items.Add("All");//do we need this line?
            var weights = Enum.GetValues(typeof(BO.WeightCategories));
            foreach (var a in weights)
                WeightSelector.Items.Add(a);
            Priority_p.Items.Add("All"); //adding priority, weight and status options for parcel list
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

            if (Tabs.SelectedIndex == 0) //updating the current tab if the window came back to focus
                Drone_Filter();
            else if (Tabs.SelectedIndex == 1)
                Filter_Stations();
            else if (Tabs.SelectedIndex == 2)
                CustomerListView.ItemsSource = bl.RequestList<BO.CustomerToList>();
            else if (Tabs.SelectedIndex == 3)
                Parcel_Filter();
            else if (Tabs.SelectedIndex == 4)
                InitializeMap();
        }
        private void Tab_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl) //updating selected tab when switching from another tab
            {
                if (Tabs.SelectedIndex == 0)
                    Drone_Filter();
                else if (Tabs.SelectedIndex == 1)
                    Filter_Stations();
                else if (Tabs.SelectedIndex == 2)
                    CustomerListView.ItemsSource = bl.RequestList<BO.CustomerToList>();
                else if (Tabs.SelectedIndex == 3)
                    Parcel_Filter();
                else if (Tabs.SelectedIndex == 4)
                    InitializeMap();
            }
        }
        #region Drone
        private void AddDrone(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
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
            if (DronesListView.SelectedItem != null) //if a drone was selected
                new DroneWindow(bl, bl.Request<BO.Drone>(((BO.DroneToList)DronesListView.SelectedItem).Id)).Show();
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            DronesListView.Items.GroupDescriptions.Clear();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
            view.GroupDescriptions.Add(groupDescription); //adding a groupdescription to group drones according to status
        }
        #endregion Drone
        #region Station
        private void SortStationList()
        {
            if (Sort_Stations.IsChecked == true) //sorting stations by number of available slots
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(StationListView.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Available", ListSortDirection.Descending));
            }
        }

        private void StationDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StationListView.SelectedItem != null) //if a station was selected
                new StationWindow(bl, (BO.StationToList)StationListView.SelectedItem).Show();
        }


        private void Filter_Stations(object sender = null, RoutedEventArgs e = null)
        {
            var a = bl.RequestList<BO.StationToList>().ToList();
            if (Only_Available.IsChecked == true) //if only stations with available slots should be displayed
            {
                a.Clear();
                foreach (var s in StationListView.Items) //adding to a list every station with available slots
                {
                    if (((BO.StationToList)s).Available != 0)
                        a.Add((BO.StationToList)s);
                }
            }
            StationListView.ItemsSource = a; //updating the item source
            SortStationList();
        }
        private void All_Stations(object sender, RoutedEventArgs e) //when the station filter is deactivated
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
            if (CustomerListView.SelectedItem != null) //if a customer was selected
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
            if (ParcelListView.SelectedItem != null) //if a parcel was selected
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
            view.GroupDescriptions.Add(groupDescription); //adding a groupdescription to group parcels according to sender
        }

        private void Group_Receiver(object sender, RoutedEventArgs e)
        {
            ParcelListView.Items.GroupDescriptions.Clear();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ReceiverName");
            view.GroupDescriptions.Add(groupDescription); //adding a groupdescription to group parcels according to receiver
        }

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
        #endregion Parcel
        #region Map

        //constant to calculate point on mapsui
        private const double Radius = 6378137;
        private const double E = 0.0000000848191908426;
        private const double D2R = Math.PI / 180;
        private const double PiDiv4 = Math.PI / 4;

        /// <summary>
        /// cast longitude and latitude to points
        /// </summary>
        /// <param name="lon">longitude of the point</param>
        /// <param name="lat">latitude of the point</param>
        /// <returns>return point of mapsui</returns>
        internal static Mapsui.Geometries.Point FromLonLat(double lon, double lat) //converts from Lon,Lat to X,Y
        {
            var lonRadians = D2R * lon;
            var latRadians = D2R * lat;

            var x = Radius * lonRadians;
            var y = Radius * Math.Log(Math.Tan(PiDiv4 + latRadians * 0.5) / Math.Pow(Math.Tan(PiDiv4 + Math.Asin(E * Math.Sin(latRadians)) / 2), E));

            return new Mapsui.Geometries.Point(x, y);
        }

        /// <summary>
        /// initialize map with stations, drones and cutomers
        /// </summary>
        private void InitializeMap()
        {
            //initialize map sui
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapControl.Map.BackColor = Mapsui.Styles.Color.FromArgb(255, 171, 210, 223);

            //set default location of the area
            var bbox = new Mapsui.Geometries.BoundingBox(FromLonLat(37, 37.1), FromLonLat(37, 37.1));
            MapControl.Navigator.NavigateTo(bbox, ScaleMethod.Fit);
            MapControl.Navigator.ZoomTo(40, 0);
            MapControl.Refresh();

            //clear the map
            foreach (var layer in MapControl.Map.Layers.Skip(1))
            {
                MapControl.Map.Layers.Remove(layer);
            }
            //get id of customers, stations and drones
            IEnumerable<int> customersId = from customer in bl.RequestList<BO.CustomerToList>() select customer.Id;
            IEnumerable<int> stationsId = from station in bl.RequestList<BO.StationToList>() select station.Id;
            IEnumerable<int> dronesId = from drone in bl.RequestList<BO.DroneToList>() select drone.Id;

            //get points of drones, stations and customers
            IEnumerable<BO.Location> DronePoints = from drone in bl.RequestList<BO.DroneToList>() select drone.Location;
            IEnumerable<BO.Location> StationPoints = from station in bl.RequestList<BO.StationToList>() let d = bl.Request<BO.Station>(station.Id) select d.Location;
            IEnumerable<BO.Location> CustomerPoints = from customer in bl.RequestList<BO.CustomerToList>() let d = bl.Request<BO.Customer>(customer.Id) select d.Location;


            //set new layer
            var ly = new Mapsui.Layers.WritableLayer { Style = null };
            //add point of the station to the layer
            for (int i = 0; i < StationPoints.Count(); i++)
            {
                ly.Add(CreateFeature(StationPoints.Skip(i).First().Longitude, StationPoints.Skip(i).First().Latitude, stationsId.Skip(i).First()));
            }
            //add the layer to the map
            MapControl.Map.Layers.Add(ly);

            ly = new Mapsui.Layers.WritableLayer { Style = null };
            for (int i = 0; i < CustomerPoints.Count(); i++)
            {
                ly.Add(CreateFeature(CustomerPoints.Skip(i).First().Longitude, CustomerPoints.Skip(i).First().Latitude, customersId.Skip(i).First()));
            }
            MapControl.Map.Layers.Add(ly);
            //small value to avoid drone id hidden by other id( station, customer)
            double a = 0.00001;
            for (int i = 0; i < DronePoints.Count(); i++)
            {
                ly.Add(CreateFeature(DronePoints.Skip(i).First().Longitude + 0.00001, DronePoints.Skip(i).First().Latitude + a, dronesId.Skip(i).First()));
                a += 0.000005;
            }
            MapControl.Map.Layers.Add(ly);

            ly = new Mapsui.Layers.WritableLayer { Style = null };
            //refresh the map
            MapControl.Refresh();

        }
        /// <summary>
        /// create feature to the point
        /// </summary>
        /// <param name="Longitude">longitude of the point</param>
        /// <param name="Lattitude"> latitude of the point</param>
        /// <param name="Id">id of the object ( staiton, drone, custoemr)</param>
        /// <returns>feature of the point</returns>
        private static IFeature CreateFeature(double Longitude, double Lattitude, int Id)
        {
            Mapsui.Geometries.Point pt;
            Mapsui.Providers.Feature feature;
            Mapsui.Styles.LabelStyle x;
            Mapsui.Styles.Color BGColor;
            pt = FromLonLat(Longitude, Lattitude);

            feature = new Mapsui.Providers.Feature { Geometry = pt };
            if (Id.ToString().Length == 9)//customer - blue color
                BGColor = Mapsui.Styles.Color.Blue;
            else if (Id.ToString().Length == 5)//drone - green color
                BGColor = Mapsui.Styles.Color.Green;
            else//station - red color
                BGColor = Mapsui.Styles.Color.Red;
            x = new Mapsui.Styles.LabelStyle()
            {
                Text = Id.ToString(),
                ForeColor = BGColor,
                Halo = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Black, 1),
                HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Left,
                MaxWidth = 10

            };

            feature.Styles.Add(x);
            return feature;
        }

        private void Filter_Map(object sender, RoutedEventArgs e)
        {
            var b = sender as System.Windows.Controls.Primitives.ToggleButton;
            if (b.IsChecked == true)
            {
                if (b.Content.Equals("Station"))
                {
                    MapControl.Map.Layers[2].Enabled = false;
                    MapControl.Map.Layers[3].Enabled = false;
                }
                else if (b.Content.Equals("Customer"))
                {
                    MapControl.Map.Layers[1].;
                    MapControl.Map.Layers[3].Opacity = 0;
                }
                else
                {
                    MapControl.Map.Layers[1].Enabled = false;
                    MapControl.Map.Layers[2].Enabled = false;
                }
                // Code for Checked state
            }
            else
            {
                // Code for Un-Checked state
                MapControl.Map.Layers[1].Enabled = true;
                MapControl.Map.Layers[2].Enabled = true;
                MapControl.Map.Layers[3].Enabled = true;
            }
            #endregion Map
        }
    }
}
