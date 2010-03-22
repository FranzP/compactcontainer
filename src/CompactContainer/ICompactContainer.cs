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
		void RemoveComponent(string key);

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