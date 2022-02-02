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
    /// Window for the user to enter the secret code, in order to open the list window
    /// </summary>
    public partial class CodeWindow : Window
    {
        public bool correct = false;
        //tries to enter the code
        public int tries = 3;
        //secret code
        public string secret = "1121";

        /// <summary>
        /// contructor
        /// </summary>
        public CodeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// the fucntion responsible to check if the admin enter the correct code/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            //if the customer didn't enter a code at all
            if (Code.Text == string.Empty)
                MessageBox.Show("Please Enter Code!");
            else
            {
                //if the user enter the correct code
                if (Code.Text == secret)
                {
                    correct = true;
                    this.Close();
                }
                else
                {
                    tries--; //updating number of tries left
                    MessageBox.Show("Wrong Code Entered: " + tries + " tries left.");

                    //if the user failed to guess the secret code 3 times, the window shut down
                    if (tries == 0)
                        this.Close();

                }    
            }
        }
    }
}
