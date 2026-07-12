using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

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
		KInputManager.InputChange.AddListener(new UnityAction(this.ResetToolTip));
		this.UpdateSandboxToggleState();
		MultiToggle sandboxToggle = this.SandboxToggle;
		sandboxToggle.onClick = (global::System.Action)Delegate.Combine(sandboxToggle.onClick, new global::System.Action(this.OnClickSandboxToggle));
		Game.Instance.Subscribe(-1948169901, delegate(object data)
		{
			this.UpdateSandboxToggleState();
		});
	}

	protected override void OnForcedCleanUp()
	{
		KInputManager.InputChange.RemoveListener(new UnityAction(this.ResetToolTip));
		base.OnForcedCleanUp();
	}

	public void RefreshName()
	{
		if (SaveGame.Instance != null)
		{
			this.locText.text = SaveGame.Instance.BaseName;
		}
	}

	public void ResetToolTip()
	{
		if (this.CheckSandboxModeLocked())
		{
			this.SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED, global::Action.ToggleSandboxTools));
			return;
		}
		this.SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED, global::Action.ToggleSandboxTools));
	}

	public void UpdateSandboxToggleState()
	{
		if (this.CheckSandboxModeLocked())
		{
			this.SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED, global::Action.ToggleSandboxTools));
			this.SandboxToggle.ChangeState(0);
		}
		else
		{
			this.SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED, global::Action.ToggleSandboxTools));
			this.SandboxToggle.ChangeState(Game.Instance.SandboxModeActive ? 2 : 1);
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
