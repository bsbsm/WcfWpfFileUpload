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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfClient.FileTransferServiceReference;

namespace WpfClient.Views
{
    /// <summary>
    /// Interaction logic for UploadPage.xaml
    /// </summary>
    public partial class UploadPage : Page
    {
        public UploadPage()
        {
            InitializeComponent();
        }

        //private void OpenFileDialog()
        //{
        //    var fileDialog = new Microsoft.Win32.OpenFileDialog();
        //    fileDialog.DefaultExt = ".txt"; 
        //    fileDialog.Filter = "Text documents (.txt)|*.txt";

        //    var result = fileDialog.ShowDialog();

        //    if(result == true)
        //    {
        //        filePath.Text = fileDialog.FileName;
        //    }
        //}
        //private void SendRequest()
        //{
        //    string path = filePath.Text;
        //    var file = new System.IO.FileStream(path ,System.IO.FileMode.Open);

        //    var data = GetUploadFileInfo(file);
        //    UploadFileAsync(data);
        //}
        //private UploadFileInfo GetUploadFileInfo(System.IO.Stream stream)
        //{
        //    var fileInfo = new UploadFileInfo();
        //    fileInfo.FileStream = stream;
        //    fileInfo.Lenght = (int)stream.Length;
        //    fileInfo.CancellationToken = new CancellationToken();

        //    return fileInfo;
        //}

        //private async void UploadFileAsync(UploadFileInfo fileInfo)
        //{
        //    var ftClient = new FileTransferServiceReference.FileTransferServiceClient(null);
        //    var chanel = ftClient.ChannelFactory.CreateChannel();

        //    await chanel.UploadFileAsync(fileInfo);
        //}
    }
}
