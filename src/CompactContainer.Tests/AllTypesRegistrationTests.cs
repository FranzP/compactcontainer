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
					.BasedOn<IView>().WithService(x => x).Configure(cfg => cfg.WithLifestyle(LifestyleType.Transient))
				);

			var c1 = container.Components.FindServiceType(typeof (FirstView));
			c1.Lifestyle.Should().Be.EqualTo(LifestyleType.Transient);
		}

		[Test]
		public void AllTypes_registration_should_allow_specification_of_registered_service()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>().WithService(x => typeof(IView))
				);

			container.Components.Count(c => c.ServiceTypes.Contains(typeof(IView))).Should().Be.Equals(2);
		}

		public interface IView { }

		public class FirstView : IView { }

		public class SecondView : IView { }

		public interface IPresenter { }

		public class FirstPresenter : IPresenter { }

		public class SecondPresenter : IPresenter { }
	}
}