using System;

namespace CompactContainer
{
	public interface IDiscoveryConvention
	{
		bool TryRegisterUnknownType(Type type, ICompactContainer container);
	}
}