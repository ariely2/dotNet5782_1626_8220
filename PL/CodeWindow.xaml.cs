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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CodeWindow : Window
    {
        public bool correct = false;
        public int tries = 3;
        public CodeWindow()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (Code.Text == string.Empty)
                MessageBox.Show("Please Enter Code!");
            else
            {
                var w = new LoginWindow();
                string s = w.secret;
                if (Code.Text == s)
                {
                    correct = true;
                    this.Close();
                }
                else
                {
                    tries--;
                    MessageBox.Show("Wrong Code Entered: " + tries + " tries left.");
                    if (tries == 0)
                        this.Close();

                }    
            }
        }
    }
}
