using System;
using UnityEngine;

internal class InputInit : MonoBehaviour
{
	private void Awake()
	{
		GameInputManager inputManager = Global.Instance.GetInputManager();
		for (int i = 0; i < inputManager.GetControllerCount(); i++)
		{
			KInputController controller = inputManager.GetController(i);
			if (controller.IsGamepad)
			{
				KInputHandler.Add(controller, base.gameObject);
			}
		}
		KInputHandler.Add(inputManager.GetDefaultController(), KScreenManager.Instance, 10);
		DebugHandler debugHandler = new DebugHandler();
		KInputHandler.Add(inputManager.GetDefaultController(), debugHandler, -1);
	}
}
