namespace CompactContainer
{
	public interface IActivator
	{
		object Create(ComponentInfo componentInfo);
	}
}