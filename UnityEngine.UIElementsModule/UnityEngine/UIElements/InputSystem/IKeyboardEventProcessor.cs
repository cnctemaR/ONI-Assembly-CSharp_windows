using System;

namespace UnityEngine.UIElements.InputSystem
{
	internal interface IKeyboardEventProcessor
	{
		void OnEnable();

		void OnDisable();

		void ProcessKeyboardEvents(InputSystemEventSystem eventSystem);
	}
}
