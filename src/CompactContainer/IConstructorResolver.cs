using System.Reflection;

namespace CompactContainer
{
	public interface IConstructorResolver
	{
		ConstructorInfo SelectConstructor(ComponentInfo componentInfo);
	}
}