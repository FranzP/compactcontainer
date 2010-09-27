using System.Reflection;
using CompactContainer.Registrations;

namespace CompactContainer
{
	public static class AllTypes
	{
		public static AllTypesRegistration FromAssembly(Assembly assembly)
		{
			return new AllTypesRegistration(assembly);
		}
	}
}