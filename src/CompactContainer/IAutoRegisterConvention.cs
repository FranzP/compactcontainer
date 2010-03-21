using System;

namespace CompactContainer
{
	public interface IAutoRegisterConvention
	{
		bool AutoRegisterUnknownType(Type type, ICompactContainer container);
	}
}