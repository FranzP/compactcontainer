using System.Reflection;

namespace InversionOfControl
{
	public class ConstructionInfo
	{
		public ConstructionInfo()
		{
			Parameters = new object[0];
		}

		public ConstructorInfo Constructor { get; set; }
		public object[] Parameters { get; set; }

		public object Construct()
		{
			return Constructor.Invoke(Parameters);
		}
	}
}