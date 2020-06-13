using System;
using System.Collections.Generic;
using System.Linq;

namespace RArcher.UAP.Toolkit.Common
{
    public class SimpleIoc
    {
        public enum ObjectLifetime { Transient, Singleton }

        private readonly Dictionary<string, MappedTypeInfo> _mappedTypes;  // Dictionary of registered types. Interface names are keys
        private TypeMapping _currentTypeMapping;

        public SimpleIoc()
        {
            _mappedTypes = new Dictionary<string, MappedTypeInfo>();
        }

        public SimpleIoc RegisterType<T>() where T : class
        {
            _currentTypeMapping = new TypeMapping {Key = typeof(T).FullName};
            return this;  // Allows for fluent (chained) calls
        }

        public SimpleIoc To<T>() where T : class
        {
            if(_currentTypeMapping == null)
            {
                ThrowGeneralException();
                return null;
            }

            _currentTypeMapping.MappedTypeInfo = new MappedTypeInfo {ConcreteType = typeof(T)};
            return this;  // Allows for fluent (chained) calls
        }

        public void InTransientScope()
        {
            // A new instance is activated for every request
            AddMapping(ObjectLifetime.Transient);
        }

        public void InSingletonScope()
        {
            // A single instance of the mapped type is returned for all requests
            AddMapping(ObjectLifetime.Singleton);
        }

        public T Get<T>() where T : class
        {
            try
            {
                var typeMapping = _mappedTypes.First(t => t.Key.Equals(typeof(T).FullName));
                var mappedTypeInfo = (MappedTypeInfo)typeMapping.Value;

                if(mappedTypeInfo.Lifetime == ObjectLifetime.Transient)
                    return (T)Activator.CreateInstance(mappedTypeInfo.ConcreteType);

                if(mappedTypeInfo.ConcreteTypeInstance == null)
                    mappedTypeInfo.ConcreteTypeInstance = Activator.CreateInstance(mappedTypeInfo.ConcreteType);

                return (T)mappedTypeInfo.ConcreteTypeInstance;
            }
            catch
            {
                throw new TypeNotRegisteredException(typeof(T).FullName);
            }
        }

        public bool UnRegisterType<T>() where T : class
        {
            return _mappedTypes.Remove(typeof(T).FullName);  // Returns true if type unregistered
        }

        private void AddMapping(ObjectLifetime lifetime)
        {
            if(_currentTypeMapping == null || _currentTypeMapping.MappedTypeInfo == null)
            {
                ThrowGeneralException();
                return;
            }

            _currentTypeMapping.MappedTypeInfo.Lifetime = lifetime;

            try
            {
                _mappedTypes.Add(_currentTypeMapping.Key, _currentTypeMapping.MappedTypeInfo);
            }
            catch(ArgumentException)
            {
                throw new TypeAlreadyRegisteredException(_currentTypeMapping.Key);
            }
            finally
            {
                _currentTypeMapping = null;
            }
        }

        private void ThrowGeneralException()
        {
            throw new Exception("Example usage: IocContainer.Container.RegisterType<IMyType>().To<MyType>().InSingletonScope();");
        }

        protected internal class MappedTypeInfo
        {
            public Type ConcreteType { get; set; }
            public object ConcreteTypeInstance { get; set; }
            public ObjectLifetime Lifetime { get; set; }
        }

        protected internal class TypeMapping
        {
            public MappedTypeInfo MappedTypeInfo { get; set; }
            public string Key { get; set; }
        }

        public class TypeAlreadyRegisteredException : Exception
        {
            public TypeAlreadyRegisteredException(string typeName) : base($"{typeName} has already been registered") {}
        }

        public class TypeNotRegisteredException : Exception
        {
            public TypeNotRegisteredException(string typeName) : base($"{typeName} has not been registered") { }
        }
    }
}