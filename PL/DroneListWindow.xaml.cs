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
using IBL;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        private IBL.IBL bl;
        private bool close = false;
        public DroneListWindow(IBL.IBL b)
        {
            bl = b;
            InitializeComponent();
            StatusSelector.Items.Add("All"); //adding all status options. do we need this line?
            var statuses  = Enum.GetValues(typeof(IBL.BO.DroneStatuses));
            foreach (var a in statuses)
                StatusSelector.Items.Add(a);
            WeightSelector.Items.Add("All");//do we need this line?
            var weights = Enum.GetValues(typeof(IBL.BO.WeightCategories));
            foreach (var a in weights)
                WeightSelector.Items.Add(a);
            DronesListView.ItemsSource = bl.RequestList<IBL.BO.DroneToList>(); //getting list of drones to display
        }
        private void Selected_Filter(object sender, SelectionChangedEventArgs e)//remove/grey out options that don't exist?
        {
            Selection();
        }

        private void Selection()//remove/grey out options that don't exist?
        {
            var a = StatusSelector.SelectedItem; //selected status
            var b = WeightSelector.SelectedItem; //selected max weight
            var c = bl.RequestList<IBL.BO.DroneToList>().ToList();
            // if no status was selected, or "all" was selected, don't remove any drone.
            // if a status was selected, remove drones whose status isn't the selected status:
            c.RemoveAll(x => (a != null && a.GetType().IsEnum) ? x.Status != (IBL.BO.DroneStatuses)a : false);
            // do the same for maxweight selection:
            c.RemoveAll(x => (b != null && b.GetType().IsEnum) ? x.MaxWeight != (IBL.BO.WeightCategories)b : false);
            DronesListView.ItemsSource = c;
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            close = true;
            this.Close();
        }
        private void AddDrone(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
            //DroneWindow d = new DroneWindow(bl);
            //d.Show();
            //while (!this.IsActive) ;
            //DronesListView.Items.Refresh();
        }

        private void DroneDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new DroneWindow(bl, (IBL.BO.DroneToList)DronesListView.SelectedItem).Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!close)
                e.Cancel = true; //if "X" was clicked, don't close window.
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DronesListView.Items.Refresh();
            Selection();
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Ascending));
        }
    }
}
