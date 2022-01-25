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

namespace PL
{
    /// <summary>
    /// Window for getting number of hours a drone was charging
    /// </summary>
    public partial class InputWindow : Window
    {
        public double time;
        public bool release = false;
        public InputWindow()
        {
            InitializeComponent();
        }
        public string GetHours()
        {
            return Hours.Text; 
        }

        private bool Check_Text()
        {
            double check = 0;
            if (Hours.Text == string.Empty)
            {
                MessageBox.Show("Please Enter Number of Hours!");
                return false;
            }
            if (double.TryParse(Hours.Text, out check)) //if a number was entered
            {
                if(check<0) 
                {
                    MessageBox.Show("Number of Hours can't be negative!\n Please Try again.");
                    return false;
                }
                time = check;
            }
            else
            {
                MessageBox.Show("Number of Hours must be a number!\n Please Try again.");
                return false;
            }
            return true;
        }

        private void Release(object sender, RoutedEventArgs e)
        {
            if (Check_Text()) //if a valid input was entered
            {
                release = true;
                this.Close();
            }
        }
    }
}
