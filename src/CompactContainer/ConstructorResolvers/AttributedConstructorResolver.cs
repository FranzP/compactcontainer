using System;
using System.Linq;
using System.Reflection;

namespace CompactContainer.ConstructorResolvers
{
	public class AttributedConstructorResolver : DefaultConstructorResolver
	{
		private readonly Type attributeType;

		public AttributedConstructorResolver(ICompactContainer container, Type attributeType)
			: base(container)
		{
			this.attributeType = attributeType;
		}

		public override ConstructorInfo SelectConstructor(ComponentInfo componentInfo)
		{
			var ctor =
				componentInfo.Classtype.GetConstructors().SingleOrDefault(c => c.GetCustomAttributes(attributeType, false).Any())
				?? base.SelectConstructor(componentInfo);

			return ctor;
		}
	}
}