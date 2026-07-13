using System;

namespace UnityEngine.UIElements
{
	internal class ScreenOverlayPanelPicker
	{
		public bool TryPick(BaseRuntimePanel panel, int pointerId, Vector2 screenPosition, Vector2 delta, int? targetDisplay, out bool captured)
		{
			bool flag;
			if (targetDisplay != null)
			{
				int? num = targetDisplay;
				int targetDisplay2 = panel.targetDisplay;
				flag = !((num.GetValueOrDefault() == targetDisplay2) & (num != null));
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool flag3;
			if (flag2)
			{
				captured = false;
				flag3 = false;
			}
			else
			{
				BaseVisualElementPanel baseVisualElementPanel;
				captured = this.GetCapturingPanel(pointerId, out baseVisualElementPanel);
				bool flag4 = captured;
				if (flag4)
				{
					bool flag5 = baseVisualElementPanel == panel;
					if (flag5)
					{
						return true;
					}
				}
				else
				{
					Vector3 vector;
					bool flag6 = panel.ScreenToPanel(screenPosition, delta, out vector, false);
					if (flag6)
					{
						VisualElement visualElement = panel.Pick(vector, pointerId);
						bool flag7 = visualElement != null;
						if (flag7)
						{
							return true;
						}
					}
				}
				flag3 = false;
			}
			return flag3;
		}

		private bool GetCapturingPanel(int pointerId, out BaseVisualElementPanel capturingPanel)
		{
			IEventHandler capturingElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId);
			VisualElement visualElement = capturingElement as VisualElement;
			bool flag = visualElement != null;
			if (flag)
			{
				capturingPanel = visualElement.elementPanel;
			}
			else
			{
				capturingPanel = PointerDeviceState.GetPlayerPanelWithSoftPointerCapture(pointerId);
			}
			return capturingPanel != null;
		}
	}
}
