using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class GradientUsageAttribute : PropertyAttribute
	{
		public GradientUsageAttribute(bool hdr)
		{
			this.hdr = hdr;
		}

		public readonly bool hdr = false;
	}
}
