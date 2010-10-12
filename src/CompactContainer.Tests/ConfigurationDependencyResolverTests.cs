using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class ConfigurationDependencyResolverTests
	{
		[Test]
		public void Can_supply_dependency_through_configuration()
		{
			var container = new CompactContainer();
			container.Configuration["CompactContainer.Tests.TestComponent.abc"] = 5;
			container.Register(Component.For<TestComponent>());

			var x = container.Resolve<TestComponent>();

			x.Abc.Should().Be.EqualTo(5);
		}
	}

	class TestComponent
	{
		public int Abc { get; private set; }

		public TestComponent(int abc)
		{
			Abc = abc;
		}
	}
}