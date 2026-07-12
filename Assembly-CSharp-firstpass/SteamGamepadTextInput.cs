using System;
using Steamworks;

public class SteamGamepadTextInput
{
	public static bool IsActive()
	{
		return KInputManager.steamInputInterpreter.Initialized && (KInputManager.currentControllerIsGamepad || SteamUtils.IsSteamRunningOnSteamDeck());
	}

	public static void ShowTextInputScreen(string desc, string init, Action<SteamGamepadTextInputData> action)
	{
		DebugUtil.DevAssert(!SteamGamepadTextInput.active, "Gamepad input already active.", null);
		if (SteamUtils.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, desc, 512U, init))
		{
			SteamGamepadTextInput.GamepadInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(new Callback<GamepadTextInputDismissed_t>.DispatchDelegate(SteamGamepadTextInput.OnGamepadInputDismissed));
			SteamGamepadTextInput.action = action;
			SteamGamepadTextInput.active = true;
		}
	}

	private static void OnGamepadInputDismissed(GamepadTextInputDismissed_t callback)
	{
		SteamGamepadTextInputData steamGamepadTextInputData = default(SteamGamepadTextInputData);
		steamGamepadTextInputData.submitted = false;
		steamGamepadTextInputData.input = "";
		if (callback.m_bSubmitted)
		{
			steamGamepadTextInputData.submitted = true;
			string text;
			if (SteamUtils.GetEnteredGamepadTextInput(out text, callback.m_unSubmittedText) && text != null)
			{
				steamGamepadTextInputData.input = text;
			}
		}
		SteamGamepadTextInput.GamepadInputDismissed.Dispose();
		SteamGamepadTextInput.active = false;
		SteamGamepadTextInput.action(steamGamepadTextInputData);
	}

	private static bool active;

	private static Action<SteamGamepadTextInputData> action;

	private static Callback<GamepadTextInputDismissed_t> GamepadInputDismissed;
}
