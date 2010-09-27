using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class PropertyInjectionTests
	{
		private CompactContainer container;

		[SetUp]
		public void Init()
		{
			container = new CompactContainer();

			container.Register(
				Component.For<IComponentA>().ImplementedBy<ComponentA>(),
				Component.For<IComponentB>().ImplementedBy<ComponentB>(),
				Component.For<IComponentC>().ImplementedBy<ComponentC>(),
				Component.For<ComponentE>()
				);
		}

		[Test]
		public void Can_mix_ctor_with_property_injection()
		{
			var e = container.Resolve<ComponentE>();

			e.ComponentA.Should().Not.Be.Null();
			e.ComponentB.Should().Not.Be.Null();
		}

		[Test]
		public void Should_not_inject_property_with_private_setter()
		{
			var e = container.Resolve<ComponentE>();

			e.ComponentC.Should().Be.Null();
		}

		[Test]
		public void Should_leave_not_resolvable_optional_dependencies_as_null()
		{
			var e = container.Resolve<ComponentE>();

			e.ComponentD.Should().Be.Null();
		}

		public class ComponentE
		{
			public IComponentA ComponentA { get; private set; }

			public IComponentB ComponentB { get; set; }

			public IComponentC ComponentC { get; private set; }

			public IComponentU ComponentD { get; set; }

			public ComponentE(IComponentA componentA)
			{
				ComponentA = componentA;
			}
		}
	}
}