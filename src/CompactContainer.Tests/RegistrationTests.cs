using CompactContainer.Registrations;
using NUnit.Framework;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class RegistrationTests
	{
		private CompactContainer container;

		[SetUp]
		public void SetUp()
		{
			container = new CompactContainer();
		}

		[Test]
		public void Can_register_single_component_specifying_service_and_implementation_using_generic_registration()
		{
			container.Register(
				Component.For<IComponentA>().ImplementedBy<ComponentA>()
				);

			var ci = container.Components.FindServiceType(typeof(IComponentA));
			Assert.That(ci, Is.Not.Null);
			Assert.That(ci.ServiceTypes, Has.Member(typeof(IComponentA)));
			Assert.That(ci.Classtype, Is.EqualTo(typeof(ComponentA)));
		}

		[Test]
		public void Can_register_single_component_specifying_service_and_implementation_using_non_generic_registration()
		{
			container.Register(
				Component.For(typeof(IComponentA)).ImplementedBy(typeof(ComponentA))
				);

			Assert.That(container.HasComponent(typeof(IComponentA)));
		}

		[Test]
		public void Can_register_single_component_specifying_only_service_using_generic_registration()
		{
			container.Register(
				Component.For<ComponentA>()
				);

			var ci = container.Components.FindServiceType(typeof(ComponentA));
			Assert.That(ci, Is.Not.Null);
			Assert.That(ci.Classtype, Is.EqualTo(typeof(ComponentA)));
		}

		[Test]
		public void Can_register_single_component_with_given_name()
		{
			container.Register(
				Component.For<IComponentA>().ImplementedBy(typeof(ComponentA)).Named("c")
				);

			var ci = container.Components.FindKey("c");
			Assert.That(ci, Is.Not.Null);
			Assert.That(ci.ServiceTypes, Has.Member(typeof(IComponentA)));
			Assert.That(ci.Classtype, Is.EqualTo(typeof(ComponentA)));
		}

		[Test]
		public void Can_register_component_with_forward_type()
		{
			container.Register(
				Component.For<ComponentA, IComponentA>().ImplementedBy<ComponentA>()
				);

			var ci = container.Components.FindServiceType(typeof(ComponentA));
			Assert.That(ci, Is.Not.Null);
			Assert.That(ci.ServiceTypes, Has.Member(typeof(IComponentA)));
			Assert.That(ci.Classtype, Is.EqualTo(typeof(ComponentA)));
		}

		[Test]
		public void Cannot_register_component_with_several_services_and_no_explicit_implementation()
		{
			Assert.Throws<CompactContainerException>(() => container.Register(Component.For<ComponentA, IComponentA>()));
		}
	}
}