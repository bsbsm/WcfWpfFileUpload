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
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Content = new UploadPage { DataContext = new UploadViewModel() };
        }

        private void menuUpload_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new UploadPage { DataContext = new UploadViewModel() };
        }

        private void menuSearchText_Click(object sender, RoutedEventArgs e)
        {
            var page = new SearchPage { DataContext = new SearchViewModel() };
            MainFrame.Content = page;

            //this.Width = page.Width;
            //this.Height = page.Height + MenuBar.Height;
        }
    }

}
