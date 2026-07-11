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
			if ((int)vector.z != displayIndex)
			{
				return false;
			}
			position = ((displayIndex != 0) ? vector : eventData.position);
			return true;
		}

		public static Vector2 GetMousePositionRelativeToMainDisplayResolution()
		{
			Vector3 mousePosition = Input.mousePosition;
			if (Display.main.renderingHeight != Display.main.systemHeight && (mousePosition.y < 0f || mousePosition.y > (float)Display.main.renderingHeight || mousePosition.x < 0f || mousePosition.x > (float)Display.main.renderingWidth) && Screen.fullScreenMode != FullScreenMode.Windowed)
			{
				mousePosition.y += (float)(Display.main.systemHeight - Display.main.renderingHeight);
			}
			return mousePosition;
		}
	}
}
