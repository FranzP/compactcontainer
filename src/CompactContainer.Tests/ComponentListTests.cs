using NUnit.Framework;

namespace CompactContainer.Tests
{
	[TestFixture]
	public class ComponentListTests
	{
		private ComponentList list;
		private ComponentInfo[] ci;

		[SetUp]
		public void Init()
		{
			list = new ComponentList();
			ci = new ComponentInfo[3];
			ci[0] = new ComponentInfo("key1", GetType(), GetType(), LifestyleType.Singleton);
			ci[1] = new ComponentInfo("key2", GetType(), GetType(), LifestyleType.Transient);
			ci[2] = new ComponentInfo("key3", GetType(), GetType(), LifestyleType.Singleton);
		}

		[Test]
		public void CanAdd()
		{
			list.Add(ci[0]);
			Assert.AreEqual(ci[0], list[0]);
		}		
		
		[Test]
		public void CanRemove()
		{
			list.Add(ci[0]);
			list.Add(ci[1]);
			Assert.AreEqual(ci[0], list[0]);

			list.Remove(ci[0]);
			Assert.AreEqual(ci[1], list[0]);
		}

		[Test]
		public void CountSingletons()
		{
			list.Add(ci[0]);
			list.Add(ci[1]);
			Assert.AreEqual(1, list.SingletonsCount);

			list.Add(ci[2]);
			Assert.AreEqual(2, list.SingletonsCount);

			list.Remove(ci[0]);
			Assert.AreEqual(1, list.SingletonsCount);
		}

		[Test]
		public void CanEnumerate()
		{
			list.Add(ci[0]);
			list.Add(ci[1]);
			list.Add(ci[2]);

			int a = 0;
			foreach (ComponentInfo info in list) {
				Assert.AreEqual(ci[a], info);
				a++;
			}
		}
		
	}
}