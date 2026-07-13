using System;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeData.h")]
	[Flags]
	public enum AccessibilityState : byte
	{
		None = 0,
		Disabled = 1,
		Selected = 2,
		Expanded = 4
	}
}
