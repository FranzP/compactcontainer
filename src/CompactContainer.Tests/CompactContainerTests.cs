using System;
using System.Linq;
using CompactContainer.Activators;
using NUnit.Framework;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class CompactContainerTests
	{
		private CompactContainer container;

		[SetUp]
		public void Init()
		{
			container = new CompactContainer();
		}

		[Test]
		public void HasComponent_verifies_that_component_is_actually_registered_using_service_type()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>());
			Assert.IsTrue(container.HasComponent(typeof(IComponentA)));
		}

		[Test]
		public void HasComponent_verifies_that_component_is_actually_registered_using_component_key()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("comp.a"));
			Assert.IsTrue(container.HasComponent("comp.a"));
		}

		[Test]
		public void Throws_exception_when_registering_two_components_with_same_key()
		{
			var ex = Assert.Throws<CompactContainerException>(() => container.Register(Component.For<ComponentA>().Named("comp"),
			                                                                  Component.For<ComponentB>().Named("comp")));
			Assert.That(ex.Message, Is.EqualTo("A component with the same key is already registered: \"comp\""));
		}

		[Test]
		public void A_single_instances_should_be_returned_for_resolve_calls_for_singleton_components()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("comp.a"));

			var compA1 = (IComponentA)container.Resolve(typeof(IComponentA));
			var compA2 = (IComponentA)container.Resolve("comp.a");

			Assert.That(compA1, Is.Not.Null);
			Assert.That(compA1, Is.SameAs(compA2));
		}

		[Test]
		public void Can_resolve_components_using_generic_syntax()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("comp.a"));

			var compA1 = container.Resolve<IComponentA>();
			var compA2 = container.Resolve<IComponentA>("comp.a");

			Assert.That(compA1, Is.Not.Null);
			Assert.That(compA1, Is.SameAs(compA2));
		}

		[Test]
		public void Can_resolve_singleton_using_constructor_injection()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compA = container.Resolve<IComponentA>();
			var compB = container.Resolve<IComponentB>();

			Assert.That(compA, Is.Not.Null);
			Assert.That(compA, Is.SameAs(compB.CompA));
		}

		[Test]
		public void Different_instances_should_be_returned_for_resolve_calls_for_transient_components()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().WithLifestyle(LifestyleType.Transient));

			var compA1 = container.Resolve<IComponentA>();
			var compA2 = container.Resolve<IComponentA>();

			Assert.AreNotSame(compA1, compA2);
		}

		[Test]
		public void Default_resolution_should_use_best_candidate_constructor()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentC>().ImplementedBy<ComponentC>().Named("comp.c1"),
			                   Component.For<IComponentC>().ImplementedBy<ComponentC>().Named("comp.c2"));

			var compC1 = container.Resolve<IComponentC>("comp.c1");
			Assert.That(compC1.ConstructorUsed, Is.EqualTo(1));
			Assert.That(compC1.CompA, Is.Not.Null);
			Assert.That(compC1.CompB, Is.Null);

			container.Register(Component.For<IComponentB>().ImplementedBy<ComponentB>());
			
			var compC2 = container.Resolve<IComponentC>("comp.c2");
			Assert.That(compC2.ConstructorUsed, Is.EqualTo(2));
			Assert.That(compC2.CompA, Is.Not.Null);
			Assert.That(compC2.CompB, Is.Not.Null);

			Assert.That(compC1, Is.Not.SameAs(compC2));
		}

		[Test]
		public void Using_AttributeActivator_should_select_constructor_by_attribute_decoration()
		{
			container.DefaultActivator = new AttributedActivator(container);
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
							   Component.For<IComponentC>().ImplementedBy<ComponentC>().Named("comp.c1"),
							   Component.For<IComponentC>().ImplementedBy<ComponentC>().Named("comp.c2"));

			var compC1 = container.Resolve<IComponentC>("comp.c1");
			Assert.That(compC1.ConstructorUsed, Is.EqualTo(1));
			Assert.That(compC1.CompA, Is.Not.Null);
			Assert.That(compC1.CompB, Is.Null);

			container.Register(Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compC2 = container.Resolve<IComponentC>("comp.c2");
			Assert.That(compC2.ConstructorUsed, Is.EqualTo(1));
			Assert.That(compC2.CompA, Is.Not.Null);
			Assert.That(compC2.CompB, Is.Null);

			Assert.That(compC1, Is.Not.SameAs(compC2));
		}

		[Test]
		public void Can_resolve_component_with_provided_instance()
		{
			IComponentA compA1 = new ComponentA();

			container.Register(Component.For<IComponentA>().Instance(compA1));
			Assert.AreSame(compA1, container.Resolve<IComponentA>());

			container.Register(Component.For<IComponentA>().Instance(compA1).Named("comp.a2"));
			Assert.AreSame(compA1, container.Resolve<IComponentA>("comp.a2"));
		}

		[Test]
		public void Container_should_register_itself()
		{
			Assert.AreSame(container, container.Resolve<ICompactContainer>());
			Assert.AreSame(container, container.Resolve("container"));
		}

		[Test]
		public void Can_resolve_all_components_that_provide_a_given_service()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentA>().ImplementedBy<ComponentAA>(),
			                   Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compsA1 = container.GetServices(typeof(IComponentA));
			Assert.That(compsA1.Count(), Is.EqualTo(2));

			var compsA2 = container.GetServices<IComponentA>();
			Assert.That(compsA2.Count(), Is.EqualTo(2));

			Assert.That(compsA1, Is.EquivalentTo(compsA2));
		}

		[Test]
		public void Should_detect_circular_reference()
		{
			container.Register(Component.For<IDependentX>().ImplementedBy<DependentX>(),
			                   Component.For<IDependentY>().ImplementedBy<DependentY>());

			var ex = Assert.Throws<CompactContainerException>(() => container.Resolve<IDependentY>());
			Assert.That(ex.Message, Is.EqualTo("Circular reference: Key:CompactContainer.Tests.DependentY - Services:IDependentY - Class:DependentY"));
		}

		[Test]
		public void Should_use_custom_activator_when_registered()
		{
			container.RegisterActivator<IStartable>(new StartableActivator(container));
			container.Register(Component.For<IComponentU>().ImplementedBy<StartableComponent>());

			var cu = container.Resolve<IComponentU>();

			Assert.That(cu.A, Is.EqualTo(1));
		}

		[Test]
		public void Should_use_default_lifestyle_as_configured_in_the_container()
		{
			container.DefaultLifestyle = LifestyleType.Transient;
			container.Register(Component.For<ComponentA>());
			
			var component1 = container.Resolve<ComponentA>();
			var component2 = container.Resolve<ComponentA>();
			Assert.That(component1, Is.Not.SameAs(component2));
		}

		[Test]
		public void Should_auto_register_concrete_type_when_convention_is_registered()
		{
			container.Register(
				Component.For<IAutoRegisterConvention>().ImplementedBy<AutoRegisterConventions.ConcreteTypeConvention>());
			var a = container.Resolve<ComponentA>();
			Assert.IsNotNull(a);
		}

        [Test]
		public void Should_auto_register_interface_type_when_convention_is_registered()
        {
			container.Register(
				Component.For<IAutoRegisterConvention>().ImplementedBy<AutoRegisterConventions.InterfaceTypeConvention>());
			var a = container.Resolve<IComponentA>();
        	Assert.That(a, Is.InstanceOf(typeof (ComponentA)));
        }

		[Test]
		public void Can_remove_registered_component_by_key()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("a"));

			container.RemoveComponent("a");

			Assert.That(container.HasComponent("a"), Is.False);
		}

		[Test]
		public void ThrowsWhenTryingToRemoveNotExistentComponent()
		{
			var ex = Assert.Throws<CompactContainerException>(() => container.RemoveComponent("a"));
			Assert.That(ex.Message, Is.EqualTo("There is not any component registered with the given key: \"a\""));
		}

        [Test]
        public void Singleton_components_should_be_disposed_when_the_container_is_disposed()
        {
			container.Register(Component.For<DisposableComponent>());
            var disposable = container.Resolve<DisposableComponent>();

        	Assert.That(disposable.DisposedCalledCount, Is.EqualTo(0));

            container.Dispose();

			Assert.That(disposable.DisposedCalledCount, Is.EqualTo(1));
        }
	}

	public class StartableActivator : IActivator
	{
		private readonly ICompactContainer container;

		public StartableActivator(ICompactContainer container)
		{
			this.container = container;
		}

		public object Create(ComponentInfo componentInfo)
		{
			var product = (IStartable)container.DefaultActivator.Create(componentInfo);
			product.Start();
			return product;
		}
	}

	public interface IStartable
	{
		void Start();
		void Stop();
	}

	public class StartableComponent : IComponentU, IStartable
	{
		public int a;

		public int A
		{
			get { return a; }
			set { a = value; }
		}

		public void Start()
		{
			a = 1;
		}

		public void Stop()
		{
			a = 2;
		}
	}

	public interface IComponentU
	{
		int A { get; set; }
	}

    public class DisposableComponent : IDisposable
    {
        public int DisposedCalledCount { get; set; }

        public void Dispose()
        {
            DisposedCalledCount++;
        }
    }
}