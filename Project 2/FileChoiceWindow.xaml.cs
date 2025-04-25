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
    /// Interaction logic for FileChoiceWindow.xaml
    /// </summary>
    public partial class FileChoiceWindow : Window
    {
        public string SelectedFile { get; private set; }
        public FileChoiceWindow(List<string> files)
        {
            InitializeComponent();
            lstFiles.ItemsSource = files;
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lstFiles.SelectedItem is string fileName)
            {
                SelectedFile = fileName;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
