using System;

namespace CompactContainer
{
	public interface IHandler
	{
		ICompactContainer Container { get; set; }
		object Create(Type classType);
	}
}