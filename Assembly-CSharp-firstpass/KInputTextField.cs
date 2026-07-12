using System;
using TMPro;

public class KInputTextField : TMP_InputField
{
	private KInputTextField()
	{
		this.onFocus = (global::System.Action)Delegate.Combine(this.onFocus, new global::System.Action(delegate
		{
			if (SteamGamepadTextInput.IsActive())
			{
				SteamGamepadTextInput.ShowTextInputScreen("", base.text, new Action<SteamGamepadTextInputData>(this.OnGamepadInputDismissed));
			}
		}));
	}

	private void OnGamepadInputDismissed(SteamGamepadTextInputData data)
	{
		if (data.submitted)
		{
			base.text = data.input;
		}
		base.OnDeselect(null);
	}
}
