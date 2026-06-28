using System;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class OverlayGUI
	{
		public static bool HasPopupControl()
		{
			return OverlayGUI.currentPopup != null;
		}

		public static void StartOverlayGUI()
		{
			if (OverlayGUI.currentPopup != null && Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
			{
				OverlayGUI.currentPopup.Draw();
			}
		}

		public static void EndOverlayGUI()
		{
			if (OverlayGUI.currentPopup != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
			{
				OverlayGUI.currentPopup.Draw();
			}
		}

		public static PopupMenu currentPopup;
	}
}
