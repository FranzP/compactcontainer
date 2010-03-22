using NUnit.Framework;

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

			Assert.That(compb.CompA, Is.SameAs(specificCompA));
		}
	}
}