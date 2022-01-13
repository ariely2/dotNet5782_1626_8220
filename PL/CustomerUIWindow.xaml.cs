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
    /// Interaction logic for CustomerUIWindow.xaml
    /// </summary>
    public partial class CustomerUIWindow : Window
    {
        private IBL bl;
        private BO.Customer customer;
        public CustomerUIWindow(IBL b, BO.Customer c)
        {
            InitializeComponent();
            bl = b;
            customer = c;
        }
    }
}
