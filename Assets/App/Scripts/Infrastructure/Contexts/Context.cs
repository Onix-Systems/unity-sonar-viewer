
using System.Collections.Generic;
using UnityEngine;
using App.Infrastructure.CommonInterfaces;

namespace App.Infrastructure.Contexts
{
    public abstract class Context : MonoBehaviour, IContext
    {
        private List<object> _services = new List<object>();

        protected void Awake()
        {
            Initialize();
        }

        protected void OnDestroy()
        {
            Dispose();
        }

        public void Initialize()
        {
            MainContext.Instance.RegisterContext(this);

            SetupServices(out _services);
            RegisterServices();
            InitializeServices();

            OnInitialized();
        }

        public void Dispose()
        {
            ReleaseServices();
            MainContext.Instance.UnregisterContext(this);

            OnDisposed();
        }

        public TService Get<TService>() where TService : class => MainContext.Instance.Get<TService>();
        public bool TryGet<TService>(out TService service) where TService : class => MainContext.Instance.TryGet(out service);

        protected abstract void SetupServices(out List<object> services);
        protected virtual void OnInitialized() { }
        protected virtual void OnDisposed() { }

        private void RegisterServices()
        {
            foreach (object service in _services)
            {
                MainContext.Instance.ServiceLocator.Register(service.GetType(), service);
            }
        }

        private void InitializeServices()
        {
            foreach (object service in _services)
            {
                (service as IInitializable)?.Initialize();
            }
        }

        private void ReleaseServices()
        {
            ServiceLocator serviceLocator = MainContext.Instance.ServiceLocator;

            foreach (object service in _services)
            {
                if (serviceLocator.IsRegistered(service))
                {
                    serviceLocator.Unregister(service);
                    (service as IDisposable)?.Dispose();
                }
            }
        }
    }
}