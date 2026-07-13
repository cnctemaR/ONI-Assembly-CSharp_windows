using System;

namespace UnityEngine.UIElements
{
	internal static class KeyboardEventExtensions
	{
		internal static bool ShouldSendNavigationMoveEvent(this KeyDownEvent e)
		{
			return e.keyCode == KeyCode.Tab && !e.ctrlKey && !e.altKey && !e.commandKey && !e.functionKey;
		}

		internal static bool ShouldSendNavigationMoveEventRuntime(this Event e)
		{
			return e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab;
		}
	}
}
