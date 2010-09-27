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
		public void Can_register_several_types_using_AllTypes_registration()
		{
			container.Register(
				AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
					.BasedOn<IView>().WithService(SelectService.FirstInterface)
				);

			var firstComponentInfo = container.Components.FindServiceType(typeof (IView));
			firstComponentInfo.Classtype.Should().Be.EqualTo(typeof(FirstView));
		}

		public interface IView { }

		public class FirstView : IView { }

		public class SecondView : IView { }

		public interface IPresenter { }

		public class FirstPresenter : IPresenter { }

		public class SecondPresenter : IPresenter { }
	}
}