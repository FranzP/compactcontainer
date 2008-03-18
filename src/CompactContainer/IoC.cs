namespace InversionOfControl
{
	public static class IoC
	{
		private static ICompactContainer container;

		public static void Initialize(ICompactContainer container)
		{
			Container = container;
		}

		public static T Resolve<T>()
		{
			return Container.Resolve<T>();
		}

		public static T Resolve<T>(string name)
		{
			return Container.Resolve<T>(name);
		}

		public static ICompactContainer Container
		{
			get { return container; }
			set { container = value; }
		}

		public static bool IsInitialized
		{
			get { return container != null; }
		}

		public static void Reset()
		{
			container.Dispose();
			container = null;
		}

	}
}