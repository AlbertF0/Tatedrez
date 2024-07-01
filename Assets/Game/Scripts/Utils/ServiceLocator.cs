using System;
using System.Collections.Generic;

namespace Utils
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, IDisposable> _services = new Dictionary<Type, IDisposable>();

        public static void RegisterService<T>(T service, bool overwrite = false) where T : IDisposable
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type) && overwrite)
            {
                UnregisterService<T>();
            }

            _services.Add(type, service);
        }

        public static void RegisterService<T>() where T : IDisposable, new()
        {
            RegisterService(new T());
        }

        public static bool HasService<T>() where T : IDisposable
        {
            return _services.ContainsKey(typeof(T));
        }

        public static T GetService<T>() where T : IDisposable
        {
            IDisposable service;
            if (!_services.TryGetValue(typeof(T), out service))
            {
                return default;
            }

            return (T)service;
        }

        public static void UnregisterService<T>()
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services[type].Dispose();
                _services.Remove(type);
            }
        }

        public static void UnregisterAll()
        {
            foreach (KeyValuePair<Type, IDisposable> service in _services)
            {
                service.Value.Dispose();
            }

            _services.Clear();
        }
    }
}