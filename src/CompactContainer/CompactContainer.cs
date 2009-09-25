using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    public class CompactContainer : ICompactContainer
    {
        private int singletonsInstanciatedCount;
        private IHandler defaultHandler;
        private LifestyleType defaultLifestyle;
        private bool _autoRegister;
        private readonly ComponentList components = new ComponentList();
        private readonly Dictionary<Type, IHandler> handlers = new Dictionary<Type, IHandler>();

        public IHandler DefaultHandler
        {
            get { return defaultHandler; }
            set { defaultHandler = value; }
        }

        public LifestyleType DefaultLifestyle
        {
            get { return defaultLifestyle; }
            set { defaultLifestyle = value; }
        }

        public bool ShouldAutoRegister
        {
            get { return _autoRegister; }
            set { _autoRegister = value; }
        }

        public CompactContainer()
        {
            defaultHandler = new DefaultHandler(this);
            defaultLifestyle = LifestyleType.Singleton;
            _autoRegister = true;
            AddComponentInstance("container", typeof(ICompactContainer), this);
        }

        public ComponentList Components
        {
            get { return components; }
        }

        public int SingletonsInstanciatedCount
        {
            get { return singletonsInstanciatedCount; }
        }

        public void AddComponent(Type classType)
        {
            AddComponent(defaultKey(classType), classType, classType, DefaultLifestyle);
        }

        public void AddComponent(Type classType, LifestyleType lifestyle)
        {
            AddComponent(defaultKey(classType), classType, classType, lifestyle);
        }

        public void AddComponent(Type serviceType, Type classType)
        {
            AddComponent(defaultKey(classType), serviceType, classType, DefaultLifestyle);
        }

        public void AddComponent(Type serviceType, Type classType, LifestyleType lifestyle)
        {
            AddComponent(defaultKey(classType), serviceType, classType, lifestyle);
        }

        public void AddComponent(string key, Type classType)
        {
            AddComponent(key, classType, classType, DefaultLifestyle);
        }

        public void AddComponent(string key, Type classType, LifestyleType lifestyle)
        {
            AddComponent(key, classType, classType, lifestyle);
        }

        public void AddComponent(string key, Type serviceType, Type classType)
        {
            AddComponent(key, serviceType, classType, DefaultLifestyle);
        }

        public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle)
        {
            if (HasComponent(key))
            {
                throw new CompactContainerException("key already registered: " + key);
            }
            ComponentInfo ci = new ComponentInfo(key, serviceType, classType, lifestyle);
            components.Add(ci);
        }

        public void RemoveComponent(string key)
        {
            ComponentInfo ci = getComponentInfo(key);
            if (ci == null)
                throw new CompactContainerException("component not registered: " + key);

            components.Remove(ci);
        }

        public void AddComponentInstance(string key, object instance)
        {
            AddComponentInstance(key, instance.GetType(), instance);
        }

        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            if (HasComponent(key))
            {
                throw new CompactContainerException("key already registered");
            }
            ComponentInfo ci = new ComponentInfo(key, serviceType, instance.GetType(), LifestyleType.Singleton);
            ci.Instance = instance;
            components.Add(ci);
            singletonsInstanciatedCount++;
        }

        public bool HasComponent(Type service)
        {
            ComponentInfo find = getComponentInfo(service);
            return (find != null);
        }

        public bool HasComponent(string key)
        {
            ComponentInfo find = getComponentInfo(key);
            return (find != null);
        }

        public object Resolve(Type service)
        {
            ComponentInfo ci = getComponentInfo(service);
            if (ci == null)
            {
                throw new CompactContainerException(service.Name + " not registered");
            }
            return resolveComponent(ci);
        }

        public object Resolve(string key)
        {
            ComponentInfo ci = getComponentInfo(key);
            return resolveComponent(ci);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public T Resolve<T>(string key)
        {
            return (T)Resolve(key);
        }

        public object[] GetServices(Type serviceType)
        {
            List<ComponentInfo> implementors = components.GetAllImplementorsFor(serviceType);

            object[] services = new object[implementors.Count];

            int i = 0;
            implementors.ForEach(delegate(ComponentInfo ci)
            {
                services[i++] = resolveComponent(ci);
            });

            return services;
        }

        public T[] GetServices<T>()
        {
            List<ComponentInfo> implementors = components.GetAllImplementorsFor(typeof(T));

            T[] services = new T[implementors.Count];

            int i = 0;
            implementors.ForEach(delegate(ComponentInfo ci)
            {
                services[i++] = (T)resolveComponent(ci);
            });

            return services;
        }

        public void RegisterHandler(Type targetType, IHandler handler)
        {
            handler.Container = this;
            handlers.Add(targetType, handler);
        }

        public void RegisterHandler<T>(IHandler handler)
        {
            RegisterHandler(typeof(T), handler);
        }

        public object TryResolve(Type service)
        {
            try
            {
                return Resolve(service);
            }
            catch
            {
                return null;
            }
        }

        public T TryResolve<T>()
        {
            return (T)TryResolve(typeof(T));
        }

        public void Dispose()
        {
            foreach(ComponentInfo item in components)
            {
                var disposable = item.Instance as IDisposable;
                if (disposable == null) continue;
                if (disposable == this) continue;
                disposable.Dispose();
            }
        }

        private string defaultKey(Type service)
        {
            return service.FullName;
        }

        private ComponentInfo getComponentInfo(Type service)
        {
            // TODO: consider implementing as Chain of Responsibility pattern

            var result = components.FindServiceType(service);

            if (result == null)
            {
                result = components.FindClassType(service);
            }
            if (result == null)
            {
                result = TryAutoResolveConcreteType(service);
            }
            if (result == null)
            {
                result = TryAutoResolveAbstractType(service);
            }
            return result;
        }

        private ComponentInfo TryAutoResolveConcreteType(Type service)
        {
            if (!(service.IsInterface || service.IsAbstract))
            {
                AddComponent(defaultKey(service), service);
                return components.FindServiceType(service);
            }
            return null;
        }

        private ComponentInfo TryAutoResolveAbstractType(Type service)
        {
            if (_autoRegister && (service.IsInterface || service.IsAbstract))
            {
                if (service.Name.IndexOf('I') == 0)
                {
                    var concreteName = service.Name.Remove(0, 1);
                    var concreteServiceName = service.FullName.Replace(service.Name, concreteName);
                    var concreteServiceType = service.Assembly.GetType(concreteServiceName);
                    AddComponent(defaultKey(service), service, concreteServiceType);
                    return components.FindServiceType(service);
                }
            }
            return null;
        }

        private ComponentInfo getComponentInfo(string key)
        {
            return components.FindKey(key);
        }

        private object resolveComponent(ComponentInfo ci)
        {
            lock (ci)
            {
                if (!HasComponent(ci.ServiceType))
                {
                    throw new CompactContainerException("Component not registered " + ci.ServiceType.Name);
                }
                if (ci.IsResolvingDependencies)
                {
                    throw new CompactContainerException("Circular reference: " + ci.ServiceType.Name);
                }

                switch (ci.Lifestyle)
                {
                    case (LifestyleType.Singleton):
                        if (ci.Instance == null)
                        {
                            ci.IsResolvingDependencies = true;
                            ci.Instance = handleCreateNew(ci.Classtype);
                            ci.IsResolvingDependencies = false;
                            singletonsInstanciatedCount++;
                        }
                        return ci.Instance;

                    case (LifestyleType.Transient):
                        ci.IsResolvingDependencies = true;
                        object result = handleCreateNew(ci.Classtype);
                        ci.IsResolvingDependencies = false;
                        return result;
                }

                return null;
            }
        }

        private object handleCreateNew(Type classType)
        {
            IHandler handler = defaultHandler;

            foreach (KeyValuePair<Type, IHandler> pair in handlers)
            {
                if (pair.Key.IsAssignableFrom(classType))
                {
                    handler = pair.Value;
                    break;
                }
            }
            return handler.Create(classType);
        }
    }
}