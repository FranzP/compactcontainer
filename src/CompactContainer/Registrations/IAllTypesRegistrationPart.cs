using System;

namespace CompactContainer.Registrations
{
	public interface IAllTypesRegistrationPart
	{
		void ApplyTo(Type type, ICompactContainer container);
	}
}