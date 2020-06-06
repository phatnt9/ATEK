using ATEK.AccessControl_2.Gates;
using ATEK.AccessControl_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace ATEK.AccessControl_2
{
    public static class ContainerHelper
    {
        private static IUnityContainer _container;

        static ContainerHelper()
        {
            _container = new UnityContainer();
            _container.RegisterType<IAccessControlRepository, AccessControlRepository>(
                new ContainerControlledLifetimeManager());
            _container.RegisterType<IFirebaseControlRepository, FirebaseControlRepository>(
               new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer Container
        {
            get { return _container; }
        }
    }
}