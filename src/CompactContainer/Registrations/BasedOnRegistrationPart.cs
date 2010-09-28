using System;

namespace CompactContainer.Registrations
{
	public class BasedOnRegistrationPart : WhereRegistrationPart
	{
		public BasedOnRegistrationPart(AllTypesRegistration allTypesRegistration, Type basedOnType)
			: base(allTypesRegistration, Accepts(basedOnType))
		{
			ServiceSelector = x => basedOnType;
		}

		protected static Func<Type, bool> Accepts(Type type)
		{
			return t => type.IsAssignableFrom(t) && t != type;
		}
	}
}