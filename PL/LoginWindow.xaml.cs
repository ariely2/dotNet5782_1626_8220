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
    /// login window for DelivAir.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static IBL bl = BlApi.BlFactory.GetBl();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Employee_log(object sender, RoutedEventArgs e)
        {
            var w = new CodeWindow();
            if (w.ShowDialog() == false && w.correct) //if the correct code was entered, open list window
                new ListWindow(bl).Show();
            else
                MessageBox.Show("The Correct Code was not Entered");
        }

        private void Customer_log(object sender, RoutedEventArgs e)
        {
            new CLoginWindow(bl).Show();
        }

        private void Customer_sign(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl).Show();
        }
    }
}
