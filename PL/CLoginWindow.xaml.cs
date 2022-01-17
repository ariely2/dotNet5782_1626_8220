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
            //BO.Customer c;
            if (!int.TryParse(Phone_c.Text, out phone)) //if entered phone number isn't an int
            {
                MessageBox.Show("Error: Phone Number must be an int!\nPlease Try Again.");
                return;
            }
            var c = bl.RequestList<BO.CustomerToList>().ToList();
            var d = c.Find(x => x.Name.Equals(name) && x.Phone.Equals(Phone_c.Text));
            if (d != default(BO.CustomerToList)) //if a name with the matching phone number of a real customer was entered, the customer can login.
            {
                new CustomerUIWindow(bl, bl.Request<BO.Customer>(d.Id)).Show();
                this.Close();
            }
            else
                MessageBox.Show("A registered customer with the entered phone and name doesn't exist");
        }
    }
}
