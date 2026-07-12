using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputModuleSwitch : MonoBehaviour
{
	private void Update()
	{
		if (this.lastMousePosition != Input.mousePosition && KInputManager.currentControllerIsGamepad)
		{
			KInputManager.currentControllerIsGamepad = false;
			KInputManager.InputChange.Invoke();
		}
		if (KInputManager.currentControllerIsGamepad)
		{
			this.virtualInput.enabled = KInputManager.currentControllerIsGamepad;
			if (this.standaloneInput.enabled)
			{
				this.standaloneInput.enabled = false;
				this.ChangeInputHandler();
				return;
			}
		}
		else
		{
			this.lastMousePosition = Input.mousePosition;
			this.standaloneInput.enabled = true;
			if (this.virtualInput.enabled)
			{
				this.virtualInput.enabled = false;
				this.ChangeInputHandler();
			}
		}
	}

	private void ChangeInputHandler()
	{
		for (int i = 0; i < Global.Instance.GetInputManager().usedMenus.Count; i++)
		{
			if (Global.Instance.GetInputManager().usedMenus[i].Equals(null))
			{
				Global.Instance.GetInputManager().usedMenus.RemoveAt(i);
			}
		}
		if (Global.Instance.GetInputManager().GetControllerCount() > 1)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				Cursor.visible = false;
				Global.Instance.GetInputManager().GetController(1).inputHandler.TransferHandles(Global.Instance.GetInputManager().GetController(0).inputHandler);
				return;
			}
			Cursor.visible = true;
			Global.Instance.GetInputManager().GetController(0).inputHandler.TransferHandles(Global.Instance.GetInputManager().GetController(1).inputHandler);
		}
	}

	public VirtualInputModule virtualInput;

	public StandaloneInputModule standaloneInput;

	private Vector3 lastMousePosition;
}
