using System;

namespace CompactContainer
{
	public class CompactContainerException : Exception
	{
		public CompactContainerException(string msg) : base(msg)
		{
		}

		public CompactContainerException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}
}