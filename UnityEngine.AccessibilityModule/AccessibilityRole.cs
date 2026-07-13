using System;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeData.h")]
	public enum AccessibilityRole : byte
	{
		None,
		Button,
		Image,
		StaticText,
		SearchField,
		KeyboardKey,
		Header,
		TabBar,
		Slider,
		Toggle,
		Container,
		TextField,
		Dropdown,
		TabButton,
		ScrollView
	}
}
