using System;
using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class CompactContainerEventsTests
	{
		[Test]
		public void Should_fire_component_registered_event_when_a_new_component_is_registered()
		{
			var container = new CompactContainer();

			var eventRisen = false;
			container.ComponentRegistered += x => { eventRisen = true; };

			container.Register(Component.For<ComponentA>());

			eventRisen.Should().Be.True();
		}

		[Test]
		public void Component_registered_event_parameter_should_match_recently_registered_component_info()
		{
			var container = new CompactContainer();

			Type registeredType = null;
			container.ComponentRegistered += x => { registeredType = x.Classtype; };

			container.Register(Component.For<ComponentA>());

			registeredType.Should().Be.EqualTo(typeof (ComponentA));
		}

		[Test]
		public void Should_fire_component_created_event_when_a_new_component_instaces_is_created()
		{
			var container = new CompactContainer();
			container.Register(Component.For<ComponentA>());

			var eventRisen = false;
			container.ComponentCreated += (ci, i) => { eventRisen = true; };

			container.Resolve<ComponentA>();

			eventRisen.Should().Be.True();
		}

		[Test]
		public void ComponentCreated_component_info_parameter_should_match_recently_added_component_info()
		{
			var container = new CompactContainer();
			container.Register(Component.For<ComponentA>());

			Type componentType = null;
			container.ComponentCreated += (ci, i) => componentType = ci.Classtype;

			container.Resolve<ComponentA>();

			componentType.Should().Be.EqualTo(typeof (ComponentA));
		}

		[Test]
		public void ComponentCreated_instace_parameter_should_be_equal_to_newly_created_instace()
		{
			var container = new CompactContainer();
			container.Register(Component.For<ComponentA>());

			object instance = null;
			container.ComponentCreated += (ci, i) => instance = i;

			var componentA = container.Resolve<ComponentA>();

			instance.Should().Be.EqualTo(componentA);
		}
	}
}