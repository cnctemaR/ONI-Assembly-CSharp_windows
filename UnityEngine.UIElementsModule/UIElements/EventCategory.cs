using System;

namespace UnityEngine.UIElements
{
	internal enum EventCategory
	{
		Default,
		Pointer,
		PointerMove,
		EnterLeave,
		EnterLeaveWindow,
		Keyboard,
		Geometry,
		Style,
		ChangeValue,
		Bind,
		Focus,
		ChangePanel,
		StyleTransition,
		Navigation,
		Command,
		Tooltip,
		IMGUI,
		Reserved = 31
	}
}
