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
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        BO.Parcel a;
        BO.ParcelDeliver b;
        BO.ParcelAtCustomer c;

        public ParcelWindow(BO.Parcel p)
        {
            InitializeComponent();
            a = p;
        }
        public ParcelWindow(BO.ParcelDeliver p)
        {
            InitializeComponent();
            b = p;
            DataContext = b;
            Sender.Text = b.Sender.Id.ToString();
            Receiver.Text = b.Receiver.Id.ToString();
        }
        public ParcelWindow(BO.ParcelAtCustomer p, int id)
        {
            InitializeComponent();
            c = p;
        }
    }
}
