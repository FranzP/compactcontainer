﻿using System.ComponentModel;

namespace CompactContainer.Registrations
{
	public class ParameterPart : IComponentRegistrationPart
	{
		private readonly string key;
		private object value;

		public ParameterPart(string key)
		{
			this.key = key;
		}

		public ParameterPart EqualsTo(object value)
		{
			this.value = value;
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public void ApplyTo(ComponentInfo componentInfo)
		{
			componentInfo.Parameters.Add(key, value);
		}
	}
}