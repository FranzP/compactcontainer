using System;

namespace InversionOfControl
{
	public class ComponentInfo
	{
		private readonly string key;
		private readonly Type serviceType;
		private readonly Type classType;
		private object instance;
		private readonly LifestyleType lifestyle;
		private bool isResolvingDependencies;

		public string Key
		{
			get { return key; }
		}

		public Type ServiceType
		{
			get { return serviceType; }
		}

		public Type Classtype
		{
			get { return classType; }
		}

		public LifestyleType Lifestyle
		{
			get { return lifestyle; }
		}

		public object Instance
		{
			get { return instance; }
			set { instance = value; }
		}

		public bool IsResolvingDependencies
		{
			get { return isResolvingDependencies; }
			set { isResolvingDependencies = value; }
		}

		public ComponentInfo(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			this.key = key;
			this.serviceType = serviceType;
			this.classType = classType;
			this.lifestyle = lifestyle;
		}

		public override string ToString()
		{
			return string.Format("Key:{0} - Service:{1} - Class:{2}", key, serviceType.Name, classType.Name);
		}
	}

	public enum LifestyleType
	{
		Singleton,
		Transient,
	}
}