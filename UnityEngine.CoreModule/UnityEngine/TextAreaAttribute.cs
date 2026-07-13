using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class TextAreaAttribute : PropertyAttribute
	{
		public TextAreaAttribute()
		{
			this.minLines = 3;
			this.maxLines = 3;
		}

		public TextAreaAttribute(int minLines, int maxLines)
		{
			this.minLines = minLines;
			this.maxLines = maxLines;
		}

		public readonly int minLines;

		public readonly int maxLines;
	}
}
