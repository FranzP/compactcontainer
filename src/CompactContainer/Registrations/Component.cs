using System;
using CompactContainer.Registrations;

namespace CompactContainer
{
	public static class Component
	{
		public static ComponentRegistration<TServ> For<TServ>()
		{
			return new ComponentRegistration<TServ>();
		}		
		
		public static ComponentRegistration For<TS1, TS2>()
		{
			return new ComponentRegistration(new[] {typeof (TS1), typeof (TS2)});
		}
	
		public static ComponentRegistration For<TS1, TS2, TS3>()
		{
			return new ComponentRegistration(new[] {typeof (TS1), typeof (TS2), typeof (TS3)});
		}

		public static ComponentRegistration For(params Type[] serviceTypes)
		{
			return new ComponentRegistration(serviceTypes);
		}
	}
}