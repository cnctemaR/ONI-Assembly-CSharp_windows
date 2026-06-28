using System;

public class GameInputManager : KInputManager
{
	public GameInputManager()
	{
		GameInputMapping.LoadBindings();
		this.AddKeyboardMouseController();
		for (int i = 0; i < 4; i++)
		{
			this.AddGamepadController(i);
		}
	}

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
			kinputController.Bind(BindingEntry.GetGamepadKeyCode(gamepad_index, bindingEntry.mButton), Modifier.None, bindingEntry.mAction);
		}
		base.AddController(kinputController);
		return kinputController;
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
	}

	public override void Update()
	{
		if (KInputManager.isFocused)
		{
			base.Update();
		}
	}

	public override void OnApplicationFocus(bool focusStatus)
	{
		base.OnApplicationFocus(focusStatus);
	}
}
