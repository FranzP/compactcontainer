namespace CompactContainer.Tests
{
	public class ComponentA : IComponentA
	{
	}

	public class ComponentAA : IComponentA
	{
	}

	public class ComponentB : IComponentB
	{
		private readonly IComponentA compA;

		public IComponentA CompA
		{
			get { return compA; }
		}

		public ComponentB(IComponentA compA)
		{
			this.compA = compA;
		}
	}

	public class ComponentC : IComponentC
	{
		private readonly IComponentA compA;
		private readonly IComponentB compB;
		private readonly int constructorUsed = 0;

		public IComponentA CompA
		{
			get { return compA; }
		}

		public IComponentB CompB
		{
			get { return compB; }
		}

		public int ConstructorUsed
		{
			get { return constructorUsed; }
		}

		public ComponentC(IComponentA compA)
		{
			this.compA = compA;
			constructorUsed = 1;
		}

		public ComponentC(IComponentA compA, IComponentB compB)
		{
			this.compA = compA;
			this.compB = compB;
			constructorUsed = 2;
		}
	}

	public class ComponentD : IComponentD
	{
		private readonly IComponentA compA;
		private readonly IComponentB compB;
		private readonly int constructorUsed = 0;

		public IComponentA CompA
		{
			get { return compA; }
		}

		public IComponentB CompB
		{
			get { return compB; }
		}

		public int ConstructorUsed
		{
			get { return constructorUsed; }
		}

		[Inject]
		public ComponentD(IComponentA compA)
		{
			this.compA = compA;
			constructorUsed = 1;
		}

		public ComponentD(IComponentA compA, IComponentB compB)
		{
			this.compA = compA;
			this.compB = compB;
			constructorUsed = 2;
		}
	}

	public interface IComponentD
	{
		IComponentA CompA { get; }
		IComponentB CompB { get; }
		int ConstructorUsed { get; }
	}

	public interface IComponentC
	{
		IComponentA CompA { get; }
		IComponentB CompB { get; }
		int ConstructorUsed { get; }
	}

	public interface IComponentA
	{
	}

	public interface IComponentB
	{
		IComponentA CompA { get; }
	}

	public interface IDependentX
	{
	}

	public class DependentX : IDependentX
	{
		private readonly IDependentY y;

		public DependentX(IDependentY y)
		{
			this.y = y;
		}
	}

	public interface IDependentY
	{
	}

	public class DependentY : IDependentY
	{
		private readonly IDependentX x;

		public DependentY(IDependentX x)
		{
			this.x = x;
		}
	}
}