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
			return string.IsNullOrEmpty(key) ? container.HasComponent(type) : container.HasComponent(key);
		}

		public object Resolve(string key, Type type, ComponentInfo componentContext)
		{
			return string.IsNullOrEmpty(key) ? container.Resolve(type) : container.Resolve(key);
		}
	}
}