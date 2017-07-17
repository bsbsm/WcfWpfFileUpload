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

        void UploadFile(UploadFileInfo file);
        [OperationContract(IsOneWay = true)]

        void CancelUploadOperation(bool operationIsCanceled);
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
        public int Length;

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
