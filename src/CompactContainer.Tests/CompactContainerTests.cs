using System;
using NUnit.Framework;

namespace InversionOfControl.Tests
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
			// turn off autoresolution
		    container.ShouldAutoRegister = false;
            
            Assert.IsFalse(container.HasComponent("comp.a"));
			Assert.IsFalse(container.HasComponent(typeof(IComponentA))); 

			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));

			Assert.IsTrue(container.HasComponent(typeof(IComponentA)));
			Assert.IsTrue(container.HasComponent("comp.a"));
		}

		[Test]
		[ExpectedException(typeof(CompactContainerException))]
		public void CannotAddDuplicatedComponent()
		{
			container.AddComponent("comp", typeof(ComponentA));
			container.AddComponent("comp", typeof(ComponentB));
		}

		[Test]
		public void CanResolveSingletonComponent()
		{
			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));

			IComponentA compA1 = (IComponentA)container.Resolve(typeof(IComponentA));
			IComponentA compA2 = (IComponentA)container.Resolve("comp.a");

			Assert.IsNotNull(compA1);
			Assert.AreSame(compA1, compA2);
		}

		[Test]
		public void CanResolveSingletonComponentUsingGenerics()
		{
			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));

			IComponentA compA1 = container.Resolve<IComponentA>();
			IComponentA compA2 = container.Resolve<IComponentA>("comp.a");

			Assert.IsNotNull(compA1);
			Assert.AreSame(compA1, compA2);
		}

		[Test]
		public void CanResolveSingletonComponentWithConstructorInjection()
		{
			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));
			container.AddComponent("comp.b", typeof(IComponentB), typeof(ComponentB));

			IComponentB compB = (IComponentB)container.Resolve(typeof(IComponentB));

			Assert.AreSame(container.Resolve(typeof(IComponentA)), compB.CompA);
		}

		[Test]
		public void CanResolveTransientComponent()
		{
			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA), LifestyleType.Transient);

			IComponentA compA1 = container.Resolve<IComponentA>();
			IComponentA compA2 = container.Resolve<IComponentA>();

			Assert.AreNotSame(compA1, compA2);
		}

		[Test]
		public void CanRegisterAndResolveComponentWithoutService()
		{
			container.AddComponent("comp.a", typeof(ComponentA));

			ComponentA compA1 = container.Resolve<ComponentA>();
			ComponentA compA2 = container.Resolve<ComponentA>();
			Assert.AreSame(compA2, compA1);
		}

		[Test]
		public void CanRegisterAndResolveComponentWithMultipleConstructors()
		{
            // turn off autoresolution
		    container.ShouldAutoRegister = false;

			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));
			container.AddComponent("comp.c1", typeof(IComponentC), typeof(ComponentC));
			container.AddComponent("comp.c2", typeof(IComponentC), typeof(ComponentC));

			IComponentC compC1 = container.Resolve<IComponentC>("comp.c1");
			Assert.AreEqual(1, compC1.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compC1.CompA);
			Assert.AreSame(null, compC1.CompB);

			container.AddComponent("comp.b", typeof(IComponentB), typeof(ComponentB));

			IComponentC compC2 = container.Resolve<IComponentC>("comp.c2");
			Assert.AreEqual(2, compC2.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compC2.CompA);
			Assert.AreSame(container.Resolve<IComponentB>(), compC2.CompB);

			Assert.AreNotSame(compC1, compC2);
		}

		[Test]
		public void CanRegisterAndResolveComponentWithMultipleConstructorsUsingCustomAttribute()
		{
			container.DefaultHandler = new AttributedHandler(container);
			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));
			container.AddComponent("comp.d1", typeof(IComponentD), typeof(ComponentD));
			container.AddComponent("comp.d2", typeof(IComponentD), typeof(ComponentD));

			IComponentD compD1 = container.Resolve<IComponentD>("comp.d1");
			Assert.AreEqual(1, compD1.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compD1.CompA);
			Assert.AreSame(null, compD1.CompB);

			container.AddComponent("comp.b", typeof(IComponentB), typeof(ComponentB));

			IComponentD compD2 = container.Resolve<IComponentD>("comp.d2");
			Assert.AreEqual(1, compD2.ConstructorUsed);
			Assert.AreSame(container.Resolve<IComponentA>(), compD2.CompA);
			Assert.AreSame(null, compD2.CompB);

			Assert.AreNotSame(compD1, compD2);
		}

		[Test]
		public void CanAddComponentWithInstance()
		{
			IComponentA compA1 = new ComponentA();

			container.AddComponentInstance("comp.a1", compA1);
			Assert.AreSame(compA1, container.Resolve<ComponentA>());

			container.AddComponentInstance("comp.a2", compA1);
			Assert.AreSame(compA1, container.Resolve<IComponentA>("comp.a2"));

			container.AddComponentInstance("comp.a3", typeof(IComponentA), compA1);
			Assert.AreSame(compA1, container.Resolve<IComponentA>());
		}

		[Test]
		public void RegisterSelf()
		{
			Assert.AreSame(container, container.Resolve<ICompactContainer>());
		}

		[Test]
		public void CanGetAllComponentsThatImplementService()
		{
			container.AddComponent("comp.a1", typeof(IComponentA), typeof(ComponentA));
			container.AddComponent("comp.a2", typeof(IComponentA), typeof(ComponentAA));
			container.AddComponent("comp.b", typeof(IComponentB), typeof(ComponentB));

			object[] compsA1 = container.GetServices(typeof(IComponentA));
			Assert.AreEqual(2, compsA1.Length);

			IComponentA[] compsA2 = container.GetServices<IComponentA>();
			Assert.AreEqual(2, compsA1.Length);

			Assert.AreEqual((IComponentA)compsA1[0], compsA2[0]);
			Assert.AreEqual((IComponentA)compsA1[1], compsA2[1]);
		}

		[Test]
		[ExpectedException(typeof(CompactContainerException))]
		public void DetectCircularReferences()
		{
			container.AddComponent("x", typeof(IDependentX), typeof(DependentX));
			container.AddComponent("y", typeof(IDependentY), typeof(DependentY));

			container.Resolve<IDependentY>();
		}

		[Test]
		public void CanResolveToClassTypeIfNotSuchService()
		{
			container.AddComponent("comp.a", typeof(IComponentA), typeof(ComponentA));

			ComponentA compa = container.Resolve<ComponentA>();

			Assert.IsNotNull(compa);
		}

		[Test]
		public void CountSingletonsInitialized()
		{
			container.AddComponent("comp.a1", typeof(IComponentA), typeof(ComponentA), LifestyleType.Transient);
			container.AddComponent("comp.b", typeof(IComponentB), typeof(ComponentB));
			Assert.AreEqual(container.SingletonsInstanciatedCount, 1);		// cuenta al contenedor mismo

			container.AddComponentInstance("comp.a2", new ComponentA());
			Assert.AreEqual(container.SingletonsInstanciatedCount, 2);

			container.Resolve<IComponentA>();
			Assert.AreEqual(container.SingletonsInstanciatedCount, 2);

			container.Resolve<IComponentB>();
			Assert.AreEqual(container.SingletonsInstanciatedCount, 3);
		}

		[Test]
		public void CanResolveWithHandler()
		{
			container.RegisterHandler<IStartable>(new StartableHandler());
			container.AddComponent("startable.service", typeof(IComponentU), typeof(StartableComponent));

			IComponentU cu = container.Resolve<IComponentU>();

			Assert.AreEqual(1, cu.A);
		}

		[Test]
		public void ResolutionOfClassCanUseDefaultLifestyleTypeTransient()
		{
			container.DefaultLifestyle = LifestyleType.Transient;
			container.AddComponent(typeof(ComponentA));
			var component1 = container.Resolve<ComponentA>();
			var component2 = container.Resolve<ComponentA>();
			Assert.AreNotSame(component1, component2);
		}

		[Test]
		public void ResolutionOfClassCanUseDefaultLifestyleTypeSingleton()
		{
			container.DefaultLifestyle = LifestyleType.Singleton;
			container.AddComponent(typeof(ComponentA));
			var component1 = container.Resolve<ComponentA>();
			var component2 = container.Resolve<ComponentA>();
			Assert.AreSame(component1, component2);
		}

		[Test]
		public void AutoregisterConcreteTypesWhenResolving()
		{
			var a = container.Resolve<ComponentA>();
			Assert.IsNotNull(a);
		}

        [Test]
        public void AutoregisterInterfaceTypeUsingDefaultNamingConvention()
        {
            // new feature - auto register an interface type when a concrete component can be resolved with matching name
            var a = container.Resolve<IComponentA>();
            Assert.IsInstanceOfType(typeof(ComponentA), a);
        }

		[Test]
		public void CanRemoveRegisteredComponent()
		{
			container.AddComponent("a", typeof(IComponentA), typeof(ComponentA));
			Assert.IsInstanceOfType(typeof(ComponentA), container.Resolve<IComponentA>());

			container.RemoveComponent("a");
			container.AddComponent("a", typeof(IComponentA), typeof(ComponentAA));
			Assert.IsInstanceOfType(typeof(ComponentAA), container.Resolve<IComponentA>());
		}

		[Test]
		[ExpectedException(typeof(CompactContainerException))]
		public void ThrowsWhenTryingToRemoveNotExistentComponent()
		{
			container.RemoveComponent("a");
		}
	}

	public class StartableHandler : AbstractHandler
	{
		public override object Create(Type classType)
		{
			IStartable product = (IStartable)Container.DefaultHandler.Create(classType);
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
}