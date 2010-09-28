using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class ComponentSelectorTests
	{
		[Test]
		public void Can_select_implementation_according_to_registered_component_selector()
		{
			var container = new CompactContainer();

			container.Register(AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
			                   	.BasedOn<IService>()
								.BasedOn<IComponentSelector>()
				);
			container.RegisterComponentSelector(new ServiceSelector());

			var service = container.Resolve<IService>();
			service.Should().Be.OfType<ComplexService>();
		}

		class ServiceSelector : IComponentSelector
		{
			public bool HasOpinionAbout(Type service)
			{
				return service == typeof (IService);
			}

			public ComponentInfo SelectComponent(Type service, IEnumerable<ComponentInfo> components)
			{
				return components.FirstOrDefault(c => c.Classtype.Name.StartsWith("Complex"));
			}
		}

		interface IService { }

		class DummyService : IService { }

		class ComplexService : IService { }
	}
}