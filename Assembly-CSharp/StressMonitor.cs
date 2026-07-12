using System;
using Klei.AI;
using Klei.CustomSettings;

public class StressMonitor : GameStateMachine<StressMonitor, StressMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		default_state = this.satisfied;
		this.root.Update("StressMonitor", delegate(StressMonitor.Instance smi, float dt)
		{
			smi.ReportStress(dt);
		}, UpdateRate.SIM_200ms, false);
		this.satisfied.TriggerOnEnter(GameHashes.NotStressed, null).Transition(this.stressed.tier1, (StressMonitor.Instance smi) => smi.stress.value >= 60f, UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Neutral, null);
		this.stressed.ToggleStatusItem(Db.Get().DuplicantStatusItems.Stressed, null).Transition(this.satisfied, (StressMonitor.Instance smi) => smi.stress.value < 60f, UpdateRate.SIM_200ms).ToggleReactable((StressMonitor.Instance smi) => smi.CreateConcernReactable())
			.TriggerOnEnter(GameHashes.Stressed, null);
		this.stressed.tier1.Transition(this.stressed.tier2, (StressMonitor.Instance smi) => smi.HasHadEnough(), UpdateRate.SIM_200ms);
		this.stressed.tier2.TriggerOnEnter(GameHashes.StressedHadEnough, null).Transition(this.stressed.tier1, (StressMonitor.Instance smi) => !smi.HasHadEnough(), UpdateRate.SIM_200ms);
	}

	public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public StressMonitor.Stressed stressed;

	private const float StressThreshold_One = 60f;

	private const float StressThreshold_Two = 100f;

	public class Stressed : GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier1;

		public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier2;
	}

	public new class Instance : GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.stress = Db.Get().Amounts.Stress.Lookup(base.gameObject);
			SettingConfig settingConfig = CustomGameSettings.Instance.QualitySettings[CustomGameSettingConfigs.StressBreaks.id];
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.StressBreaks);
			this.allowStressBreak = settingConfig.IsDefaultLevel(currentQualitySetting.id);
		}

		public bool IsStressed()
		{
			return base.IsInsideState(base.sm.stressed);
		}

		public bool HasHadEnough()
		{
			return this.allowStressBreak && this.stress.value >= 100f;
		}

		public void ReportStress(float dt)
		{
			for (int num = 0; num != this.stress.deltaAttribute.Modifiers.Count; num++)
			{
				AttributeModifier attributeModifier = this.stress.deltaAttribute.Modifiers[num];
				DebugUtil.DevAssert(!attributeModifier.IsMultiplier, "Reporting stress for multipliers not supported yet.", null);
				ReportManager.Instance.ReportValue(ReportManager.ReportType.StressDelta, attributeModifier.Value * dt, attributeModifier.GetDescription(), base.gameObject.GetProperName());
			}
		}

		public Reactable CreateConcernReactable()
		{
			return new EmoteReactable(base.master.gameObject, "StressConcern", Db.Get().ChoreTypes.Emote, 15, 8, 0f, 30f, float.PositiveInfinity, 0f).SetEmote(Db.Get().Emotes.Minion.Concern);
		}

		public AmountInstance stress;

		private bool allowStressBreak = true;
	}
}
