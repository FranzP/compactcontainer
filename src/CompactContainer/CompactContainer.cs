using System;
using System.Collections.Generic;
using System.Linq;
using CompactContainer.Activators;
using CompactContainer.DependencyResolvers;

namespace CompactContainer
{
    public class CompactContainer : ICompactContainer
    {
        private int singletonsInstanciatedCount;

    	private readonly List<ComponentInfo> components = new List<ComponentInfo>();
		private readonly Dictionary<Type, IActivator> activators = new Dictionary<Type, IActivator>();

    	public IActivator DefaultActivator { get; set; }

    	public LifestyleType DefaultLifestyle { get; set; }

		public IDependencyResolver DependencyResolver { get; set; }

    	public CompactContainer()
        {
            DefaultActivator = new DefaultActivator(this);
    		DependencyResolver = new DefaultDependencyResolver(this);
            DefaultLifestyle = LifestyleType.Singleton;

            AddComponentInstance("container", typeof(ICompactContainer), this);
        }

        public IEnumerable<ComponentInfo> Components
        {
            get { return components; }
		}

		#region Registration API - will change

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

		#endregion

		public bool HasComponent(Type service)
        {
            var find = getComponentInfo(service);
            return (find != null);
        }

        public bool HasComponent(string key)
        {
            var find = getComponentInfo(key);
            return (find != null);
        }

        public object Resolve(Type service)
        {
            var ci = getComponentInfo(service);
            if (ci == null)
            {
                throw new CompactContainerException(service.Name + " not registered");
            }
            return resolveComponent(ci);
        }

        public object Resolve(string key)
        {
            var ci = getComponentInfo(key);
            return resolveComponent(ci);
        }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        public T Resolve<T>(string key)
        {
            return (T) Resolve(key);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
        	return components.GetAllImplementorsFor(serviceType).Select(ci => resolveComponent(ci)).ToArray();
        }

		public IEnumerable<T> GetServices<T>()
        {
			return components.GetAllImplementorsFor(typeof (T)).Select(ci => (T) resolveComponent(ci)).ToArray();
        }

        public void RegisterActivator(Type targetType, IActivator activator)
        {
            activators.Add(targetType, activator);
        }

        public void RegisterActivator<T>(IActivator activator)
        {
            RegisterActivator(typeof(T), activator);
        }

        public void Dispose()
        {
        	var disposables = components
        		.Select(component => component.Instance)
        		.OfType<IDisposable>()
        		.Where(disposable => disposable != this);
        	
			foreach (var disposable in disposables)
        	{
        		disposable.Dispose();
        	}
        }

    	private static string defaultKey(Type service)
        {
            return service.FullName;
        }

        private ComponentInfo getComponentInfo(Type service)
        {
            var result = components.FindServiceType(service) ?? components.FindClassType(service);

        	if (result == null)
            {
            	var autoRegisterConventions = GetServices<IAutoRegisterConvention>();
            	if (autoRegisterConventions.Any(c => c.AutoRegisterUnknownType(service, this)))
            	{
            		result = components.FindServiceType(service);
            	}
            }
        	return result;
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
                            ci.Instance = handleCreateNew(ci);
                            ci.IsResolvingDependencies = false;
                            singletonsInstanciatedCount++;
                        }
                        return ci.Instance;

                    case (LifestyleType.Transient):
                        ci.IsResolvingDependencies = true;
                        var result = handleCreateNew(ci);
                        ci.IsResolvingDependencies = false;
                        return result;
                }

                return null;
            }
        }

        private object handleCreateNew(ComponentInfo componentInfo)
        {
            var activator = DefaultActivator;

			foreach (var pair in activators.Where(pair => pair.Key.IsAssignableFrom(componentInfo.Classtype)))
            {
            	activator = pair.Value;
            	break;
            }
			return activator.Create(componentInfo);
        }
    }
}