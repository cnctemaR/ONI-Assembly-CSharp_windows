using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BionicUpgradeComponent : Assignable, IGameObjectEffectDescriptor
{
	public BionicUpgradeComponent.IWattageController WattageController { get; private set; }

	public float CurrentWattage
	{
		get
		{
			if (!this.HasWattageController)
			{
				return 0f;
			}
			return this.WattageController.GetCurrentWattageCost();
		}
	}

	public string CurrentWattageName
	{
		get
		{
			if (!this.HasWattageController)
			{
				return string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_INACTIVE_TEMPLATE, this.GetProperName(), GameUtil.GetFormattedWattage(this.PotentialWattage, GameUtil.WattageFormatterUnit.Automatic, true));
			}
			return this.WattageController.GetCurrentWattageCostName();
		}
	}

	public bool HasWattageController
	{
		get
		{
			return this.WattageController != null;
		}
	}

	public float PotentialWattage
	{
		get
		{
			return this.data.WattageCost;
		}
	}

	public BionicUpgradeComponentConfig.BoosterType Booster
	{
		get
		{
			return this.data.Booster;
		}
	}

	public Func<StateMachine.Instance, StateMachine.Instance> StateMachine
	{
		get
		{
			return this.data.stateMachine;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.data = BionicUpgradeComponentConfig.UpgradesData[base.gameObject.PrefabID()];
		base.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.AssignablePrecondition_OnlyOnBionics));
		base.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.AssignablePrecondition_HasAvailableSlots));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.assignmentManager.Remove(this);
		this.customAssignmentUITooltipFunc = new Func<Assignables, string>(this.GetTooltipForBoosterAssignment);
		this.customAssignablesUITooltipFunc = new Func<Assignables, string>(this.GetTooltipForMinionAssigment);
		base.Subscribe(856640610, new Action<object>(this.RefreshStatusItem));
		this.RefreshStatusItem(null);
	}

	private void RefreshStatusItem(object _ = null)
	{
		if (this.assignee == null && !base.gameObject.HasTag(GameTags.Stored))
		{
			if (this.unassignedStatusItem == Guid.Empty)
			{
				this.unassignedStatusItem = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.UnassignedBionicBooster, null);
				return;
			}
		}
		else if (this.unassignedStatusItem != Guid.Empty)
		{
			this.unassignedStatusItem = base.GetComponent<KSelectable>().RemoveStatusItem(this.unassignedStatusItem, false);
		}
	}

	public string GetTooltipForMinionAssigment(Assignables assignables)
	{
		MinionAssignablesProxy component = assignables.GetComponent<MinionAssignablesProxy>();
		if (component == null)
		{
			return "ERROR N/A";
		}
		GameObject targetGameObject = component.GetTargetGameObject();
		if (targetGameObject == null)
		{
			return "ERROR N/A";
		}
		BionicUpgradesMonitor.Instance smi = targetGameObject.GetSMI<BionicUpgradesMonitor.Instance>();
		if (smi == null)
		{
			return "This Duplicant cannot install boosters";
		}
		int num = smi.CountBoosterAssignments(this.PrefabID());
		string text = ((num == 0) ? string.Format(UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.NOT_ALREADY_ASSIGNED, smi.gameObject.GetProperName(), num) : string.Format(UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.ALREADY_ASSIGNED, smi.gameObject.GetProperName(), num));
		string text2 = string.Format((smi.AssignedSlotCount < smi.UnlockedSlotCount) ? UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.AVAILABLE_SLOTS : UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.NO_AVAILABLE_SLOTS, targetGameObject.GetProperName(), smi.AssignedSlotCount, smi.UnlockedSlotCount);
		string text3 = "";
		List<AttributeInstance> list = new List<AttributeInstance>(targetGameObject.GetAttributes().AttributeTable).FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill);
		for (int i = 0; i < list.Count; i++)
		{
			string text4 = UIConstants.ColorPrefixWhite;
			if (list[i].GetTotalValue() > 0f)
			{
				text4 = UIConstants.ColorPrefixGreen;
			}
			else if (list[i].GetTotalValue() < 0f)
			{
				text4 = UIConstants.ColorPrefixRed;
			}
			text3 += string.Format("{0}: {1}", list[i].Name, text4 + list[i].GetFormattedValue() + UIConstants.ColorSuffix);
			if (i != list.Count - 1)
			{
				text3 += "\n";
			}
		}
		return string.Concat(new string[]
		{
			targetGameObject.GetProperName(),
			"\n\n",
			text,
			"\n\n",
			text2,
			"\n\n",
			text3
		});
	}

	public string GetTooltipForBoosterAssignment(Assignables assignables)
	{
		MinionAssignablesProxy component = assignables.GetComponent<MinionAssignablesProxy>();
		if (component == null)
		{
			return "ERROR N/A";
		}
		GameObject targetGameObject = component.GetTargetGameObject();
		if (targetGameObject == null)
		{
			return "ERROR N/A";
		}
		BionicUpgradesMonitor.Instance smi = targetGameObject.GetSMI<BionicUpgradesMonitor.Instance>();
		if (smi == null)
		{
			return "ERROR N/A";
		}
		int num = smi.CountBoosterAssignments(this.PrefabID());
		string text = ((num == 0) ? string.Format(UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.NOT_ALREADY_ASSIGNED, smi.gameObject.GetProperName(), num) : string.Format(UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.ALREADY_ASSIGNED, smi.gameObject.GetProperName(), num));
		return BionicUpgradeComponentConfig.GenerateTooltipForBooster(this) + "\n\n" + text;
	}

	public void InformOfWattageChanged()
	{
		global::System.Action onWattageCostChanged = this.OnWattageCostChanged;
		if (onWattageCostChanged == null)
		{
			return;
		}
		onWattageCostChanged();
	}

	public void SetWattageController(BionicUpgradeComponent.IWattageController wattageController)
	{
		this.WattageController = wattageController;
	}

	public override void Assign(IAssignableIdentity new_assignee)
	{
		AssignableSlotInstance assignableSlotInstance = null;
		if (new_assignee == this.assignee)
		{
			return;
		}
		if (new_assignee != this.assignee && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity || new_assignee is MinionAssignablesProxy))
		{
			Ownables soleOwner = new_assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				BionicUpgradesMonitor.Instance smi = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetSMI<BionicUpgradesMonitor.Instance>();
				if (smi != null)
				{
					BionicUpgradesMonitor.UpgradeComponentSlot firstEmptyAvailableSlot = smi.GetFirstEmptyAvailableSlot();
					if (firstEmptyAvailableSlot != null)
					{
						assignableSlotInstance = firstEmptyAvailableSlot.GetAssignableSlotInstance();
					}
				}
			}
		}
		base.Assign(new_assignee, assignableSlotInstance);
		base.Trigger(1980521255, null);
		this.RefreshStatusItem(null);
	}

	public override void Unassign()
	{
		base.Unassign();
		base.Trigger(1980521255, null);
		this.RefreshStatusItem(null);
	}

	private bool AssignablePrecondition_OnlyOnBionics(MinionAssignablesProxy worker)
	{
		return worker.GetMinionModel() == BionicMinionConfig.MODEL;
	}

	private bool AssignablePrecondition_HasAvailableSlots(MinionAssignablesProxy worker)
	{
		if (SelectTool.Instance.selected != null && SelectTool.Instance.selected.gameObject == worker.GetTargetGameObject())
		{
			return true;
		}
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			BionicUpgradesMonitor.Instance smi = minionIdentity.GetSMI<BionicUpgradesMonitor.Instance>();
			if (smi == null)
			{
				return true;
			}
			foreach (BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot in smi.upgradeComponentSlots)
			{
				if (!upgradeComponentSlot.IsLocked && (upgradeComponentSlot.assignedUpgradeComponent == null || upgradeComponentSlot.assignedUpgradeComponent == this))
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(BionicUpgradeComponentConfig.UpgradesData[base.gameObject.PrefabID()].stateMachineDescription, null, Descriptor.DescriptorType.Effect, false)
		};
	}

	private BionicUpgradeComponentConfig.BionicUpgradeData data;

	public global::System.Action OnWattageCostChanged;

	private Guid unassignedStatusItem = Guid.Empty;

	public interface IWattageController
	{
		float GetCurrentWattageCost();

		string GetCurrentWattageCostName();
	}
}
