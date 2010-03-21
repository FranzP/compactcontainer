using System.Reflection;
using System.Text;

namespace CompactContainer.ConstructorResolvers
{
	public class DefaultConstructorResolver : IConstructorResolver
	{
		private readonly ICompactContainer container;

		public DefaultConstructorResolver(ICompactContainer container)
		{
			this.container = container;
		}

		public virtual ConstructorInfo SelectConstructor(ComponentInfo componentInfo)
		{
			ConstructorInfo selectedCtor = null;
			var missingComponents = new StringBuilder();

			foreach (var constructorInfo in componentInfo.Classtype.GetConstructors())
			{
				var parameters = constructorInfo.GetParameters();
				if (selectedCtor != null && parameters.Length <= selectedCtor.GetParameters().Length) 
					continue;

				var proposeNewConstructor = true;

				foreach (var parameterInfo in parameters)
				{
					if (!container.DependencyResolver.CanResolve(null, parameterInfo.ParameterType, componentInfo))
					{
						missingComponents.Append(parameterInfo.ParameterType.Name + "; ");
						proposeNewConstructor = false;
						break;
					}
				}

				if (proposeNewConstructor)
				{
					selectedCtor = constructorInfo;
				}
			}

			if (selectedCtor == null)
			{
				throw new CompactContainerException("Cannot infer constructor to instantiate " + componentInfo.Classtype.Name +
				                                    " - missing components: " + missingComponents);
			}

			return selectedCtor;
		}
	}
}