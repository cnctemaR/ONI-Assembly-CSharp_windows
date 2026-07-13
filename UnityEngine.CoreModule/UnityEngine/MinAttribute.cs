using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class MinAttribute : PropertyAttribute
	{
		public MinAttribute(float min)
		{
			this.min = min;
		}

		public readonly float min;
	}
}
