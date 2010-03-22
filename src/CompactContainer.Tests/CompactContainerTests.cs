using System;
using System.Collections.Generic;
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
		public void CanAddComponent()
		{
            Assert.IsFalse(container.HasComponent("comp.a"));
			Assert.IsFalse(container.HasComponent(typeof(IComponentA))); 

			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("comp.a"));

			Assert.IsTrue(container.HasComponent(typeof(IComponentA)));
			Assert.IsTrue(container.HasComponent("comp.a"));
		}

		[Test]
		[ExpectedException(typeof(CompactContainerException))]
		public void CannotAddDuplicatedComponent()
		{
			container.Register(Component.For<ComponentA>().Named("comp"),
			                   Component.For<ComponentB>().Named("comp"));
		}

		[Test]
		public void CanResolveSingletonComponent()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("comp.a"));

			var compA1 = (IComponentA)container.Resolve(typeof(IComponentA));
			var compA2 = (IComponentA)container.Resolve("comp.a");

			Assert.IsNotNull(compA1);
			Assert.AreSame(compA1, compA2);
		}

		[Test]
		public void CanResolveSingletonComponentUsingGenerics()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("comp.a"));

			var compA1 = container.Resolve<IComponentA>();
			var compA2 = container.Resolve<IComponentA>("comp.a");

			Assert.IsNotNull(compA1);
			Assert.AreSame(compA1, compA2);
		}

		[Test]
		public void CanResolveSingletonComponentWithConstructorInjection()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compB = (IComponentB)container.Resolve(typeof(IComponentB));

			Assert.AreSame(container.Resolve(typeof(IComponentA)), compB.CompA);
		}

		[Test]
		public void CanResolveTransientComponent()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().WithLifestyle(LifestyleType.Transient));

			var compA1 = container.Resolve<IComponentA>();
			var compA2 = container.Resolve<IComponentA>();

			Assert.AreNotSame(compA1, compA2);
		}

		[Test]
		public void CanRegisterAndResolveComponentWithoutService()
		{
			container.Register(Component.For<ComponentA>());

			var compA1 = container.Resolve<ComponentA>();
			var compA2 = container.Resolve<ComponentA>();
			Assert.AreSame(compA2, compA1);
		}

		[Test]
		public void CanRegisterAndResolveComponentWithMultipleConstructors()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentC>().ImplementedBy<ComponentC>().Named("comp.c1"),
			                   Component.For<IComponentC>().ImplementedBy<ComponentC>().Named("comp.c2"));

			var compC1 = container.Resolve<IComponentC>("comp.c1");
			Assert.AreEqual(1, compC1.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compC1.CompA);
			Assert.AreSame(null, compC1.CompB);

			container.Register(Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compC2 = container.Resolve<IComponentC>("comp.c2");
			Assert.AreEqual(2, compC2.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compC2.CompA);
			Assert.AreSame(container.Resolve<IComponentB>(), compC2.CompB);

			Assert.AreNotSame(compC1, compC2);
		}

		[Test]
		public void CanRegisterAndResolveComponentWithMultipleConstructorsUsingCustomAttribute()
		{
			container.DefaultActivator = new AttributedActivator(container);
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentD>().ImplementedBy<ComponentD>().Named("comp.d1"),
			                   Component.For<IComponentD>().ImplementedBy<ComponentD>().Named("comp.d2"));

			var compD1 = container.Resolve<IComponentD>("comp.d1");
			Assert.AreEqual(1, compD1.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compD1.CompA);
			Assert.AreSame(null, compD1.CompB);

			container.Register(Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compD2 = container.Resolve<IComponentD>("comp.d2");
			Assert.AreEqual(1, compD2.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compD2.CompA);
			Assert.AreSame(null, compD2.CompB);

			Assert.AreNotSame(compD1, compD2);
		}

		[Test]
		public void CanAddComponentWithInstance()
		{
			IComponentA compA1 = new ComponentA();

			container.Register(Component.For<IComponentA>().Instance(compA1));
			Assert.AreSame(compA1, container.Resolve<IComponentA>());

			container.Register(Component.For<IComponentA>().Instance(compA1).Named("comp.a2"));
			Assert.AreSame(compA1, container.Resolve<IComponentA>("comp.a2"));
		}

		[Test]
		public void RegisterSelf()
		{
			Assert.AreSame(container, container.Resolve<ICompactContainer>());
		}

		[Test]
		public void CanGetAllComponentsThatImplementService()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>(),
			                   Component.For<IComponentA>().ImplementedBy<ComponentAA>(),
			                   Component.For<IComponentB>().ImplementedBy<ComponentB>());

			var compsA1 = container.GetServices(typeof(IComponentA));
			Assert.AreEqual(2, compsA1.Count());

			var compsA2 = container.GetServices<IComponentA>();
			Assert.AreEqual(2, compsA1.Count());

			Assert.AreEqual(compsA1.First(), compsA2.First());
			Assert.AreEqual(compsA1.ElementAt(1), compsA2.ElementAt(1));
		}

		[Test]
		public void DetectCircularReferences()
		{
			container.Register(Component.For<IDependentX>().ImplementedBy<DependentX>(),
			                   Component.For<IDependentY>().ImplementedBy<DependentY>());

			var ex = Assert.Throws<CompactContainerException>(() => container.Resolve<IDependentY>());
			Assert.That(ex.Message, Is.EqualTo("Circular reference: Key:CompactContainer.Tests.DependentY - Services:IDependentY - Class:DependentY"));
		}

		[Test]
		public void CanResolveWithHandler()
		{
			container.RegisterActivator<IStartable>(new StartableActivator(container));
			container.Register(Component.For<IComponentU>().ImplementedBy<StartableComponent>());

			var cu = container.Resolve<IComponentU>();

			Assert.AreEqual(1, cu.A);
		}

		[Test]
		public void ResolutionOfClassCanUseDefaultLifestyleTypeTransient()
		{
			container.DefaultLifestyle = LifestyleType.Transient;
			container.Register(Component.For<ComponentA>());
			
			var component1 = container.Resolve<ComponentA>();
			var component2 = container.Resolve<ComponentA>();
			Assert.AreNotSame(component1, component2);
		}


		[Test]
		public void AutoregisterConcreteTypesWhenResolving()
		{
			container.Register(
				Component.For<IAutoRegisterConvention>().ImplementedBy<AutoRegisterConventions.ConcreteTypeConvention>());
			var a = container.Resolve<ComponentA>();
			Assert.IsNotNull(a);
		}

        [Test]
        public void AutoregisterInterfaceTypeUsingDefaultNamingConvention()
        {
            // new feature - auto register an interface type when a concrete component can be resolved with matching name
			container.Register(
				Component.For<IAutoRegisterConvention>().ImplementedBy<AutoRegisterConventions.AbstractTypeConvention>());
			var a = container.Resolve<IComponentA>();
            Assert.IsInstanceOfType(typeof(ComponentA), a);
        }

		[Test]
		public void CanRemoveRegisteredComponent()
		{
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentA>().Named("a"));
			Assert.IsInstanceOfType(typeof(ComponentA), container.Resolve<IComponentA>());

			container.RemoveComponent("a");
			container.Register(Component.For<IComponentA>().ImplementedBy<ComponentAA>());
			Assert.IsInstanceOfType(typeof(ComponentAA), container.Resolve<IComponentA>());
		}

        [Test]
        public void DisposingContainerAlsoDisposedComponentsInContainer()
        {
			container.Register(Component.For<DisposableComponent>());
            var disposable = container.Resolve<DisposableComponent>();

            Assert.AreEqual(0, disposable.DisposedCalledCount);

            container.Dispose();

            Assert.AreEqual(1, disposable.DisposedCalledCount);
        }

		[Test]
		[ExpectedException(typeof(CompactContainerException))]
		public void ThrowsWhenTryingToRemoveNotExistentComponent()
		{
			container.RemoveComponent("a");
		}


		private static int getSingletonsCount(IEnumerable<ComponentInfo> components)
		{
			return components.Count(c => c.Lifestyle.Equals(LifestyleType.Singleton));
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