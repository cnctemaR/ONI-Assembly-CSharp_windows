using System;

namespace UnityEngine.Timeline
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CustomStyleAttribute : Attribute
	{
		public CustomStyleAttribute(string ussStyle)
		{
			this.ussStyle = ussStyle;
		}

		public readonly string ussStyle;
	}
}
