﻿using System.Collections.Generic;
using System.Linq;

namespace CompactContainer
{
	public static class StringExtensions
	{
		public static string ToCommandSeparatedString(this IEnumerable<string> strings)
		{
			return strings.Aggregate((a, b) => a + ", " + b);
		}
	}
}