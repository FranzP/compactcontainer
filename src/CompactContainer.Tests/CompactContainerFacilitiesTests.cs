using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class CompactContainerFacilitiesTests
	{
		[Test]
		public void Should_initialize_facility_when_added_to_the_container()
		{
			var container = new CompactContainer();

			var facility = new TestFacility();
			container.AddFacility(facility);

			facility.Initialized.Should().Be.True();
		}

		[Test]
		public void Should_terminate_added_facilities_when_disposing_the_container()
		{
			var facility = new TestFacility();

			using( var container = new CompactContainer())
			{
				container.AddFacility(facility);
			}

			facility.Terminated.Should().Be.True();
		}

		public class TestFacility : IFacility
		{
			public bool Initialized { get; private set; }

			public bool Terminated { get; private set; }

			public void Init(ICompactContainer container)
			{
				Initialized = true;
			}

			public void Terminate()
			{
				Terminated = true;
			}
		}
	}
}