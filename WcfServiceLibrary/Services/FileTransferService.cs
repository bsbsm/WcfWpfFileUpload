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
            cancelSource = new CancellationTokenSource();
        }

        public FileTransferService(IFileDatabaseRepository repository)
        {
            this.repo = repository;
            cancelSource = new CancellationTokenSource();
        }

        public async void UploadFile(UploadFileInfo file)
        {
            var cancelToken = cancelSource.Token;

            callBack = OperationContext.Current.GetCallbackChannel<IUploadProgressService>();

            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    int bufferSize = file.Length;

                    if (memoryStream == null)
                    {
                        buffer = new byte[bufferSize];
                        memoryStream = new System.IO.MemoryStream(bufferSize);
                    }

                    do
                    {
                        //Ожидлание для теста отмены задачи
                        //Thread.Sleep(1000);

                        if (cancelToken.IsCancellationRequested) throw new TaskCanceledException();

                        var readedBytes = file.FileStream.Read(buffer, 0, bufferSize);
                        if (readedBytes == 0) break;

                        memoryStream.Write(buffer, (int)memoryStream.Position, readedBytes);

                        var progress = ((int)memoryStream.Position * 100) / bufferSize;

                        callBack.SendProgress(progress);
                    }
                    while (true);

                    var saved = repo.SaveFile(memoryStream);
                }
                catch (TaskCanceledException)
                {
                    //Вызов при отмене операции
                    callBack.SendProgress(-1);
                }
                finally
                {
                    buffer = null;
                    memoryStream.Close();
                }
            },
            cancelToken);

            await task.ConfigureAwait(false);
        }

        public void CancelUploadOperation(bool operationIsCanceled)
        {
            if (operationIsCanceled)
            {
                cancelSource.Cancel();
            }
        }
    }
}
