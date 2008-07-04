using System;
using System.Collections.Generic;

namespace InversionOfControl
{
	public class CompactContainer : ICompactContainer
	{
		private int singletonsInstanciatedCount;
		private readonly DefaultHandler defaultHandler;
		private readonly ComponentList components = new ComponentList();
		private readonly Dictionary<Type, IHandler> handlers = new Dictionary<Type, IHandler>();

		public DefaultHandler DefaultHandler
		{
			get { return defaultHandler; }
		}

		public CompactContainer()
		{
			defaultHandler = new DefaultHandler(this);
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

		public void AddComponent(string key, Type classType)
		{
			AddComponent(key, classType, classType, LifestyleType.Singleton);
		}

		public void AddComponent(string key, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, classType, classType, lifestyle);
		}

		public void AddComponent(string key, Type serviceType, Type classType)
		{
			AddComponent(key, serviceType, classType, LifestyleType.Singleton);
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
		}

		private ComponentInfo getComponentInfo(Type service)
		{
			ComponentInfo result;
			result = components.FindServiceType(service);
			if (result == null)
			{
				result = components.FindClassType(service);
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