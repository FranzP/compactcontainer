using System;

namespace CompactContainer.Registrations
{
	public static class Component
	{
		public static ComponentRegistration<TServ> For<TServ>()
		{
			return new ComponentRegistration<TServ>();
		}

		public static ComponentRegistration For(Type serviceType)
		{
			return new ComponentRegistration(serviceType);
		}
	}
}