using System;

namespace UnityEngine.Timeline
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TrackBindingTypeAttribute : Attribute
	{
		public TrackBindingTypeAttribute(Type type)
		{
			this.type = type;
			this.flags = TrackBindingFlags.AllowCreateComponent;
		}

		public TrackBindingTypeAttribute(Type type, TrackBindingFlags flags)
		{
			this.type = type;
			this.flags = flags;
		}

		public readonly Type type;

		public readonly TrackBindingFlags flags;
	}
}
