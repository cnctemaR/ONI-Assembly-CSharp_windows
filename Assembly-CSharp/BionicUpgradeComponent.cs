using System;
using STRINGS;

public class BionicUpgradeComponent : Assignable
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
				return string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.STANDARD_INACTIVE_TEMPLATE, this.GetProperName(), GameUtil.GetFormattedWattage(this.PotentialWattage, GameUtil.WattageFormatterUnit.Automatic, true));
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

	public BionicUpgradeComponentConfig.RarityType Rarity
	{
		get
		{
			return this.data.rarity;
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
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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
					BionicUpgradesMonitor.UpgradeComponentSlot firstEmptySlot = smi.GetFirstEmptySlot();
					if (firstEmptySlot != null)
					{
						assignableSlotInstance = firstEmptySlot.GetAssignableSlotInstance();
					}
				}
			}
		}
		base.Assign(new_assignee, assignableSlotInstance);
		base.Trigger(1980521255, null);
	}

	public override void Unassign()
	{
		base.Unassign();
		base.Trigger(1980521255, null);
	}

	private bool AssignablePrecondition_OnlyOnBionics(MinionAssignablesProxy worker)
	{
		bool flag = false;
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			flag = minionIdentity.GetSMI<BionicUpgradesMonitor.Instance>() != null;
		}
		return flag;
	}

	private BionicUpgradeComponentConfig.BionicUpgradeData data;

	public global::System.Action OnWattageCostChanged;

	public interface IWattageController
	{
		float GetCurrentWattageCost();

		string GetCurrentWattageCostName();
	}
}
