using System.Linq;
using CompactContainer.ConstructorResolvers;

namespace CompactContainer.Activators
{
	public class DefaultActivator : IActivator
	{
		private readonly ICompactContainer container;
		private readonly IConstructorResolver constructorResolver;

		public DefaultActivator(ICompactContainer container) 
			: this(container, new DefaultConstructorResolver(container))
		{
		}	
		
		public DefaultActivator(ICompactContainer container, IConstructorResolver constructorResolver)
		{
			this.container = container;
			this.constructorResolver = constructorResolver;
		}

		public object Create(ComponentInfo componentInfo)
		{
			// ctor injection - required dependencies

			var ctor = constructorResolver.SelectConstructor(componentInfo);

			if (ctor == null)
				throw new CompactContainerException("No suitable constructor for type " + componentInfo.Classtype);

			var parameters = ctor.GetParameters();
			var parameterValues = parameters
				.Select(parameter => container.DependencyResolver.Resolve(parameter.Name, parameter.ParameterType, componentInfo))
				.ToArray();

			var result = ctor.Invoke(parameterValues);

			// property injection - optional dependencies

			var properties = result.GetType().GetProperties().Where(p => p.CanWrite && p.GetSetMethod() != null);
			foreach (var prop in properties)
			{
				if (container.DependencyResolver.CanResolve(prop.Name, prop.PropertyType, componentInfo))
				{
					prop.SetValue(result, container.DependencyResolver.Resolve(prop.Name, prop.PropertyType, componentInfo), null);
				}
			}
			return result;
		}
	}
}