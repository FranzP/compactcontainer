using System;

namespace CompactContainer
{
	public interface IDependencyResolver
	{
		bool CanResolve(string key, Type type, ComponentInfo componentContext);
		object Resolve(string key, Type type, ComponentInfo componentContext);
	}
}