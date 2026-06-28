using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class SpaceAttribute : PropertyAttribute
	{
		public SpaceAttribute()
		{
			this.height = 8f;
		}

		public SpaceAttribute(float height)
		{
			this.height = height;
		}

		public readonly float height;
	}
}
