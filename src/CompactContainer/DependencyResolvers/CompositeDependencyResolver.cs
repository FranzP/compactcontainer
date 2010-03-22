using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactContainer.DependencyResolvers
{
	public class CompositeDependencyResolver : IDependencyResolver
	{
		private readonly List<IDependencyResolver> resolvers = new List<IDependencyResolver>();

		public CompositeDependencyResolver(ICompactContainer container)
		{
			resolvers.Add(new DefaultDependencyResolver(container));
			resolvers.Add(new ArrayDependencyResolver(container));
			resolvers.Add(new ParameterDependencyResolver());
		}

		public bool CanResolve(string key, Type type, ComponentInfo componentContext)
		{
			return resolvers.Any(r => r.CanResolve(key, type, componentContext));
		}

		public object Resolve(string key, Type type, ComponentInfo componentContext)
		{
			var resolver = resolvers.FirstOrDefault(r => r.CanResolve(key, type, componentContext));
			return resolver != null ? resolver.Resolve(key, type, componentContext) : null;
		}
	}
}