using System;
using UnityEngine;

public class TelepadSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.viewImmigrantsBtn.onClick += delegate
		{
			ImmigrantScreen.InitializeImmigrantScreen(this.targetTelepad);
			PlanScreen.Instance.ExternalClose();
			SelectTool.Instance.Select(null, true);
		};
	}

	public override void SetTarget(GameObject target)
	{
		Telepad component = target.GetComponent<Telepad>();
		if (component == null)
		{
			Debug.LogError("Target doesn't have a telepad associated with it.");
			return;
		}
		this.targetTelepad = component;
	}

	private void Update()
	{
		if (this.targetTelepad != null)
		{
			if (GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver())
			{
				this.timeLabel.text = Strings.Get("STRINGS.UI.UISIDESCREENS.TELEPADSIDESCREEN.GAMEOVER");
				this.SetContentState(true);
			}
			else
			{
				this.timeLabel.text = string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.TELEPADSIDESCREEN.NEXTPRODUCTION"), GameUtil.GetFormattedCycles(this.targetTelepad.GetTimeRemaining()));
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
