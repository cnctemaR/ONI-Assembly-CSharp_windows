using System;
using UnityEngine;

public class ScreenResolutionMonitor : MonoBehaviour
{
	private void Awake()
	{
		this.previousSize = new Vector2((float)Screen.width, (float)Screen.height);
	}

	private void Update()
	{
		if ((this.previousSize.x != (float)Screen.width || this.previousSize.y != (float)Screen.height) && Game.Instance != null)
		{
			Game.Instance.Trigger(445618876, null);
			this.previousSize.x = (float)Screen.width;
			this.previousSize.y = (float)Screen.height;
		}
		this.UpdateShouldUseGamepadUIMode();
	}

	public static bool UsingGamepadUIMode()
	{
		return ScreenResolutionMonitor.previousGamepadUIMode;
	}

	private void UpdateShouldUseGamepadUIMode()
	{
		bool flag = (Screen.dpi > 130f && Screen.height < 900) || KInputManager.currentControllerIsGamepad;
		if (flag != ScreenResolutionMonitor.previousGamepadUIMode)
		{
			ScreenResolutionMonitor.previousGamepadUIMode = flag;
			if (Game.Instance == null)
			{
				return;
			}
			Game.Instance.Trigger(-442024484, null);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound(flag ? "ControllerType_ToggleOn" : "ControllerType_ToggleOff", false));
		}
	}

	[SerializeField]
	private Vector2 previousSize;

	private static bool previousGamepadUIMode;

	private const float HIGH_DPI = 130f;
}
