using System;
using STRINGS;

public class BionicUpgrade_OnGoingEffect : BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.Inactive;
		this.Inactive.EventTransition(GameHashes.ScheduleBlocksChanged, this.Active, new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_OnGoingEffect.IsOnlineAndNotInBatterySaveMode)).EventTransition(GameHashes.ScheduleChanged, this.Active, new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_OnGoingEffect.IsOnlineAndNotInBatterySaveMode)).EventTransition(GameHashes.BionicOnline, this.Active, new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_OnGoingEffect.IsOnlineAndNotInBatterySaveMode))
			.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged, null);
		this.Active.ToggleEffect(new Func<BionicUpgrade_OnGoingEffect.Instance, string>(BionicUpgrade_OnGoingEffect.GetEffectName)).Enter(new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.State.Callback(BionicUpgrade_OnGoingEffect.ApplySkills)).Exit(new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.State.Callback(BionicUpgrade_OnGoingEffect.RemoveSkills))
			.EventTransition(GameHashes.ScheduleBlocksChanged, this.Inactive, new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.IsInBedTimeChore))
			.EventTransition(GameHashes.ScheduleChanged, this.Inactive, new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.IsInBedTimeChore))
			.EventTransition(GameHashes.BionicOffline, this.Inactive, GameStateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Not(new StateMachine<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance, IStateMachineTarget, BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def>.Transition.ConditionCallback(BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.IsOnline)))
			.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged, null);
	}

	public static string GetEffectName(BionicUpgrade_OnGoingEffect.Instance smi)
	{
		return ((BionicUpgrade_OnGoingEffect.Def)smi.def).EFFECT_NAME;
	}

	public static void ApplySkills(BionicUpgrade_OnGoingEffect.Instance smi)
	{
		smi.ApplySkills();
	}

	public static void RemoveSkills(BionicUpgrade_OnGoingEffect.Instance smi)
	{
		smi.RemoveSkills();
	}

	public static bool IsOnlineAndNotInBatterySaveMode(BionicUpgrade_OnGoingEffect.Instance smi)
	{
		return BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.IsOnline(smi) && !BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.IsInBedTimeChore(smi);
	}

	public new class Def : BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.Def
	{
		public Def(string upgradeID, string effectID, string[] skills = null)
			: base(upgradeID)
		{
			this.EFFECT_NAME = effectID;
			this.SKILLS_IDS = skills;
		}

		public override string GetDescription()
		{
			return "BionicUpgrade_OnGoingEffect.Def description not implemented";
		}

		public string EFFECT_NAME;

		public string[] SKILLS_IDS;
	}

	public new class Instance : BionicUpgrade_SM<BionicUpgrade_OnGoingEffect, BionicUpgrade_OnGoingEffect.Instance>.BaseInstance
	{
		public Instance(IStateMachineTarget master, BionicUpgrade_OnGoingEffect.Def def)
			: base(master, def)
		{
			this.resume = base.GetComponent<MinionResume>();
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

		public void ApplySkills()
		{
			BionicUpgrade_OnGoingEffect.Def def = (BionicUpgrade_OnGoingEffect.Def)base.def;
			if (def.SKILLS_IDS != null)
			{
				for (int i = 0; i < def.SKILLS_IDS.Length; i++)
				{
					string text = def.SKILLS_IDS[i];
					this.resume.GrantSkill(text);
				}
			}
		}

		public void RemoveSkills()
		{
			BionicUpgrade_OnGoingEffect.Def def = (BionicUpgrade_OnGoingEffect.Def)base.def;
			if (def.SKILLS_IDS != null)
			{
				for (int i = 0; i < def.SKILLS_IDS.Length; i++)
				{
					string text = def.SKILLS_IDS[i];
					this.resume.UngrantSkill(text);
				}
			}
		}

		private MinionResume resume;
	}
}
