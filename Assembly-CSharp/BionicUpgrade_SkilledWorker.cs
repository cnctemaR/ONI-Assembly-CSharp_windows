using System;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BionicUpgrade_SkilledWorker : BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.Inactive;
		this.root.Enter(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.ApplySkillPerks)).Exit(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.RemoveSkillPerks)).Enter(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.ApplyModifiers))
			.Exit(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.RemoveModifiers))
			.Enter(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.ApplyHats))
			.Exit(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.RemoveHats));
		this.Inactive.EventTransition(GameHashes.ScheduleBlocksChanged, this.Active, new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SkilledWorker.IsMinionWorkingOnlineAndNotInBatterySaveMode)).EventTransition(GameHashes.ScheduleChanged, this.Active, new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SkilledWorker.IsMinionWorkingOnlineAndNotInBatterySaveMode)).EventTransition(GameHashes.BionicOnline, this.Active, new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SkilledWorker.IsMinionWorkingOnlineAndNotInBatterySaveMode))
			.EventTransition(GameHashes.StartWork, this.Active, new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SkilledWorker.IsMinionWorkingOnlineAndNotInBatterySaveMode))
			.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged, null);
		this.Active.EventTransition(GameHashes.ScheduleBlocksChanged, this.Inactive, new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.IsInBedTimeChore)).EventTransition(GameHashes.ScheduleChanged, this.Inactive, new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.IsInBedTimeChore)).EventTransition(GameHashes.BionicOffline, this.Inactive, null)
			.EventTransition(GameHashes.StopWork, this.Inactive, null)
			.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged, null)
			.Enter(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.CreateFX))
			.Exit(new StateMachine<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def>.State.Callback(BionicUpgrade_SkilledWorker.ClearFX));
	}

	public static void ApplySkillPerks(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.resume.ApplyAdditionalSkillPerks(((BionicUpgrade_SkilledWorker.Def)smi.def).SkillPerksIds);
	}

	public static void RemoveSkillPerks(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.resume.RemoveAdditionalSkillPerks(((BionicUpgrade_SkilledWorker.Def)smi.def).SkillPerksIds);
	}

	public static void ApplyModifiers(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.ApplyModifiers();
	}

	public static void RemoveModifiers(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.RemoveModifiers();
	}

	public static void ApplyHats(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.ApplyHats();
	}

	public static void RemoveHats(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.RemoveHats();
	}

	public static bool IsMinionWorkingOnlineAndNotInBatterySaveMode(BionicUpgrade_SkilledWorker.Instance smi)
	{
		return BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.IsOnline(smi) && !BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.IsInBedTimeChore(smi) && BionicUpgrade_SkilledWorker.IsMinionWorkingWithAttribute(smi);
	}

	public static bool IsMinionWorkingWithAttribute(BionicUpgrade_SkilledWorker.Instance smi)
	{
		Workable workable = smi.worker.GetWorkable();
		return workable != null && smi.worker.GetState() == WorkerBase.State.Working && workable.GetWorkAttribute() != null && workable.GetWorkAttribute().Id == ((BionicUpgrade_SkilledWorker.Def)smi.def).AttributeId;
	}

	public static void CreateFX(BionicUpgrade_SkilledWorker.Instance smi)
	{
		BionicUpgrade_SkilledWorker.CreateAndReturnFX(smi);
	}

	public static BionicAttributeUseFx.Instance CreateAndReturnFX(BionicUpgrade_SkilledWorker.Instance smi)
	{
		if (!smi.isMasterNull)
		{
			smi.fx = new BionicAttributeUseFx.Instance(smi.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.FXFront)));
			smi.fx.StartSM();
			return smi.fx;
		}
		return null;
	}

	public static void ClearFX(BionicUpgrade_SkilledWorker.Instance smi)
	{
		smi.fx.sm.destroyFX.Trigger(smi.fx);
		smi.fx = null;
	}

	public new class Def : BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.Def
	{
		public Def(string upgradeID, string attributeID, AttributeModifier[] modifiers = null, SkillPerk[] skillPerks = null, string[] hats = null)
			: base(upgradeID)
		{
			this.AttributeId = attributeID;
			this.modifiers = modifiers;
			this.SkillPerksIds = skillPerks;
			this.hats = hats;
		}

		public override string GetDescription()
		{
			string text = "";
			if (this.SkillPerksIds.Length != 0)
			{
				text += UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.HEADER_PERKS;
				for (int i = 0; i < this.SkillPerksIds.Length; i++)
				{
					text += "\n";
					text += SkillPerk.GetDescription(this.SkillPerksIds[i].Id);
				}
				if (this.modifiers.Length != 0)
				{
					text += "\n\n";
				}
			}
			if (this.modifiers.Length != 0)
			{
				text += UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.HEADER_ATTRIBUTES;
				for (int j = 0; j < this.modifiers.Length; j++)
				{
					text += "\n";
					text = text + this.modifiers[j].GetName() + ": " + this.modifiers[j].GetFormattedString();
				}
			}
			return text;
		}

		public SkillPerk[] SkillPerksIds;

		public string AttributeId;

		public AttributeModifier[] modifiers;

		public string[] hats;
	}

	public new class Instance : BionicUpgrade_SM<BionicUpgrade_SkilledWorker, BionicUpgrade_SkilledWorker.Instance>.BaseInstance
	{
		public Instance(IStateMachineTarget master, BionicUpgrade_SkilledWorker.Def def)
			: base(master, def)
		{
		}

		public override float GetCurrentWattageCost()
		{
			if (base.IsInsideState(base.sm.Active))
			{
				return base.Data.WattageCost;
			}
			return 0f;
		}

		public override string GetCurrentWattageCostName()
		{
			float currentWattageCost = this.GetCurrentWattageCost();
			if (base.IsInsideState(base.sm.Active))
			{
				string text = "<b>" + ((currentWattageCost >= 0f) ? "+" : "-") + "</b>";
				return string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, this.upgradeComponent.GetProperName(), text + GameUtil.GetFormattedWattage(currentWattageCost, GameUtil.WattageFormatterUnit.Automatic, true));
			}
			return string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_INACTIVE_TEMPLATE, this.upgradeComponent.GetProperName(), GameUtil.GetFormattedWattage(this.upgradeComponent.PotentialWattage, GameUtil.WattageFormatterUnit.Automatic, true));
		}

		public void ApplyModifiers()
		{
			Klei.AI.Attributes attributes = this.resume.GetIdentity.GetAttributes();
			foreach (AttributeModifier attributeModifier in ((BionicUpgrade_SkilledWorker.Def)base.smi.def).modifiers)
			{
				attributes.Add(attributeModifier);
			}
		}

		public void RemoveModifiers()
		{
			Klei.AI.Attributes attributes = this.resume.GetIdentity.GetAttributes();
			foreach (AttributeModifier attributeModifier in ((BionicUpgrade_SkilledWorker.Def)base.smi.def).modifiers)
			{
				attributes.Remove(attributeModifier);
			}
		}

		public void ApplyHats()
		{
			string[] hats = ((BionicUpgrade_SkilledWorker.Def)base.smi.def).hats;
			if (hats == null)
			{
				return;
			}
			MinionResume component = base.GetComponent<MinionResume>();
			string properName = Assets.GetPrefab(base.smi.def.UpgradeID).GetProperName();
			foreach (string text in hats)
			{
				component.AddAdditionalHat(properName, text);
			}
		}

		public void RemoveHats()
		{
			string[] hats = ((BionicUpgrade_SkilledWorker.Def)base.smi.def).hats;
			if (hats == null)
			{
				return;
			}
			MinionResume component = base.GetComponent<MinionResume>();
			string properName = Assets.GetPrefab(base.smi.def.UpgradeID).GetProperName();
			foreach (string text in hats)
			{
				component.RemoveAdditionalHat(properName, text);
			}
		}

		[MyCmpGet]
		public WorkerBase worker;

		[MyCmpGet]
		public MinionResume resume;

		public BionicAttributeUseFx.Instance fx;
	}
}
