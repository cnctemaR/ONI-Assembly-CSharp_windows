using System;
using System.Collections.Generic;

public class GameInputManager : KInputManager
{
	public KInputController AddKeyboardMouseController()
	{
		KInputController kinputController = new KInputController(false);
		foreach (BindingEntry bindingEntry in GameInputMapping.GetBindingEntries())
		{
			kinputController.Bind(bindingEntry.mKeyCode, bindingEntry.mModifier, bindingEntry.mAction);
		}
		base.AddController(kinputController);
		return kinputController;
	}

	public KInputController AddGamepadController(int gamepad_index)
	{
		KInputController kinputController = new KInputController(true);
		foreach (BindingEntry bindingEntry in GameInputMapping.GetBindingEntries())
		{
			kinputController.Bind(bindingEntry.mKeyCode, Modifier.None, bindingEntry.mAction);
		}
		base.AddController(kinputController);
		return kinputController;
	}

	public GameInputManager(BindingEntry[] default_keybindings)
	{
		GameInputMapping.SetDefaultKeyBindings(default_keybindings);
		GameInputMapping.LoadBindings();
		this.AddKeyboardMouseController();
		KInputManager.steamInputInterpreter.OnEnable();
		if (KInputManager.steamInputInterpreter.NumOfISteamInputs > 0)
		{
			this.AddGamepadController(base.GetControllerCount());
		}
	}

	public void RebindControls()
	{
		foreach (KInputController kinputController in this.mControllers)
		{
			kinputController.ClearBindings();
			foreach (BindingEntry bindingEntry in GameInputMapping.GetBindingEntries())
			{
				kinputController.Bind(bindingEntry.mKeyCode, bindingEntry.mModifier, bindingEntry.mAction);
			}
			kinputController.HandleCancelInput();
		}
		KInputManager.InputChange.Invoke();
	}

	public override void Update()
	{
		if (KInputManager.isFocused)
		{
			KInputManager.steamInputInterpreter.Update();
			if (KInputManager.steamInputInterpreter.NumOfISteamInputs > 0 && base.GetControllerCount() <= 1)
			{
				this.AddGamepadController(base.GetControllerCount());
			}
			else if (KInputManager.steamInputInterpreter.NumOfISteamInputs < 1 && KInputManager.currentControllerIsGamepad)
			{
				KInputManager.currentControllerIsGamepad = false;
				KInputManager.InputChange.Invoke();
			}
			int num = 0;
			while (num < this.mControllers.Count && num + 1 < this.mControllers.Count)
			{
				if (this.mControllers[num].inputHandler.HandleChildCount() != this.mControllers[num + 1].inputHandler.HandleChildCount())
				{
					this.mControllers[num].inputHandler.TransferHandles(this.mControllers[num + 1].inputHandler);
				}
				num++;
			}
			base.Update();
		}
	}

	public override void OnApplicationFocus(bool focusStatus)
	{
		base.OnApplicationFocus(focusStatus);
	}

	public List<IInputHandler> usedMenus = new List<IInputHandler>();
}
