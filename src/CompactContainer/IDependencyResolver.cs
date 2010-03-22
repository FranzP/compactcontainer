using System;

namespace CompactContainer
{
	public interface IDependencyResolver
	{
		/// <summary>
		/// Determines whether this implementation can provide a value for a fullfiling a dependency
		/// based on the provided context information
		/// </summary>
		/// <param name="key">string key to lookup for dependencies (each implementor can used this key string as needed)</param>
		/// <param name="type">type of the expected dependency</param>
		/// <param name="componentContext">component to which the dependency will be applied</param>
		/// <returns>true if this this resolver can provide a value for the required info, false otherwise</returns>
		bool CanResolve(string key, Type type, ComponentInfo componentContext);

		object Resolve(string key, Type type, ComponentInfo componentContext);
	}
}