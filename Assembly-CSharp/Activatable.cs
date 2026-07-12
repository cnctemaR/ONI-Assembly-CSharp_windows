using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Activatable")]
public class Activatable : Workable, ISidescreenButtonControl
{
	public bool IsActivated
	{
		get
		{
			return this.activated;
		}
	}

	protected override void OnSpawn()
	{
		this.UpdateFlag();
		if (this.awaitingActivation && this.activateChore == null)
		{
			this.CreateChore();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.activated = true;
		this.awaitingActivation = false;
		this.UpdateFlag();
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCompleteWork(worker);
	}

	private void UpdateFlag()
	{
		base.GetComponent<Operational>().SetFlag(this.activatedFlag, this.activated);
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.DuplicantActivationRequired, !this.activated, null);
		base.Trigger(-1909216579, this.IsActivated);
	}

	private void CreateChore()
	{
		if (this.activateChore != null)
		{
			return;
		}
		Prioritizable.AddRef(base.gameObject);
		this.activateChore = new WorkChore<Activatable>(Db.Get().ChoreTypes.Toggle, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		if (!string.IsNullOrEmpty(this.requiredSkillPerk))
		{
			this.shouldShowSkillPerkStatusItem = true;
			this.requireMinionToWork = true;
			this.UpdateStatusItem(null);
		}
	}

	private void CancelChore()
	{
		if (this.activateChore == null)
		{
			return;
		}
		this.activateChore.Cancel("User cancelled");
		this.activateChore = null;
	}

	public string SidescreenButtonText
	{
		get
		{
			return (this.activateChore == null) ? UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE : UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE_CANCEL;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			return (this.activateChore == null) ? UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_ACTIVATE : UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_CANCEL;
		}
	}

	public bool SidescreenEnabled()
	{
		return !this.activated;
	}

	public void OnSidescreenButtonPressed()
	{
		if (this.activateChore == null)
		{
			this.CreateChore();
		}
		else
		{
			this.CancelChore();
		}
		this.awaitingActivation = this.activateChore != null;
	}

	public bool SidescreenButtonInteractable()
	{
		return !this.activated;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	private Operational.Flag activatedFlag = new Operational.Flag("activated", Operational.Flag.Type.Requirement);

	[Serialize]
	private bool activated;

	[Serialize]
	private bool awaitingActivation;

	private Guid statusItem;

	private Chore activateChore;
}
