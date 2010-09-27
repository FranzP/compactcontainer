using System;

namespace CompactContainer.Registrations
{
	public class BasedOnRegistrationPart : WhereRegistrationPart
	{
		public BasedOnRegistrationPart(AllTypesRegistration allTypesRegistration, Type basedOnType)
			: base(allTypesRegistration, Accepts(basedOnType))
		{
		}

		protected static Func<Type, bool> Accepts(Type type)
		{
			return t =>
			       	{
			       		return type.IsAssignableFrom(t) && t != type;
			       	};
		}
	}
}