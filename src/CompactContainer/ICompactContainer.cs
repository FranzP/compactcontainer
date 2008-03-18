using System;

namespace InversionOfControl
{
	public interface ICompactContainer : IDisposable
	{
		bool HasComponent(Type service);
		bool HasComponent(string key);
		object Resolve(Type service);
		object Resolve(string key);
		T Resolve<T>();
		T Resolve<T>(string key);
		void AddComponent(string key, Type classType);
		void AddComponent(string key, Type classType, LifestyleType lifestyle);
		void AddComponent(string key, Type serviceType, Type classType);
		void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle);
		void AddComponentInstance(string key, object instance);
		void AddComponentInstance(string key, Type serviceType, object instance);
		object[] GetServices(Type serviceType);
		T[] GetServices<T>();
		ComponentList Components { get; }
		int SingletonsInstanciatedCount { get; }
	}
}