using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CompactContainer.Registrations
{
	public class AllTypesRegistration : IRegistration
	{
		private readonly Assembly assembly;
		private readonly List<IAllTypesRegistrationPart> parts = new List<IAllTypesRegistrationPart>();

		public AllTypesRegistration(Assembly assembly)
		{
			this.assembly = assembly;
		}

		public BasedOnRegistrationPart BasedOn<T>()
		{
			var basedOnRegistrationPart = new BasedOnRegistrationPart(this, typeof(T));
			parts.Add(basedOnRegistrationPart);
			return basedOnRegistrationPart;
		}

		public BasedOnRegistrationPart BasedOn(Type basedOnType)
		{
			var basedOnRegistrationPart = new BasedOnRegistrationPart(this, basedOnType);
			parts.Add(basedOnRegistrationPart);
			return basedOnRegistrationPart;
		}

		public WhereRegistrationPart Where(Func<Type, bool> typeFilter)
		{
			var whereRegistrationPart = new WhereRegistrationPart(this, typeFilter);
			parts.Add(whereRegistrationPart);
			return whereRegistrationPart;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Apply(ICompactContainer container)
		{
			// by default we scan all concrete types of the pointed assembly (even non-public ones)
			// and we register each type as many times as needed acording to the specified configuration
			// (a given type can be registered more than once with different services)

			foreach (var type in assembly.GetTypes().Where(t => t.IsAbstract == false && t.IsClass))
			{
				foreach (var part in parts)
				{
					part.ApplyTo(type, container);
				}
			}
		}
	}
}