using System;
using System.Collections.Generic;

namespace CompactContainer
{
	public class ComponentInfo
	{
		private readonly IDictionary<string, object> parameters = new Dictionary<string, object>();

		public ComponentInfo(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			Key = key;
			ServiceType = serviceType;
			Classtype = classType;
			Lifestyle = lifestyle;
		}

		public string Key { get; private set; }

		public Type ServiceType { get; private set; }

		public Type Classtype { get; private set; }

		public LifestyleType Lifestyle { get; private set; }

		public IEnumerable<Type> ForwardTypes { get; set; }

		public IDictionary<string, object> Parameters
		{
			get { return parameters; }
		}

		public object Instance { get; set; }

		internal bool IsResolvingDependencies { get; set; }

		public override string ToString()
		{
			return string.Format("Key:{0} - Service:{1} - Class:{2}", Key, ServiceType.Name, Classtype.Name);
		}
	}
}