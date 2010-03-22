using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactContainer
{
	public static class EnumerableComponentInfoExtensions
	{
		public static IEnumerable<ComponentInfo> GetAllImplementorsFor(this IEnumerable<ComponentInfo> list, Type serviceType)
		{
			return list.Where(ci => ci.ServiceTypes.Any(t => t.Equals(serviceType))).ToList();
		}

		public static ComponentInfo FindServiceType(this IEnumerable<ComponentInfo> list, Type serviceType)
		{
			return list.FirstOrDefault(ci => ci.ServiceTypes.Any(t => t.Equals(serviceType)));
		}

		public static ComponentInfo FindKey(this IEnumerable<ComponentInfo> list, string key)
		{
			return list.FirstOrDefault(ci => ci.Key.Equals(key));
		}
	}
}