using System;
using System.Collections.Generic;

namespace EQuery.Core
{
    static class IoC
    {
        private readonly static Dictionary<Type, Func<object>> _loaders = new Dictionary<Type, Func<object>>();

        static IoC()
        {
            UseDefaults();
        }

        public static void Register<T>(T instance) where T : class, IResolvable
        {
            _loaders[typeof(T)] = () => instance;
        }

        public static void Register<T>(Func<T> loader) where T : class, IResolvable
        {
            _loaders[typeof(T)] = loader;
        }

        public static T Resolve<T>() where T : class
        {
            Func<object> loader;

            if (_loaders.TryGetValue(typeof(T), out loader))
            {
                return (T)loader();
            }

            throw new ArgumentException("Unable to resolve :" + typeof(T));
        }

        internal static void ClearAll()
        {
            _loaders.Clear();
        }

        internal static void UseDefaults()
        {
            ClearAll();
            Register<INamingConvention>(new DefaultConvention());
        }
    }
}
