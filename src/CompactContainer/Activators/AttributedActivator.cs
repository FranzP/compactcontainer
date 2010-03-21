using System;
using CompactContainer.ConstructorResolvers;

namespace CompactContainer.Activators
{
	public class AttributedActivator : DefaultActivator
	{
		public AttributedActivator(ICompactContainer container)
			: this(container, typeof(InjectAttribute))
		{
		}

		public AttributedActivator(ICompactContainer container, Type attributeType)
			: base(container, new AttributedConstructorResolver(container, attributeType))
		{
		}
	}
}