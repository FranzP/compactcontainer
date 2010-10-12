using System.Linq;
using CompactContainer.DependencyResolvers;
using NUnit.Framework;
using SharpTestsEx;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class CompositeResolverTests
	{
		[Test]
		public void Can_add_custom_resolver_to_composite_resolver_by_subclassing()
		{
			var tr = new TestResolver();
			tr.AssertCustomResolverWasAdded();
		}

		class CustomResolver : SimpleDependencyResolver
		{
			public CustomResolver() : base(null)
			{
			}
		}

		class TestResolver : CompositeDependencyResolver
		{
			public TestResolver() 
				: base(null)
			{
				Resolvers.Insert(2, new CustomResolver());
			}

			public void AssertCustomResolverWasAdded()
			{
				Resolvers.ElementAt(2).Should().Be.OfType<CustomResolver>();
			}
		}
	}
}