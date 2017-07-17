using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary.Contracts;
using WcfServiceLibrary.Repositories;
using WcfServiceLibrary.Services;

namespace WindowsService
{
    public partial class WindowsServiceHost : ServiceBase
    {
        IWindsorContainer _container;

        internal static ServiceHost hostFileTransfer;
        internal static ServiceHost hostSearchRow;

        public WindowsServiceHost()
        {
            _container = new WindsorContainer();

            InitializeComponent();          
        }

        protected override void OnStart(string[] args)
        {
            ConfigureContainer(_container);

            CloseAllHosts();

            //new FileTransferService(new WcfServiceLibrary.Repositories.FileDatabaseRepository()),
            //    new SearchRowService(new WcfServiceLibrary.Repositories.FileDatabaseRepository())

            //var ftService = ServiceFactory.GetFileTranserService();
            //var srService = ServiceFactory.GetSearchRowService();

            hostFileTransfer = new ServiceHost(typeof(IFileTransferService));
            hostSearchRow = new ServiceHost(typeof(ISearchRowService));

            OpenAllHosts();
        }

        protected override void OnStop()
        {
            CloseAllHosts(true);
        }

        private void OpenAllHosts()
        {
            hostFileTransfer.Open();
            hostSearchRow.Open();
        }

        private void CloseAllHosts(bool nullableAllHosts = false)
        {
            if (hostFileTransfer != null)
            {
                hostFileTransfer.Close();

                if(nullableAllHosts) hostFileTransfer = null;
            }

            if (hostSearchRow != null)
            {
                hostSearchRow.Close();

                if (nullableAllHosts) hostSearchRow = null;
            }
        }

        private void ConfigureContainer(IWindsorContainer container)
        {
            container.AddFacility<WcfFacility>()
              .Register(
              Component.For<IFileDatabaseRepository>().ImplementedBy<FileDatabaseRepository>(),
              Component.For<IFileTransferService>().ImplementedBy<FileTransferService>().AsWcfService(),
              Component.For<ISearchRowService>().ImplementedBy<SearchRowService>().AsWcfService()
              );
        }
    }
}
