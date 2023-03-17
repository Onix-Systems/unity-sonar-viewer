
using System;
using System.Collections.Generic;
using System.Linq;
using App.Infrastructure.CommonInterfaces;

namespace App.Infrastructure
{
    public class ServiceLocator
    {
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public object[] RegisteredServices { get; private set; } = { };
        public ITickable[] TickableServices { get; private set; } = { };

        public void Register<T>(T service) where T : class
        {
            Type type = service.GetType();
            
            if (_services.ContainsKey(type))
            {
                throw new Exception($"Trying to register type \"{type.Name}\" wich already registered.");
            }

            _services.Add(type, service); 
            UpdateRegisteredArray();
            UpdateTickablesArray();
        }

        public void Register(Type type, object service)
        {
            if (_services.ContainsKey(type))
            {
                throw new Exception($"Trying to register type \"{type.Name}\" wich already registered.");
            }

            _services.Add(type, service);
            UpdateRegisteredArray();
            UpdateTickablesArray();
        }

        public bool IsRegistered(object service)
        {
            Type type = service.GetType();
            return _services.ContainsKey(type);
        }

        public void Unregister<T>(T service) where T : class
        {
            Type type = service.GetType();
            
            if (!_services.ContainsKey(type))
            {
                return;
            }

            _services.Remove(type); 
            UpdateRegisteredArray();
            UpdateTickablesArray();
        }

        public void Unregister(Type type)
        {
            if (!_services.ContainsKey(type))
            {
                return;
            }

            _services.Remove(type);
            UpdateRegisteredArray();
            UpdateTickablesArray();
        }

        public void Unregister<T>() where T : class
        {
            Type type = typeof(T);
            
            if (_services.TryGetValue(type, out object service))
            {
                _services.Remove(type);
                UpdateRegisteredArray();
                UpdateTickablesArray();
            }
        }

        public void UnregisterAll()
        {
            _services.Clear();
            RegisteredServices = new object[] { };
            TickableServices = new ITickable[] { };
        }

        public T Get<T>() where T: class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object foundService))
            {
                return foundService as T;
            }
            else
            {
                throw new Exception($"Trying to get unregistered type \"{type.Name}\".");
            }
        }

        public bool TryGet<T>(out T service) where T: class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object foundService))
            {
                service = foundService as T;
                return true;
            }

            service = default;
            return false;
        }

        private void UpdateRegisteredArray()
        {
            RegisteredServices = _services.Values.ToArray();
        }

        private void UpdateTickablesArray()
        {
            TickableServices = RegisteredServices.Where(x => (x as ITickable) != null).Cast<ITickable>().ToArray();
        }
    }
}
