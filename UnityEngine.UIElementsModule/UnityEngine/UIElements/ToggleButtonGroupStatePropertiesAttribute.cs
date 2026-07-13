using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class ToggleButtonGroupStatePropertiesAttribute : PropertyAttribute
	{
		public bool allowMultipleSelection { get; }

		public bool allowEmptySelection { get; }

		public int length { get; }

		public ToggleButtonGroupStatePropertiesAttribute(bool allowMultipleSelection = true, bool allowEmptySelection = true, int length = -1)
		{
			this.allowMultipleSelection = allowMultipleSelection;
			this.allowEmptySelection = allowEmptySelection;
			this.length = length;
		}
	}
}
