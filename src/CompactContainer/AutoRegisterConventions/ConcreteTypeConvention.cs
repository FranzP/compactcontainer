using System;

namespace CompactContainer.AutoRegisterConventions
{
	public class ConcreteTypeConvention : IAutoRegisterConvention
	{
		public bool AutoRegisterUnknownType(Type type, ICompactContainer container)
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