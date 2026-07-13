using System;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeData.h")]
	public enum AccessibilityScrollDirection : byte
	{
		Unknown,
		Forward,
		Backward,
		Left,
		Right,
		Up,
		Down
	}
}
