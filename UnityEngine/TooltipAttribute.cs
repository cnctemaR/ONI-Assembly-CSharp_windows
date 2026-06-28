using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class TooltipAttribute : PropertyAttribute
	{
		public TooltipAttribute(string tooltip)
		{
			this.tooltip = tooltip;
		}

		public readonly string tooltip;
	}
}
