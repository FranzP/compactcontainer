using System;
using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class ComponentRegistrationTests
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
			ci.Should().Not.Be.Null();
			ci.ServiceTypes.Should().Contain(typeof(IComponentA));
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
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
			ci.Should().Not.Be.Null();
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
		}

		[Test]
		public void Can_register_single_component_with_given_name()
		{
			container.Register(
				Component.For<IComponentA>().ImplementedBy(typeof(ComponentA)).Named("c")
				);

			var ci = container.Components.FindKey("c");
			ci.Should().Not.Be.Null();
			ci.ServiceTypes.Should().Contain(typeof(IComponentA));
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
		}

		[Test]
		public void Can_register_component_with_multiple_service_types()
		{
			container.Register(
				Component.For<ComponentA, IComponentA>().ImplementedBy<ComponentA>()
				);

			var ci = container.Components.FindServiceType(typeof(ComponentA));
			ci.Should().Not.Be.Null();
			ci.ServiceTypes.Should().Contain(typeof(IComponentA));
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
		}

		[Test]
		public void Can_register_component_with_multiple_service_types_using_forward()
		{
			container.Register(
				Component.For<ComponentA>().ImplementedBy<ComponentA>().Forward<IComponentA>()
				);

			var ci = container.Components.FindServiceType(typeof(ComponentA));
			ci.Should().Not.Be.Null();
			ci.ServiceTypes.Should().Contain(typeof(IComponentA));
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
		}

		[Test]
		public void Can_register_component_with_multiple_service_types_using_non_generic_forward()
		{
			container.Register(
				Component.For<ComponentA>().ImplementedBy<ComponentA>().Forward(typeof(IComponentA))
				);

			var ci = container.Components.FindServiceType(typeof(ComponentA));
			ci.Should().Not.Be.Null();
			ci.ServiceTypes.Should().Contain(typeof(IComponentA));
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
		}

		[Test]
		public void Can_register_component_and_apply_custom_action_to_registration()
		{
			container.Register(
				Component.For<ComponentA>().Apply(r =>
				                                  	{
				                                  		r.ImplementedBy<ComponentA>();
				                                  		r.Forward(typeof (IComponentA));
				                                  	})
				);

			var ci = container.Components.FindServiceType(typeof(ComponentA));
			ci.Should().Not.Be.Null();
			ci.ServiceTypes.Should().Contain(typeof(IComponentA));
			ci.Classtype.Should().Be.EqualTo(typeof(ComponentA));
		}

		[Test]
		public void Cannot_register_component_with_several_services_and_no_explicit_implementation()
		{
			Assert.Throws<CompactContainerException>(() => container.Register(Component.For<ComponentA, IComponentA>()));
		}

		[Test]
		public void Can_specify_component_parameters()
		{
			container.Register(
				Component.For<IComponentA>().ImplementedBy<ComponentA>()
					.With(Parameter.ForKey("p1").Eq("v1"),
					      Parameter.ForKey("p2").Eq(2)));

			var ci = container.Components.FindServiceType(typeof(IComponentA));
			ci.Parameters["p1"].Should().Be.EqualTo("v1");
			ci.Parameters["p2"].Should().Be.EqualTo(2);
		}

		[Test]
		public void Can_specify_concrete_instance_during_singleton_registration()
		{
			var a = new ComponentA();
			container.Register(Component.For<IComponentA>().Instance(a));

			var ci = container.Components.FindServiceType(typeof(IComponentA));
			ci.Instance.Should().Be.SameInstanceAs(a);
		}


		[Test]
		public void Should_throw_when_trying_to_register_abstract_class_as_implementation()
		{
			new Action(() => container.Register(Component.For<IComponentA>().ImplementedBy<AbstractComponentA>()))
				.Should().Throw<CompactContainerException>()
				.And.Exception.Message.Should().Be.EqualTo(
					"Cannot register implementation type CompactContainer.Tests.AbstractComponentA because it is abstract");
		}
	}
}