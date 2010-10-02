using System;

namespace CompactContainer.DependencyResolvers
{
	public class SimpleDependencyResolver : IDependencyResolver
	{
		private readonly ICompactContainer container;

		public SimpleDependencyResolver(ICompactContainer container)
		{
			this.container = container;
		}

		public bool CanResolve(string key, Type type, ComponentInfo componentContext)
		{
			return container.HasComponent(type);
		}

		public object Resolve(string key, Type type, ComponentInfo componentContext)
		{
			return container.Resolve(type);
		}
	}
}