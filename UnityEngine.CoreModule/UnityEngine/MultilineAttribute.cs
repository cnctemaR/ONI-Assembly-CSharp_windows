using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class MultilineAttribute : PropertyAttribute
	{
		public MultilineAttribute()
		{
			this.lines = 3;
		}

		public MultilineAttribute(int lines)
		{
			this.lines = lines;
		}

		public readonly int lines;
	}
}
