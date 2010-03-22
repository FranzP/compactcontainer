﻿using System;

namespace CompactContainer.DependencyResolvers
{
	public class DefaultDependencyResolver : IDependencyResolver
	{
		private readonly ICompactContainer container;

		public DefaultDependencyResolver(ICompactContainer container)
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