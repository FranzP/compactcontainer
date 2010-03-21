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
			var ctor = constructorResolver.SelectConstructor(componentInfo);

			if (ctor == null)
				throw new CompactContainerException("No suitable constructor for type " + componentInfo.Classtype);

			var parameters = ctor.GetParameters();
			var parameterValues = parameters
				.Select(parameter => container.DependencyResolver.Resolve(null, parameter.ParameterType, componentInfo))
				.ToArray();

			var result = ctor.Invoke(parameterValues);

			// here we can do property injection

			return result;
		}
	}
}