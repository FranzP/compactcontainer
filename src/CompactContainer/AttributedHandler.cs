using System;

namespace InversionOfControl
{
	public class AttributedHandler : DefaultHandler
	{
		public AttributedHandler(ICompactContainer container)
			: this(container, typeof(InjectAttribute))
		{
		}

		public AttributedHandler(ICompactContainer container, Type attributeType)
			: base(container, new AttributedConstructorResolver(container, attributeType))
		{
		}
	}
}