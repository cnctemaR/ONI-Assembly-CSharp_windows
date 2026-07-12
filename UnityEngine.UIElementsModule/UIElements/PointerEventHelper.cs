using System;

namespace UnityEngine.UIElements
{
	internal static class PointerEventHelper
	{
		public static EventBase GetPooled(EventType eventType, Vector3 mousePosition, Vector2 delta, int button, int clickCount, EventModifiers modifiers)
		{
			bool flag = eventType == EventType.MouseDown && !PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, button);
			EventBase eventBase;
			if (flag)
			{
				eventBase = PointerEventBase<PointerDownEvent>.GetPooled(eventType, mousePosition, delta, button, clickCount, modifiers);
			}
			else
			{
				bool flag2 = eventType == EventType.MouseUp && !PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, button);
				if (flag2)
				{
					eventBase = PointerEventBase<PointerUpEvent>.GetPooled(eventType, mousePosition, delta, button, clickCount, modifiers);
				}
				else
				{
					eventBase = PointerEventBase<PointerMoveEvent>.GetPooled(eventType, mousePosition, delta, button, clickCount, modifiers);
				}
			}
			return eventBase;
		}
	}
}
