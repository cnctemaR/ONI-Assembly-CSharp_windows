using System;

namespace UnityEngine.Timeline
{
	internal class TimeFieldAttribute : PropertyAttribute
	{
		public TimeFieldAttribute.UseEditMode useEditMode { get; }

		public TimeFieldAttribute(TimeFieldAttribute.UseEditMode useEditMode = TimeFieldAttribute.UseEditMode.ApplyEditMode)
		{
			this.useEditMode = useEditMode;
		}

		public enum UseEditMode
		{
			None,
			ApplyEditMode
		}
	}
}
