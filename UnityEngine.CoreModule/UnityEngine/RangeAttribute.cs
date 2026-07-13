using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class RangeAttribute : PropertyAttribute
	{
		public RangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public readonly float min;

		public readonly float max;
	}
}
