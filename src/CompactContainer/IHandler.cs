using System;

namespace InversionOfControl
{
	public interface IHandler
	{
		ICompactContainer Container { get; set; }
		object Create(Type classType);
	}
}