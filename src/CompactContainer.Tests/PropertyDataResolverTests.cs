using InversionOfControl;
using MbUnit.Framework;
using Rhino.Mocks;

namespace InversionOfControl.Tests
{
	[TestFixture]
	public class PropertyDataResolverTests
	{
		private MockRepository mocks;

		[SetUp]
		public void Init()
		{
			mocks = new MockRepository();
		}

		[Test]
		public void GetPropertyData()
		{
			TestObject test1 = new TestObject();
			test1.Desc = "xxx"; 
			
			ICompactContainer container = mocks.CreateMock<ICompactContainer>();
			Expect.Call(container.Resolve(typeof(ITestObject))).Return(test1);

			mocks.ReplayAll();

			PropertyDataResolver resolver = new PropertyDataResolver(container);
			PropertyData data = resolver.Get("InversionOfControl.Tests.PropertyDataResolverTests+ITestObject, CompactContainer.Tests, Desc");

			mocks.VerifyAll();

			Assert.AreEqual(test1, data.Owner);
			Assert.AreEqual("Desc", data.Name);
			Assert.AreEqual("xxx", data.Value);
			Assert.AreEqual(typeof(ITestObject).GetProperty("Desc"), data.Info);
		}


		public interface ITestObject
		{
			string Desc { get; set; }
		}

		public class TestObject : ITestObject
		{
			private string desc;

			public string Desc
			{
				get { return desc; }
				set { desc = value; }
			}
		}

	}
}