using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	internal static class MultipleDisplayUtilities
	{
		public static bool GetRelativeMousePositionForDrag(PointerEventData eventData, ref Vector2 position)
		{
			int displayIndex = eventData.pointerPressRaycast.displayIndex;
			Vector3 vector = Display.RelativeMouseAt(eventData.position);
			int num = (int)vector.z;
			bool flag;
			if (num != displayIndex)
			{
				flag = false;
			}
			else
			{
				position = ((displayIndex == 0) ? eventData.position : vector);
				flag = true;
			}
			return flag;
		}

		public static Vector2 GetMousePositionRelativeToMainDisplayResolution()
		{
			Vector3 mousePosition = Input.mousePosition;
			if (Display.main.renderingHeight != Display.main.systemHeight)
			{
				if (mousePosition.y < 0f || mousePosition.y > (float)Display.main.renderingHeight || mousePosition.x < 0f || mousePosition.x > (float)Display.main.renderingWidth)
				{
					mousePosition.y += (float)(Display.main.systemHeight - Display.main.renderingHeight);
				}
			}
			return mousePosition;
		}
	}
}
