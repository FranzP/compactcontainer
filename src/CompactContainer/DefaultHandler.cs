using System;

namespace CompactContainer
{
	public class DefaultHandler : AbstractHandler
	{
		private readonly IConstructorResolver constructorResolver;

		public DefaultHandler(ICompactContainer container) 
			: this(container, new DefaultConstructorResolver(container))
		{
		}	
		
		public DefaultHandler(ICompactContainer container, IConstructorResolver constructorResolver)
		{
			Container = container;
			this.constructorResolver = constructorResolver;
		}

		public override object Create(Type classType)
		{
			ConstructionInfo theConstruction = constructorResolver.GetConstructionInfo(classType);
			return theConstruction.Construct();
		}
	}
}