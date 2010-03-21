using System;

namespace CompactContainer.AutoRegisterConventions
{
	public class ConcreteTypeConvention : IAutoRegisterConvention
	{
		public bool AutoRegisterUnknownType(Type type, ICompactContainer container)
		{
			if (!(type.IsInterface || type.IsAbstract))
			{
				container.AddComponent(type.FullName, type);
				return true;
			}
			return false;
		}
	}
}