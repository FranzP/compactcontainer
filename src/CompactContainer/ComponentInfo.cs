using System;

namespace CompactContainer
{
	public class ComponentInfo
	{
		public string Key { get; private set; }

		public Type ServiceType { get; private set; }

		public Type Classtype { get; private set; }

		public LifestyleType Lifestyle { get; private set; }

		public object Instance { get; set; }

		public bool IsResolvingDependencies { get; set; }

		public ComponentInfo(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			Key = key;
			ServiceType = serviceType;
			Classtype = classType;
			Lifestyle = lifestyle;
		}

		public override string ToString()
		{
			return string.Format("Key:{0} - Service:{1} - Class:{2}", Key, ServiceType.Name, Classtype.Name);
		}
	}
}