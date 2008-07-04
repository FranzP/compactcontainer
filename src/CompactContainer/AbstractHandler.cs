using System;

namespace InversionOfControl
{
	public abstract class AbstractHandler : IHandler
	{
		private ICompactContainer container;

		public ICompactContainer Container
		{
			get { return container; }
			set { container = value; }
		}

		public abstract object Create(Type classType);
	}
}