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
using IBL;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        private IBL.IBL bl;
        public DroneListWindow()
        {
            InitializeComponent();
        }
        public DroneListWindow(IBL.IBL b)
        {
            bl = b;
            InitializeComponent();
            StatusSelector.Items.Add("All");
            var statuses  = Enum.GetValues(typeof(IBL.BO.DroneStatuses));
            foreach (var a in statuses)
                StatusSelector.Items.Add(a);
            WeightSelector.Items.Add("All");
            var weights = Enum.GetValues(typeof(IBL.BO.WeightCategories));
            foreach (var a in weights)
                WeightSelector.Items.Add(a);
            DronesListView.ItemsSource = bl.RequestList<IBL.BO.DroneToList>();
        }
        private void Selection(object sender, SelectionChangedEventArgs e)//grey out options that don't exist?
        {
            var a = StatusSelector.SelectedItem;
            var b = WeightSelector.SelectedItem;
            var c = bl.RequestList<IBL.BO.DroneToList>().ToList();
            c.RemoveAll(x => (a != null && a.GetType().IsEnum) ? x.Status != (IBL.BO.DroneStatuses)a : false);
            c.RemoveAll(x => (b != null && b.GetType().IsEnum) ? x.MaxWeight != (IBL.BO.WeightCategories)b : false);
            DronesListView.ItemsSource = c;
        }
    }
}
