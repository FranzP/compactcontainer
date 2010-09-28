using System;
using System.Collections.Generic;

namespace CompactContainer
{
	public interface IComponentSelector
	{
		bool HasOpinionAbout(Type service);

		ComponentInfo SelectComponent(Type service, IEnumerable<ComponentInfo> components);
	}
}