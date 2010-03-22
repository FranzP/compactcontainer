using System;
using System.Collections.Generic;

namespace CompactContainer
{
	public interface ICompactContainer : IDisposable
	{
		IActivator DefaultActivator { get; set; }
		LifestyleType DefaultLifestyle { get; set; }
		IDependencyResolver DependencyResolver { get; set; }

		IEnumerable<ComponentInfo> Components { get; }

		void AddComponentInfo(ComponentInfo componentInfo);
		#region Registration api - will change
		void AddComponent(string key, Type classType);
		void AddComponent(string key, Type classType, LifestyleType lifestyle);
		void AddComponent(string key, Type serviceType, Type classType);
		void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle);
		void RemoveComponent(string key);
		void AddComponentInstance(string key, object instance);
		void AddComponentInstance(string key, Type serviceType, object instance);
		#endregion

		bool HasComponent(Type service);
		bool HasComponent(string key);

		object Resolve(Type service);
		object Resolve(string key);
		T Resolve<T>();
		T Resolve<T>(string key);

		IEnumerable<object> GetServices(Type serviceType);
		IEnumerable<T> GetServices<T>();

		void RegisterActivator(Type targetType, IActivator activator);
		void RegisterActivator<T>(IActivator activator);
	}
}