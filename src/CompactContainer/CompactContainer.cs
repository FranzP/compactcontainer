using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CompactContainer.Activators;
using CompactContainer.DependencyResolvers;
using CompactContainer.Registrations;

namespace CompactContainer
{
    public class CompactContainer : ICompactContainer
    {
    	private readonly List<ComponentInfo> components = new List<ComponentInfo>();
		private readonly Hashtable configuration = new Hashtable();
		private readonly Dictionary<Type, IActivator> activators = new Dictionary<Type, IActivator>();
		private readonly IList<IComponentSelector> componentSelectors = new List<IComponentSelector>();
		private readonly IList<IDiscoveryConvention> discoveryConventions = new List<IDiscoveryConvention>();
		private readonly IList<IFacility> facilities = new List<IFacility>();

    	public IActivator DefaultActivator { get; set; }

    	public LifestyleType DefaultLifestyle { get; set; }

		public IDependencyResolver DependencyResolver { get; set; }

    	public CompactContainer()
        {
            DefaultActivator = new DefaultActivator(this);
    		DependencyResolver = new CompositeDependencyResolver(this);
            DefaultLifestyle = LifestyleType.Singleton;
        }

		public Hashtable Configuration
		{
			get { return configuration; }
		}

		public IEnumerable<ComponentInfo> Components
		{
			get { return components; }
		}

		public void Register(params IRegistration[] registrations)
		{
			foreach (var registration in registrations)
			{
				registration.Apply(this);
			}
		}

    	public void AddFacility<T>(T facility) where T : IFacility
    	{
			facility.Init(this);
			facilities.Add(facility);
    	}

    	public event Action<ComponentInfo> ComponentRegistered = delegate { };
    	public event Action<ComponentInfo, object> ComponentCreated = delegate { };

    	public void Install(params IComponentsInstaller[] installers)
		{
			foreach (var installer in installers)
			{
				installer.Install(this);
			}
		}

		public void AddComponentInfo(ComponentInfo componentInfo)
		{
			if (HasComponent(componentInfo.Key))
				throw new CompactContainerException("A component with the same key is already registered: \"" + componentInfo.Key + "\"");

			components.Add(componentInfo);

			ComponentRegistered(componentInfo);
		}

		public bool HasComponent(Type service)
        {
            var find = getComponentInfo(service);
            return (find != null);
        }

        public bool HasComponent(string key)
        {
			if (key == null)
				throw new ArgumentNullException("key");

            var find = getComponentInfo(key);
            return (find != null);
        }

        public object Resolve(Type service)
        {
            var ci = getComponentInfo(service);
            if (ci == null)
            {
                throw new CompactContainerException("Component with service type " + service.Name + " not registered in the container");
            }
            return resolveComponent(ci);
        }

        public object Resolve(string key)
        {
            var ci = getComponentInfo(key);
			if (ci == null)
			{
				throw new CompactContainerException("Component with key \"" + key + "\" not registered in the container");
			}
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

    	public void RegisterComponentSelector(IComponentSelector componentSelector)
    	{
    		componentSelectors.Add(componentSelector);
    	}

    	public void RegisterDiscoveryConvention(IDiscoveryConvention discoveryConvention)
    	{
    		discoveryConventions.Add(discoveryConvention);
    	}

    	public void Dispose()
    	{
    		terminateFacilities();
    		disposeDisposableComponents();
    	}

		private void terminateFacilities()
		{
			foreach (var facility in facilities)
			{
				facility.Terminate();
			}
		}

    	private void disposeDisposableComponents()
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

    	private ComponentInfo getComponentInfo(Type service)
        {
    		ComponentInfo result = null;

    		var availableComponents = components.GetAllImplementorsFor(service);

			if (availableComponents.Count() == 1)
			{
				result = availableComponents.Single();
			}
			else if (availableComponents.Count() > 1)
			{
				var componentSelector = componentSelectors.FirstOrDefault(cs => cs.HasOpinionAbout(service));
				result = componentSelector != null
				         	? componentSelector.SelectComponent(service, availableComponents)
				         	: availableComponents.First();

			}
			else
            {
				// no matching component registered... try IAutoRegisterConventions

            	if (discoveryConventions.Any(c => c.TryRegisterUnknownType(service, this)))
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
                if (ci.IsResolvingDependencies)
                {
                    throw new CompactContainerException("Circular reference: " + ci);
                }

                switch (ci.Lifestyle)
                {
                    case (LifestyleType.Singleton):
                        if (ci.Instance == null)
                        {
                            ci.Instance = handleCreateNew(ci);
                        }
                        return ci.Instance;

                    case (LifestyleType.Transient):
                		return handleCreateNew(ci);
                }

                return null;
            }
        }

		private object handleCreateNew(ComponentInfo componentInfo)
		{
			componentInfo.IsResolvingDependencies = true;

			var activator = DefaultActivator;

			foreach (var pair in activators.Where(pair => pair.Key.IsAssignableFrom(componentInfo.Classtype)))
			{
				activator = pair.Value;
				break;
			}

			var instance = activator.Create(componentInfo);

			componentInfo.IsResolvingDependencies = false;

			ComponentCreated(componentInfo, instance);

			return instance;
		}
    }
}