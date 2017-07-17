using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary.Contracts;
using WcfServiceLibrary.Repositories;

namespace WcfServiceLibrary.Services
{
    /// <summary>
    /// Как альтернатива Castle.Windsor, но ServiceHost не создавался при передаче экземпляра сервиса (если класс не имеет конструктора по умолчанию). 
    /// Сейчас фабрика не используется.
    /// </summary>
    public static class ServiceFactory
    {
        public static IFileTransferService GetFileTranserService()
        {
            var repository = new FileDatabaseRepository();
            var service = new FileTransferService(repository);

            return service;
        }

        public static ISearchRowService GetSearchRowService()
        {
            var repository = new FileDatabaseRepository();
            var service = new SearchRowService(repository);

            return service;
        }
    }
}
