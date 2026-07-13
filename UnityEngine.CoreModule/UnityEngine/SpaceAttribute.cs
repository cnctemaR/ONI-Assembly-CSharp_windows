using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class SpaceAttribute : PropertyAttribute
	{
		public SpaceAttribute()
			: base(true)
		{
			this.height = 8f;
		}

		public SpaceAttribute(float height)
			: base(true)
		{
			this.height = height;
		}

		public readonly float height;
	}
}
