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
    /// Interaction logic for CLoginWindow.xaml
    /// </summary>
    public partial class CLoginWindow : Window
    {
        private IBL bl;
        public CLoginWindow(IBL b)
        {
            InitializeComponent();
            bl = b;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            int phone;
            string name = Name_c.Text;
            BO.Customer c;
            if (!int.TryParse(Phone_c.Text, out phone))
            {
                MessageBox.Show("Error: Phone Number must be an int!\nPlease Try Again.");
                return;
            }
            try
            {
                c = bl.Request<BO.Customer>(phone);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to login for the following reason: \n" + ex.Message + "Please try again\n");
                return;
            }
            if (c.Name.Equals(name))
            {
                new CustomerUIWindow(bl, c).Show();
                this.Close();
            }
        }
    }
}
