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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.UpdateFlag();
		if (this.awaitingActivation && this.activateChore == null)
		{
			this.CreateChore();
		}
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		this.activated = true;
		if (this.onActivate != null)
		{
			this.onActivate();
		}
		this.awaitingActivation = false;
		this.UpdateFlag();
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCompleteWork(worker);
	}

	private void UpdateFlag()
	{
		base.GetComponent<Operational>().SetFlag(this.Required ? Activatable.activatedFlagRequirement : Activatable.activatedFlagFunctional, this.activated);
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.DuplicantActivationRequired, !this.activated, null);
		base.Trigger(-1909216579, BoxedBools.Box(this.IsActivated));
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

	public int HorizontalGroupID()
	{
		return -1;
	}

	public string SidescreenButtonText
	{
		get
		{
			if (this.activateChore != null)
			{
				return this.textOverride.IsValid ? this.textOverride.CancelText : UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE_CANCEL;
			}
			return this.textOverride.IsValid ? this.textOverride.Text : UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			if (this.activateChore != null)
			{
				return this.textOverride.IsValid ? this.textOverride.CancelToolTip : UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_CANCEL;
			}
			return this.textOverride.IsValid ? this.textOverride.ToolTip : UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_ACTIVATE;
		}
	}

	public bool SidescreenEnabled()
	{
		return !this.activated;
	}

	public void SetButtonTextOverride(ButtonMenuTextOverride text)
	{
		this.textOverride = text;
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
		return !this.activated && (this.activationCondition == null || this.activationCondition());
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	public bool Required = true;

	private static readonly Operational.Flag activatedFlagRequirement = new Operational.Flag("activated", Operational.Flag.Type.Requirement);

	private static readonly Operational.Flag activatedFlagFunctional = new Operational.Flag("activated", Operational.Flag.Type.Functional);

	[Serialize]
	private bool activated;

	[Serialize]
	private bool awaitingActivation;

	public Func<bool> activationCondition;

	private Guid statusItem;

	private Chore activateChore;

	public global::System.Action onActivate;

	[SerializeField]
	private ButtonMenuTextOverride textOverride;
}
