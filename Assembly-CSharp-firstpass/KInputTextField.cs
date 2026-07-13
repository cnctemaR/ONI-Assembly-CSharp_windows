using System;
using TMPro;
using UnityEngine;

public class KInputTextField : TMP_InputField
{
	protected override void Awake()
	{
		base.Awake();
		base.onValueChanged.AddListener(delegate(string value)
		{
			if (value.IsNullOrWhiteSpace())
			{
				this.skipValueChangeRefreshDelay = true;
			}
			this.timeOfLastEditCompletion = Time.unscaledTime;
			this.waitingForValueChangeRefresh = true;
		});
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!this.waitingForValueChangeRefresh)
		{
			return;
		}
		if (this.skipValueChangeRefreshDelay || Time.unscaledTime - this.timeOfLastEditCompletion >= 0.2f)
		{
			this.waitingForValueChangeRefresh = false;
			this.skipValueChangeRefreshDelay = false;
			global::System.Action onValueChangesPaused = this.OnValueChangesPaused;
			if (onValueChangesPaused == null)
			{
				return;
			}
			onValueChangesPaused();
		}
	}

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

	public void ForceChangeValueRefresh()
	{
		this.skipValueChangeRefreshDelay = true;
	}

	private const float VALUE_CHANGE_REFRESH_DELAY = 0.2f;

	private bool waitingForValueChangeRefresh;

	private bool skipValueChangeRefreshDelay;

	private float timeOfLastEditCompletion = -1f;

	public global::System.Action OnValueChangesPaused;
}
