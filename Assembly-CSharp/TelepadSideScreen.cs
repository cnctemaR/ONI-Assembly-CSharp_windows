using System;
using STRINGS;
using UnityEngine;

public class TelepadSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.viewImmigrantsBtn.onClick += delegate
		{
			ImmigrantScreen.InitializeImmigrantScreen(this.targetTelepad);
			Game.Instance.Trigger(288942073, null);
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Telepad>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		Telepad component = target.GetComponent<Telepad>();
		if (component == null)
		{
			global::Debug.LogError("Target doesn't have a telepad associated with it.");
			return;
		}
		this.targetTelepad = component;
		if (this.targetTelepad != null)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (this.targetTelepad != null)
		{
			if (GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver())
			{
				base.gameObject.SetActive(false);
				this.timeLabel.text = UI.UISIDESCREENS.TELEPADSIDESCREEN.GAMEOVER;
				this.SetContentState(true);
			}
			else
			{
				if (this.targetTelepad.GetComponent<Operational>().IsOperational)
				{
					this.timeLabel.text = string.Format(UI.UISIDESCREENS.TELEPADSIDESCREEN.NEXTPRODUCTION, GameUtil.GetFormattedCycles(this.targetTelepad.GetTimeRemaining(), "F1"));
				}
				else
				{
					base.gameObject.SetActive(false);
				}
				this.SetContentState(!Immigration.Instance.ImmigrantsAvailable);
			}
		}
	}

	private void SetContentState(bool isLabel)
	{
		if (this.timeLabel.gameObject.activeInHierarchy != isLabel)
		{
			this.timeLabel.gameObject.SetActive(isLabel);
		}
		if (this.viewImmigrantsBtn.gameObject.activeInHierarchy == isLabel)
		{
			this.viewImmigrantsBtn.gameObject.SetActive(!isLabel);
		}
	}

	[SerializeField]
	private LocText timeLabel;

	[SerializeField]
	private KButton viewImmigrantsBtn;

	[SerializeField]
	private Telepad targetTelepad;
}
