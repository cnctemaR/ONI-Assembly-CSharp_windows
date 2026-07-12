using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	public class TooltipAttribute : PropertyAttribute
	{
		public TooltipAttribute(string tooltip)
		{
			this.tooltip = tooltip;
		}

		public readonly string tooltip;
	}
}
