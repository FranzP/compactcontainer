using System;

namespace CompactContainer.DependencyResolvers
{
	public class ParameterDependencyResolver : IDependencyResolver
	{

		public bool CanResolve(string key, Type type, ComponentInfo componentContext)
		{
			if (string.IsNullOrEmpty(key))
				return false;

			return componentContext.Parameters.ContainsKey(key);
		}

		public object Resolve(string key, Type type, ComponentInfo componentContext)
		{
			var dependency = componentContext.Parameters[key];
			
			if (type.IsAssignableFrom(dependency.GetType()) == false)
			{
				throw new CompactContainerException("Cannot convert parameter override \"" + key + "\" to type " + type.FullName +
				                                    " for component " + componentContext);
			}

			return dependency;
		}
	}
}