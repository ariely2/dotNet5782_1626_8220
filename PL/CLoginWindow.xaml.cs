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

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="b">the bl layer</param>
        public CLoginWindow(IBL b)
        {
            InitializeComponent();
            bl = b;
        }

        /// <summary>
        /// this function check if the customer enter valid id.
        //  if the id isn't exist, the function show a message box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            int id;
            //BO.Customer c;
            if (!int.TryParse(Id_c.Text, out id)) //if entered phone number isn't an int
            {
                MessageBox.Show("Error: Id must be an int!\nPlease Try Again.");
                return;
            }
            BO.Customer c;
            try
            {
                c = bl.Request<BO.Customer>(id);
                new CustomerUIWindow(bl, c).Show();
            }
            catch (BO.NotExistException ex)//throw an exception that the customer isn't exist
            {
                MessageBox.Show($"Error: A registered customer with the entered id: ({Id_c.Text}), doesn't exist\n");
            }

            this.Close();
        }
    }
}
