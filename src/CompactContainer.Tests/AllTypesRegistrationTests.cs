using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class AllTypesRegistrationTests
	{
		private CompactContainer container;

		[SetUp]
		public void SetUp()
		{
			container = new CompactContainer();
		}

		[Test]
		public void AllTypes_registration_should_register_BasedOn_asignable_types()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>().WithService(x => x)
				);

			container.Components.Satisfy(components => components.Any(c => c.Classtype == typeof(FirstView)));
		}

		[Test]
		public void AllTypes_registration_should_register_types_that_satisfy_Where_condition()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.Where(t => t.Name.EndsWith("View")).WithService(x => x)
				);

			container.Components.Satisfy(components => components.Any(c => c.Classtype == typeof(FirstView)));
		}

		[Test]
		public void AllTypes_registration_should_allow_configuration_per_component()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>().WithService(x => x).Configure(r => r.WithLifestyle(LifestyleType.Transient))
				);

			var c1 = container.Components.FindServiceType(typeof (FirstView));
			c1.Lifestyle.Should().Be.EqualTo(LifestyleType.Transient);
		}

		[Test]
		public void AllTypes_registration_should_allow_to_name_each_component()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>().WithService(x => x).Configure(r => r.Named(r.ImplementationType.Name + "X"))
				);

			var c1 = container.Components.FindServiceType(typeof(FirstView));
			c1.Key.Should().Be.EqualTo("FirstViewX");
		}

		[Test]
		public void AllTypes_Where_registration_should_use_implementation_type_as_service()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.Where(t => typeof(IView).IsAssignableFrom(t))
				);

			container.HasComponent(typeof(FirstView)).Should().Be.True();
			container.HasComponent(typeof(SecondView)).Should().Be.True();
			container.HasComponent(typeof(IView)).Should().Be.False();
		}

		[Test]
		public void AllTypes_BasedOn_registration_should_use_based_on_type_as_service()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>()
				);

			container.Components.Count(c => c.ServiceTypes.Contains(typeof(IView))).Should().Be.EqualTo(2);
		}

		[Test]
		public void AllTypes_registration_should_allow_specification_of_registered_service()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>().WithService(x => typeof(int))
				);

			container.Components.Count(c => c.ServiceTypes.Contains(typeof(int))).Should().Be.EqualTo(2);
		}


		public interface IView { }

		public class FirstView : IView { }

		public class SecondView : IView { }

		public interface IPresenter { }

		public class FirstPresenter : IPresenter { }

		public class SecondPresenter : IPresenter { }
	}
}