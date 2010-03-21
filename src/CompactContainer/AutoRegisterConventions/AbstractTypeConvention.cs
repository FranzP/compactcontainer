using System;

namespace CompactContainer.AutoRegisterConventions
{
	public class AbstractTypeConvention : IAutoRegisterConvention
	{
		public bool AutoRegisterUnknownType(Type type, ICompactContainer container)
		{
			if (type.IsInterface)
			{
				if (type.Name.StartsWith("I"))
				{
					var concreteName = type.Name.Remove(0, 1);
					var concreteFullName = type.FullName.Replace(type.Name, concreteName);
					var concreteType = type.Assembly.GetType(concreteFullName);
					container.AddComponent(type.FullName, type, concreteType);
					return true;
				}
			}

			return false;
		}
	}
}