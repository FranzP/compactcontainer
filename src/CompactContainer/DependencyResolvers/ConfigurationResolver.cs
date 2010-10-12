using System;

namespace CompactContainer.DependencyResolvers
{
	/// <summary>
	/// Contributes to dependency resolution chain by looking for a configuration
	/// that matches the parameter being resolved.
	/// It uses the convention: ComponentKey.DependencyKey
	/// </summary>
	public class ConfigurationResolver : IDependencyResolver
	{
		private readonly ICompactContainer container;

		public ConfigurationResolver(ICompactContainer container)
		{
			this.container = container;
		}

		public bool CanResolve(string key, Type type, ComponentInfo componentContext)
		{
			var configKey = GetKey(componentContext, key);
			return container.Configuration.Contains(configKey);
		}

		public object Resolve(string key, Type type, ComponentInfo componentContext)
		{
			var configKey = GetKey(componentContext, key);
			var configValue = container.Configuration[configKey];

			if (type.IsAssignableFrom(configValue.GetType()) == false)
			{
				throw new CompactContainerException("Cannot convert configuration \"" + configKey + "\" to type " + type.FullName +
				                                    " for component " + componentContext);
			}

			return configValue;
		}

		private static string GetKey(ComponentInfo componentInfo, string key)
		{
			return componentInfo.Key + "." + key;
		}
	}
}