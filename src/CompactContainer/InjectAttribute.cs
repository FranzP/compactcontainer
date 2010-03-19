using System;

namespace CompactContainer
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public class InjectAttribute : Attribute
	{
	}
}