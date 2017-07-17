using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfClient.FileTransferServiceReference;

namespace WpfClient.ViewModels
{
    public class UploadViewModel : ViewModelBase, IFileTransferServiceCallback
    {
        //backgroungworker
        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged("FilePath");
                }
            }
        }
        private string _uploadOnfprmation;
        public string UploadInformation
        {
            get
            {
                return _uploadOnfprmation;
            }
            set
            {
                if (_uploadOnfprmation != value)
                {
                    _uploadOnfprmation = value;
                    OnPropertyChanged("UploadInformation");
                }
            }
        }

        public ICommand OpenCommand { get; set; }
        public ICommand UploadCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private int _progressMin;
        public  int ProgressMin
        {
            get
            {
                return _progressMin;
            }
            set
            {
                if (_progressMin != value)
                {
                    _progressMin = value;
                    OnPropertyChanged("TextProperty1");
                }
            }
        }
        private int _progressMax;
        public int ProgressMax
        {
            get
            {
                return _progressMax;
            }
            set
            {
                if (_progressMax != value)
                {
                    _progressMax = value;
                    OnPropertyChanged("ProgressMax");
                }
            }
        }
        private int _progressValue;
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged("ProgressValue");

                    if (_progressValue == ProgressMax) UploadInformation = "Файл загружен";
                }
            }
        }

        private bool UploadIsCanceled { get; set; }
        private IFileTransferService channel { get; set; }
        public UploadViewModel ()
        {
            ProgressMin = 0;
            ProgressMax = 100;
            ProgressValue = ProgressMin;
            UploadIsCanceled = false;

            FilePath = "Выберите файл";
            OpenCommand = new RelayCommand(param => this.OpenFileDialog());
            UploadCommand = new RelayCommand(param => this.SendRequest());
            CancelCommand = new RelayCommand(param => this.CancelFileUpload());
        }

        private void OpenFileDialog()
        {
            
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = ".txt";
            fileDialog.Filter = "Text documents (.txt)|*.txt";

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                FilePath = fileDialog.FileName;
            }
        }
        private void SendRequest()
        {
            if(!String.IsNullOrWhiteSpace(FilePath) && FilePath.EndsWith(".txt"))
            {

                ProgressValue = ProgressMin;
                UploadInformation = String.Empty;

                var file = new System.IO.FileStream(FilePath, System.IO.FileMode.Open);
                    if (file != null)
                    {                                       
                            var data = GetUploadFileInfo(file);
                            UploadFileAsync(data);                                               
                    }
                    else
                    {
                        UploadInformation = "Выберите корректный файл";
                    }                       
            }      

        }
        private UploadFileInfo GetUploadFileInfo(System.IO.Stream stream)
        {
            var fileInfo = new UploadFileInfo();
            fileInfo.FileStream = stream;
            fileInfo.Length = (int)stream.Length;

            return fileInfo;
        }

        private async void UploadFileAsync(UploadFileInfo fileInfo)
        {
            var context = new InstanceContext(this);
            var ftClient = new FileTransferServiceReference.FileTransferServiceClient(context);
            channel = ftClient.ChannelFactory.CreateChannel();
            
            await channel.UploadFileAsync(fileInfo);
        }

        //private async void UploadFileAsyncCancellation()
        //{         
        //    var fileInfo = new UploadFileInfo { OperationIsCanceled = UploadIsCanceled, FileStream = new System.IO.MemoryStream() };
        //    await channel.UploadFileAsync(fileInfo);
        //}

        public void SendProgress(int value)
        {
            if(value > 0)
            {
                ProgressValue = value;
            }
            else
            {
                ProgressValue = ProgressMin;
                UploadInformation = "Загрузка отменена";
            }
            
        }

        private void CancelFileUpload()
        {
            UploadIsCanceled = true;

            if(channel != null)
            {
                channel.CancelUploadOperation(UploadIsCanceled);
            }          
        }
    }
}
