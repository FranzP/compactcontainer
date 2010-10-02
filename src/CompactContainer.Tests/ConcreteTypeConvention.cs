using System;

namespace CompactContainer.Tests
{
	public class ConcreteTypeConvention : IDiscoveryConvention
	{
		public bool TryRegisterUnknownType(Type type, ICompactContainer container)
		{
			if (!(type.IsInterface || type.IsAbstract))
			{
				container.AddComponentInfo(new ComponentInfo(type.FullName, type, type, container.DefaultLifestyle));
				return true;
			}
			return false;
		}
	}
}