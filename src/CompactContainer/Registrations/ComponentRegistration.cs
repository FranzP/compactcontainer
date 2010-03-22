using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CompactContainer.Registrations
{
	public class ComponentRegistration<TServ> : IRegistration
	{
		protected IEnumerable<Type> ServiceTypes;

		private Type implementationType;
		private LifestyleType? lifestyle;
		private string name;
		private object instance;
		private IComponentRegistrationPart[] registrationParts;

		public ComponentRegistration()
		{
			ServiceTypes = new[] {typeof (TServ)};
		}

		public ComponentRegistration(params Type[] serviceTypes)
		{
			ServiceTypes = serviceTypes;
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

		public ComponentRegistration<TServ> Instance(TServ instance)
		{
			this.instance = instance;
			return this;
		}

		public ComponentRegistration<TServ> With(params IComponentRegistrationPart[] registrationParts)
		{
			this.registrationParts = registrationParts;
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Apply(ICompactContainer container)
		{
			if (implementationType == null)
			{
				if (instance != null)
				{
					implementationType = instance.GetType();
				}
				else
				{
					if (ServiceTypes.Count() != 1)
					{
						throw new CompactContainerException("Cannot infer implementation type when more than one service is specified: " +
						                                    ServiceTypes.Select(t => t.Name).ToCommandSeparatedString());
					}
					implementationType = ServiceTypes.Single();
				}
			}

			// name calculation
			if (name == null)
			{
				if (implementationType != null)
				{
					name = implementationType.FullName;
				}
				else if (instance != null)
				{
					name = instance.GetType().FullName;
				}
				else if (ServiceTypes.Count() > 0)
				{
					name = ServiceTypes.First().FullName;
				}
				else
				{
					throw new CompactContainerException("Cannot infer name for component");
				}
			}

			var ci = new ComponentInfo(name, ServiceTypes, implementationType, lifestyle ?? container.DefaultLifestyle)
			         	{
			         		Instance = instance,
			         	};

			if (registrationParts != null)
			{
				foreach (var part in registrationParts)
				{
					part.ApplyTo(ci);
				}
			}

			container.AddComponentInfo(ci);
		}
	}

	public class ComponentRegistration : ComponentRegistration<object>
	{
		public ComponentRegistration(params Type[] serviceTypes)
		{
			ServiceTypes = serviceTypes;
		}
	}
}