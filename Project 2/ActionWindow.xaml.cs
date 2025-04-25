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

namespace Project_2
{
    /// <summary>
    /// Interaction logic for ActionWindow.xaml
    /// </summary>
    public partial class ActionWindow : Window
    {
        public List<string> SelectedItems { get; private set; } = new List<string>();

        public ActionWindow(string title, string message)
        {
            InitializeComponent();
            txtTitle.Text = title;
            txtMessage.Text = message;
            // Load default cohorts
            lstItems.ItemsSource = Models.Constraints.Constraints.DefaultCohorts;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedItems = lstItems.SelectedItems.Cast<string>().ToList();
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
