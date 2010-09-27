using CompactContainer.Registrations;

namespace CompactContainer
{
	public static class Parameter
	{
		public static ParameterPart ForKey(string key)
		{
			return new ParameterPart(key);
		}
	}
}