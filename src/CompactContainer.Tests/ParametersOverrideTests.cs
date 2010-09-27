using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class ParametersOverrideTests
	{
		private CompactContainer container;

		[SetUp]
		public void Init()
		{
			container = new CompactContainer();
		}
		
		[Test]
		public void Should_use_parameter_override_when_resolving_component()
		{
			var specificCompA = new ComponentA();
			container.Register(
				Component.For<IComponentB>().ImplementedBy<ComponentB>()
					.With(Parameter.ForKey("compA").EqualsTo(specificCompA))
				);

			var compb = container.Resolve<IComponentB>();

			compb.CompA.Should().Be.SameInstanceAs(specificCompA);
		}

		[Test]
		public void Should_throw_meaninful_exception_when_parameter_type_is_wrong()
		{
			container.Register(
				Component.For<IComponentB>().ImplementedBy<ComponentB>()
					.With(Parameter.ForKey("compA").EqualsTo("x"))
				);

			var ex = Assert.Throws<CompactContainerException>(() => container.Resolve<IComponentB>());
			ex.Message.Should().Be.EqualTo("Cannot convert parameter override \"compA\" to type CompactContainer.Tests.IComponentA for component Key:CompactContainer.Tests.ComponentB - Services:IComponentB - Class:ComponentB");
		}
	}
}