using System;
using STRINGS;
using UnityEngine;

public class TopLeftControlScreen : KScreen
{
	public static void DestroyInstance()
	{
		TopLeftControlScreen.Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		TopLeftControlScreen.Instance = this;
		this.RefreshName();
		this.UpdateSandboxToggleState();
		MultiToggle sandboxToggle = this.SandboxToggle;
		sandboxToggle.onClick = (global::System.Action)Delegate.Combine(sandboxToggle.onClick, new global::System.Action(this.OnClickSandboxToggle));
		Game.Instance.Subscribe(-1948169901, delegate(object data)
		{
			this.UpdateSandboxToggleState();
		});
	}

	public void RefreshName()
	{
		if (SaveGame.Instance != null)
		{
			this.locText.text = SaveGame.Instance.BaseName;
		}
	}

	public void UpdateSandboxToggleState()
	{
		if (this.CheckSandboxModeLocked())
		{
			this.SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED + GameUtil.GetHotkeyString(global::Action.ToggleSandboxTools));
			this.SandboxToggle.ChangeState(0);
		}
		else
		{
			this.SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED + GameUtil.GetHotkeyString(global::Action.ToggleSandboxTools));
			this.SandboxToggle.ChangeState((!Game.Instance.SandboxModeActive) ? 1 : 2);
		}
		this.SandboxToggle.gameObject.SetActive(SaveGame.Instance.sandboxEnabled);
	}

	private void OnClickSandboxToggle()
	{
		if (this.CheckSandboxModeLocked())
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
		}
		this.UpdateSandboxToggleState();
	}

	private bool CheckSandboxModeLocked()
	{
		return !SaveGame.Instance.sandboxEnabled;
	}

	public static TopLeftControlScreen Instance;

	[SerializeField]
	private MultiToggle SandboxToggle;

	[SerializeField]
	private LocText locText;
}
