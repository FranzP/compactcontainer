using System.Collections.Generic;
using System.Linq;

namespace CompactContainer
{
	public static class StringExtensions
	{
		public static string ToCommaSeparatedString(this IEnumerable<string> strings)
		{
			return strings.Aggregate((a, b) => a + ", " + b);
		}
	}
}