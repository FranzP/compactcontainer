using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactContainer
{
	public class ComponentInfo
	{
		private readonly IDictionary<string, object> parameters = new Dictionary<string, object>();

		public ComponentInfo(string key, Type serviceType, Type classType, LifestyleType lifestyle)
			: this(key, new[] {serviceType}, classType, lifestyle)
		{
		}

		public ComponentInfo(string key, IEnumerable<Type> serviceType, Type classType, LifestyleType lifestyle)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			Key = key;
			ServiceTypes = serviceType;
			Classtype = classType;
			Lifestyle = lifestyle;
		}

		public string Key { get; private set; }

		public IEnumerable<Type> ServiceTypes { get; private set; }

		public Type Classtype { get; private set; }

		public LifestyleType Lifestyle { get; private set; }

		public IDictionary<string, object> Parameters
		{
			get { return parameters; }
		}

		public object Instance { get; set; }

		internal bool IsResolvingDependencies { get; set; }

		public override string ToString()
		{
			return string.Format("Key:{0} - Services:{1} - Class:{2}",
			                     Key,
			                     ServiceTypes.Select(t => t.Name).ToCommandSeparatedString(),
			                     Classtype.Name);
		}
	}
}