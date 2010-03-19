using System;

namespace CompactContainer
{
	public interface IConstructorResolver
	{
		ConstructionInfo GetConstructionInfo(Type classType);
	}
}