using System;
using System.Reflection;
using System.Text;

namespace InversionOfControl
{
	public class DefaultConstructorResolver : IConstructorResolver
	{
		private readonly ICompactContainer container;

		public DefaultConstructorResolver(ICompactContainer container)
		{
			this.container = container;
		}

		public virtual ConstructionInfo GetConstructionInfo(Type classType)
		{
			ConstructorInfo[] constructors = classType.GetConstructors();
			if (constructors.Length < 1)
			{
				throw new CompactContainerException("Type " + classType.Name + " must have at least one public constructor");
			}

			var theConstruction = new ConstructionInfo();
			var missingComponents = new StringBuilder();

			foreach (ConstructorInfo constructorInfo in constructors)
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length > theConstruction.Parameters.Length
				    || theConstruction.Constructor == null)
				{

					bool proposeNewConstructor = true;
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
							proposeNewConstructor = false;
							break;
						}
					}

					if (proposeNewConstructor)
					{
						theConstruction.Constructor = constructorInfo;
						theConstruction.Parameters = parameterObjects;
					}
				}
			}

			if (theConstruction.Constructor == null)
			{
				throw new CompactContainerException("Missing components: " + missingComponents + " required by " + classType.Name);
			}

			return theConstruction;
		}
	}
}