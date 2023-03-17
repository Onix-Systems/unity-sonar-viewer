
using System;
using System.Collections.Generic;
using UnityEngine;
using App.Infrastructure.CommonInterfaces;

namespace App.Infrastructure.Contexts
{
    public abstract class MainContext: MonoBehaviour, IContext
    {
        public static MainContext Instance { get; private set; }

        internal ServiceLocator ServiceLocator { get; private set; } = new ServiceLocator();

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(this);
            }
        }

        protected void Update()
        {
            int servicesAmount = ServiceLocator.TickableServices.Length;
            for (int iService = 0; iService < servicesAmount; ++iService)
            {
                ITickable tickable = ServiceLocator.TickableServices[iService];
                tickable.Tick();
            }
        }

        protected void OnDestroy() => Dispose();
        
        public TService Get<TService>() where TService : class => ServiceLocator.Get<TService>();

        public bool TryGet<TService>(out TService service) where TService : class => ServiceLocator.TryGet(out service);

        protected abstract void SetupServices(out List<object> services);

        public void Initialize()
        {
            List<object> services;
            SetupServices(out services);

            if (services == null || services.Count == 0)
            {
                return;
            }

            foreach (object service in services)
            {
                ServiceLocator.Register(service.GetType(), service);
            }

            foreach (object service in services)
            {
                (service as IInitializable)?.Initialize();
            }
        }

        public void Dispose()
        {
            foreach (object service in ServiceLocator.RegisteredServices)
            {
                (service as CommonInterfaces.IDisposable)?.Dispose();
            }

            ServiceLocator.UnregisterAll();
        }

        public void RegisterContext(IContext context)
        {
            ServiceLocator.Register(context);
        }

        public void UnregisterContext(IContext context)
        {
            ServiceLocator.Unregister(context);
        }
    }
}