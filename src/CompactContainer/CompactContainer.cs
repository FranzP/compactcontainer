using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InversionOfControl
{
	public class CompactContainer : ICompactContainer
	{
		private int singletonsInstanciatedCount;
		private readonly ComponentList components = new ComponentList();

		public CompactContainer()
		{
			AddComponentInstance("condor.container", typeof(ICompactContainer), this);
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
			if (HasComponent(key)) {
				throw new CompactContainerException("key ya registrado");
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
				throw new CompactContainerException("key ya registrado");
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
			if (ci == null) {
				throw new CompactContainerException(service.Name + " no registrado");
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
			return (T) Resolve(typeof (T));
		}

		public T Resolve<T>(string key)
		{
			return (T) Resolve(key);
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
			List<ComponentInfo> implementors = components.GetAllImplementorsFor(typeof (T));

			T[] services = new T[implementors.Count];

			int i = 0;
			implementors.ForEach(delegate(ComponentInfo ci)
			                     	{
			                     		services[i++] = (T) resolveComponent(ci);
			                     	});

			return services;
		}

		public void Dispose()
		{
		}

		private ComponentInfo getComponentInfo(Type service)
		{
			ComponentInfo result;
			result = components.FindServiceType(service);
			if (result == null) {
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
			if (!HasComponent(ci.ServiceType)) {
				throw new CompactContainerException("Componente no registrado " + ci.ServiceType.Name);
			}
			if (ci.IsResolvingDependencies) {
				throw new CompactContainerException("Referencia circular: " + ci.ServiceType.Name);
			}

			switch(ci.Lifestyle) {
				case (LifestyleType.Singleton):
					if (ci.Instance == null) {
						ci.IsResolvingDependencies = true;
						ci.Instance = createNew(ci.Classtype);
						ci.IsResolvingDependencies = false;
						singletonsInstanciatedCount++;
					}
					return ci.Instance;

				case (LifestyleType.Transient):
					ci.IsResolvingDependencies = true;
					object result = createNew(ci.Classtype);
					ci.IsResolvingDependencies = false;
					return result;
			}

			return null;
		}

		private object createNew(Type classType)
		{
			ConstructorInfo[] constructors = classType.GetConstructors();
			if (constructors.Length < 1) {
				throw new CompactContainerException("El tipo " + classType.Name + " debe tener por lo menos un ctor publico");
			}

			ConstructorInfo theConstructor = null;
			object[] theConstructorParameters = new object[0];

			StringBuilder missingComponents = new StringBuilder();

			foreach (ConstructorInfo constructorInfo in constructors) {

				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length > theConstructorParameters.Length
				    || theConstructor == null) {
					
					
					bool proposeNewConstructor = true;
					object[] parameterObjects = new object[parameters.Length];

					for (int i = 0; i < parameters.Length; i++) {
						if (HasComponent(parameters[i].ParameterType)) {
							parameterObjects[i] = Resolve(parameters[i].ParameterType);
						}
						else {
							missingComponents.Append(parameters[i].ParameterType.Name + "; ");
							proposeNewConstructor = false;
							break;
						}
					}

					if (proposeNewConstructor) {
						theConstructor = constructorInfo;
						theConstructorParameters = parameterObjects;
					}
				}
			}

			if (theConstructor == null) {
				throw new CompactContainerException("Missing components: " + missingComponents);
			}

			return theConstructor.Invoke(theConstructorParameters);
		}
	}
}