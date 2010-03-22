using NUnit.Framework;

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

			Assert.That(e.ComponentA, Is.Not.Null);
			Assert.That(e.ComponentB, Is.Not.Null);
		}

		[Test]
		public void Should_not_inject_property_with_private_setter()
		{
			var e = container.Resolve<ComponentE>();

			Assert.That(e.ComponentC, Is.Null);
		}

		[Test]
		public void Should_leave_not_resolvable_optional_dependencies_as_null()
		{
			var e = container.Resolve<ComponentE>();

			Assert.That(e.ComponentD, Is.Null);
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