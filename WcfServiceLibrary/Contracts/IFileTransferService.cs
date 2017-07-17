using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace WcfServiceLibrary.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required,
                 CallbackContract = typeof(IUploadProgressService))]
    public interface IFileTransferService
    {
        [OperationContract(IsOneWay = true)]
        //Task<string> UploadFile(UploadFileInfo file);
        void UploadFile(UploadFileInfo file);
    }

    [DataContract]
    public class UploadFault
    {
        [DataMember]
        public string Message { get; set; }
    }

    [MessageContract]
    public class UploadFileInfo
    {
        [MessageHeader]
        public int Lenght;

        [MessageHeader]
        public bool OperationIsCanceled;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileStream;

        public void Dispose()
        {
            if(FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }
        }
    }
}
