using System;

namespace UnityEngine.UIElements
{
	public interface IDebugPanelChangeReceiver
	{
		void OnVisualElementChange(VisualElement element, VersionChangeType changeType);
	}
}
