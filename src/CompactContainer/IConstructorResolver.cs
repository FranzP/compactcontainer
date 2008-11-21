using System;

namespace InversionOfControl
{
	public interface IConstructorResolver
	{
		ConstructionInfo GetConstructionInfo(Type classType);
	}
}