using System;
using System.Linq;

namespace CompactContainer
{
	public static class SelectService
	{
		public static Type FirstInterface(Type type)
		{
			return type.GetInterfaces().First();
		}
	}
}