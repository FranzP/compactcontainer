using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CompactContainer.Registrations
{
	public class ComponentRegistration<TServ> : IRegistration
	{
		protected Type serviceType;
		private Type implementationType;
		private LifestyleType? lifestyle;
		private string name;
		private readonly List<Type> forwardTypes = new List<Type>();

		public ComponentRegistration()
		{
			serviceType = typeof(TServ);
		}

		public ComponentRegistration<TServ> Named(string name)
		{
			this.name = name;
			return this;
		}

		public ComponentRegistration<TServ> ImplementedBy<TImpl>()
			where TImpl : TServ
		{
			return ImplementedBy(typeof (TImpl));
		}

		public ComponentRegistration<TServ> ImplementedBy(Type implementationType)
		{
			this.implementationType = implementationType;
			name = implementationType.FullName;
			return this;
		}

		public ComponentRegistration<TServ> WithLifestyle(LifestyleType lifestyle)
		{
			this.lifestyle = lifestyle;
			return this;
		}

		public ComponentRegistration<TServ> Forward<TF1>()
		{
			forwardTypes.Add(typeof(TF1));
			return this;
		}		
		
		public ComponentRegistration<TServ> Forward<TF1, TF2>()
		{
			forwardTypes.Add(typeof(TF1));
			forwardTypes.Add(typeof(TF2));
			return this;
		}

		public ComponentRegistration<TServ> Forward(params Type[] types)
		{
			forwardTypes.AddRange(types);
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void Apply(ICompactContainer container)
		{
			if (implementationType == null)
				implementationType = serviceType;

			var ci = new ComponentInfo(name, serviceType, implementationType, lifestyle ?? container.DefaultLifestyle)
			         	{
			         		ForwardTypes = forwardTypes
			         	};
			container.AddComponentInfo(ci);
		}
	}

	public class ComponentRegistration : ComponentRegistration<object>
	{
		public ComponentRegistration(Type serviceType)
		{
			this.serviceType = serviceType;
		}
	}
}