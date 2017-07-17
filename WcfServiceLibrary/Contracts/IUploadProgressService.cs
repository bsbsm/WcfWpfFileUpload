using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.Contracts
{
    [ServiceContract]
    public interface IUploadProgressService
    {
        [OperationContract(IsOneWay = true)]
        void SendProgress(int uploadedPart);
    }
}
