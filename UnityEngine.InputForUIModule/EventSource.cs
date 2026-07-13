using System;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal enum EventSource
	{
		Unspecified,
		Keyboard,
		Gamepad,
		Mouse,
		Pen,
		Touch,
		TrackedDevice
	}
}
