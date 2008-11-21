using System;

namespace InversionOfControl
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public class InjectAttribute : Attribute
	{
	}
}