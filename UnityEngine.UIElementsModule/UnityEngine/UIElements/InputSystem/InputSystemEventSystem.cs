using System;

namespace UnityEngine.UIElements.InputSystem
{
	[AddComponentMenu("UI Toolkit/Input System Event System (UI Toolkit)")]
	public class InputSystemEventSystem : MonoBehaviour
	{
		public bool isAppFocused { get; private set; } = true;

		[Obsolete("EventSystem no longer supports input override for legacy input. Install Input System package for full input binding functionality.")]
		public InputWrapper inputOverride { get; set; }

		protected InputSystemEventSystem()
		{
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			this.isAppFocused = hasFocus;
		}
	}
}
