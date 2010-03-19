using System;
using System.Reflection;
using System.Text;

namespace CompactContainer
{
	public class AttributedConstructorResolver : DefaultConstructorResolver
	{
		private readonly ICompactContainer container;
		private readonly Type attributeType;

		public AttributedConstructorResolver(ICompactContainer container, Type attributeType)
			: base(container)
		{
			this.container = container;
			this.attributeType = attributeType;
		}

		public override ConstructionInfo GetConstructionInfo(Type classType)
		{
			ConstructorInfo theConstructor = GetInjectionConstructor(classType);
			if (theConstructor == null) return base.GetConstructionInfo(classType);


			var theConstruction = new ConstructionInfo();
			var missingComponents = new StringBuilder();

			ParameterInfo[] parameters = theConstructor.GetParameters();
			var parameterObjects = new object[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				if (container.HasComponent(parameters[i].ParameterType))
				{
					parameterObjects[i] = container.Resolve(parameters[i].ParameterType);
				}
				else
				{
					missingComponents.Append(parameters[i].ParameterType.Name + "; ");
					break;
				}
			}

			theConstruction.Constructor = theConstructor;
			theConstruction.Parameters = parameterObjects;

			if (theConstruction.Constructor == null)
			{
				throw new CompactContainerException("Missing components: " + missingComponents + " required by " + classType.Name);
			}

			return theConstruction;
		}

		private ConstructorInfo GetInjectionConstructor(Type classType)
		{
			ConstructorInfo[] allConstructors = classType.GetConstructors();
			if (allConstructors.Length < 1)
			{
				throw new CompactContainerException("Type " + classType.Name + " must have at least one public constructor");
			}
			ConstructorInfo theConstructor = null;
			for (int i = 0; i < allConstructors.Length; i++)
			{
				object[] attributes = allConstructors[i].GetCustomAttributes(attributeType, false);
				if (attributes.Length == 1)
				{
					theConstructor = allConstructors[i];
					break;
				}
			}
			return theConstructor;
		}
	}
}