namespace CompactContainer.Registrations
{
	public interface IComponentRegistrationPart
	{
		void ApplyTo(ComponentInfo componentInfo);
	}
}