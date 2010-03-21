using System;
using System.Linq;

namespace CompactContainer.DependencyResolvers
{
	public class ArrayDependencyResolver : IDependencyResolver
	{
		private readonly ICompactContainer container;

		public ArrayDependencyResolver(ICompactContainer container)
		{
			this.container = container;
		}

		public bool CanResolve(string key, Type type, ComponentInfo componentContext)
		{
			if (!type.IsArray)
				return false;

			var arrayType = type.GetElementType();
			return container.HasComponent(arrayType);
		}

		public object Resolve(string key, Type type, ComponentInfo componentContext)
		{
			if (!type.IsArray)
				return null;

			var arrayType = type.GetElementType();
			var services = container.GetServices(arrayType).ToArray();
			var typedArray = Array.CreateInstance(arrayType, services.Length);
			Array.Copy(services, typedArray, services.Length);

			return typedArray;
		}
	}
}