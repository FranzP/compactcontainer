using System;
using System.Collections;
using System.Collections.Generic;
using CompactContainer.Registrations;

namespace CompactContainer
{
	public interface ICompactContainer : IDisposable
	{
		IActivator DefaultActivator { get; set; }
		LifestyleType DefaultLifestyle { get; set; }
		IDependencyResolver DependencyResolver { get; set; }

		Hashtable Configuration { get; }
		IEnumerable<ComponentInfo> Components { get; }

		void Install(params IComponentsInstaller[] installers);

		void AddComponentInfo(ComponentInfo componentInfo);

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
		void RegisterComponentSelector(IComponentSelector componentSelector);
		void RegisterDiscoveryConvention(IDiscoveryConvention discoveryConvention);
		void Register(params IRegistration[] registrations);

		void AddFacility<T>(T facility) where T : IFacility;

		event Action<ComponentInfo> ComponentRegistered;
		event Action<ComponentInfo, object> ComponentCreated;
	}
}