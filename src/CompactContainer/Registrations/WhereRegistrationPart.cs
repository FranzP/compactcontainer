using System;
using System.ComponentModel;

namespace CompactContainer.Registrations
{
	public class WhereRegistrationPart : IAllTypesRegistrationPart, IRegistration
	{
		private readonly AllTypesRegistration allTypesPart;
		private readonly Func<Type, bool> typeFilter;
		private Func<Type, Type> serviceSelector = x => x;
		private Action<ComponentRegistration> configurationDelegate;

		public WhereRegistrationPart(AllTypesRegistration allTypesPart, Func<Type, bool> typeFilter)
		{
			this.allTypesPart = allTypesPart;
			this.typeFilter = typeFilter;
		}

		public WhereRegistrationPart WithService(Func<Type, Type> serviceSelector)
		{
			this.serviceSelector = serviceSelector;
			return this;
		}

		public WhereRegistrationPart Configure(Action<ComponentRegistration> configurationDelegate)
		{
			this.configurationDelegate = configurationDelegate;
			return this;
		}

		public BasedOnRegistrationPart BasedOn<T>()
		{
			return allTypesPart.BasedOn<T>();
		}

		public BasedOnRegistrationPart BasedOn(Type basedOnType)
		{
			return allTypesPart.BasedOn(basedOnType);
		}

		public WhereRegistrationPart Where(Func<Type, bool> typeFilter)
		{
			return allTypesPart.Where(typeFilter);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		void IAllTypesRegistrationPart.ApplyTo(Type type, ICompactContainer container)
		{
			if (typeFilter(type))
			{
				var serviceType = serviceSelector(type);
				var registration = Component.For(serviceType);
				registration.ImplementedBy(type);

				if (configurationDelegate != null)
				{
					configurationDelegate(registration);
				}

				registration.Apply(container);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		void IRegistration.Apply(ICompactContainer container)
		{
			allTypesPart.Apply(container);
		}
	}
}