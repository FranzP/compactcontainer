using System;
using System.Reflection;
using System.Text;

namespace InversionOfControl
{
	public class DefaultHandler : AbstractHandler
	{
		public DefaultHandler(ICompactContainer container)
		{
			Container = container;
		}

		public override object Create(Type classType)
		{
			ConstructorInfo[] constructors = classType.GetConstructors();
			if (constructors.Length < 1)
			{
				throw new CompactContainerException("Type " + classType.Name + " must have at least one public constructor");
			}

			ConstructorInfo theConstructor = null;
			object[] theConstructorParameters = new object[0];

			StringBuilder missingComponents = new StringBuilder();

			foreach (ConstructorInfo constructorInfo in constructors)
			{

				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length > theConstructorParameters.Length
				    || theConstructor == null)
				{


					bool proposeNewConstructor = true;
					object[] parameterObjects = new object[parameters.Length];

					for (int i = 0; i < parameters.Length; i++)
					{
						if (Container.HasComponent(parameters[i].ParameterType))
						{
							parameterObjects[i] = Container.Resolve(parameters[i].ParameterType);
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
						theConstructor = constructorInfo;
						theConstructorParameters = parameterObjects;
					}
				}
			}

			if (theConstructor == null)
			{
				throw new CompactContainerException("Missing components: " + missingComponents + " required by " + classType.Name);
			}

			return theConstructor.Invoke(theConstructorParameters);
		}
	}
}