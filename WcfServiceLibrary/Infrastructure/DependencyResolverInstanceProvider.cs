using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.Infrastructure
{
    class DependencyResolverInstanceProvider : IInstanceProvider
    {
        private readonly Type _serviceType;
        private readonly WindsorContainer _container;

        public DependencyResolverInstanceProvider(Type serviceType)
        {
            this._container = new WindsorContainer();
            _container.Install(FromAssembly.Named("DependencyResolver"));

            _container.Register(Classes.FromThisAssembly());

            DTOs.DTOMapper.Initialize();
            this._serviceType = serviceType;
        }

        #region IInstanceProvider Members
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return _container.Resolve(_serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instance is IDisposable)
                ((IDisposable)instance).Dispose();
        }
        #endregion
    }
}
