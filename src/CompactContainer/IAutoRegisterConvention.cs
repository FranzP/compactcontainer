using System;

namespace CompactContainer
{
	public interface IAutoRegisterConvention
	{
		bool TryRegisterUnknownType(Type type, ICompactContainer container);
	}
}