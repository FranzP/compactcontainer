using System;
using System.Collections;
using System.Collections.Generic;

namespace InversionOfControl
{
	public class ComponentList : IEnumerable<ComponentInfo>
	{
		private int singletonsCount;
		private readonly List<ComponentInfo> list = new List<ComponentInfo>();

		public int SingletonsCount
		{
			get { return singletonsCount; }
		}

		public void Add(ComponentInfo ci)
		{
			if (ci.Lifestyle == LifestyleType.Singleton)
			{
				singletonsCount++;
			}
			list.Add(ci);
		}

		public bool Remove(ComponentInfo ci)
		{
			if (ci.Lifestyle == LifestyleType.Singleton)
			{
				singletonsCount--;
			}
			return list.Remove(ci);
		}

		public ComponentInfo this[int index]
		{
			get { return list[index]; }
		}

		public List<ComponentInfo> GetAllImplementorsFor(Type serviceType)
		{
			return list.FindAll(delegate(ComponentInfo ci)
			{
				return (ci.ServiceType == serviceType);
			});
		}

		public ComponentInfo FindServiceType(Type serviceType)
		{
			return list.Find(delegate(ComponentInfo ci)
			{
				return (ci.ServiceType.Equals(serviceType));
			});
		}

		public ComponentInfo FindClassType(Type classType)
		{
			return list.Find(delegate(ComponentInfo ci)
			{
				return (ci.Classtype.Equals(classType));
			});
		}

		public ComponentInfo FindKey(string key)
		{
			return list.Find(delegate(ComponentInfo ci)
			{
				return (ci.Key.Equals(key));
			});
		}


		public IEnumerator<ComponentInfo> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)list).GetEnumerator();
		}

	}
}