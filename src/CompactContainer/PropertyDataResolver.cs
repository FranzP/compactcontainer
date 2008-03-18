using System;
using System.Reflection;

namespace InversionOfControl
{
	public class PropertyDataResolver : IPropertyDataResolver
	{
		private const char SEPARATOR = ',';

		private readonly ICompactContainer container;

		public PropertyDataResolver(ICompactContainer container)
		{
			this.container = container;
		}

		public PropertyData Get(string qualifier)
		{
			PropertyData data = new PropertyData();

			string[] parts = qualifier.Split(SEPARATOR);
			string typeName = parts[0].Trim();
			string assemblyName = parts[1].Trim();
			data.Name = parts[2].Trim();

			Type type = Type.GetType(typeName + ", " + assemblyName, true);
			data.Owner = container.Resolve(type);
			data.Info = type.GetProperty(data.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			data.Value = data.Info.GetValue(data.Owner, null);

			return data;
		}
	}

	public struct PropertyData
	{
		public object Owner;
		public string Name;
		public object Value;
		public PropertyInfo Info;
	}
}