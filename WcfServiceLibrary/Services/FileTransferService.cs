using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WcfServiceLibrary.Contracts;
using WcfServiceLibrary.Repositories;

namespace WcfServiceLibrary.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class FileTransferService : IFileTransferService
    {
        byte[] buffer;
        System.IO.MemoryStream memoryStream;
        IUploadProgressService callBack = null;
        readonly IFileDatabaseRepository repo;
        CancellationTokenSource cancelSource;

        public FileTransferService()
        {
            repo = new FileDatabaseRepository();
        }

        public FileTransferService(IFileDatabaseRepository repository)
        {
            this.repo = repository;
        }

        public async void UploadFile(UploadFileInfo file)
        {
            cancelSource = new CancellationTokenSource();

            if (file.OperationIsCanceled)
            {                
                cancelSource.Cancel();
                var c = cancelSource.IsCancellationRequested;
            }

            callBack = OperationContext.Current.GetCallbackChannel<IUploadProgressService>();

            var task = Task.Factory.StartNew(() =>
            {
                //try
                //{
                    int bufferSize = file.Lenght;

                    if (memoryStream == null)
                    {
                        buffer = new byte[bufferSize];
                        memoryStream = new System.IO.MemoryStream(bufferSize);
                    }
                    Thread.Sleep(10000);
                    do
                    {
                        var readedBytes = file.FileStream.Read(buffer, 0, bufferSize);
                        if (readedBytes == 0) break;

                        memoryStream.Write(buffer, (int)memoryStream.Position, readedBytes);

                        var progress = ((int)memoryStream.Position * 100) / bufferSize;

                        callBack.SendProgress(progress);
                    }
                    while (true);
                 
                    var saved =  repo.SaveFile(memoryStream);
                    
                    buffer = null;
                    memoryStream.Close();

                //}
                //catch(Exception e)
                //{
                //    var fault = new UploadFault { Message = e.Message };
                //    //var reason = new FaultReason(e.Message);

                //    //throw new FaultException<UploadFault>(fault);
                //    return;
                //}
                
            },
            cancelSource.Token);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                //Вызов при отмене операции
                callBack.SendProgress(-1);               
            }         
        }
    }
}
