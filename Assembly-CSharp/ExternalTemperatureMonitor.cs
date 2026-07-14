using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class ExternalTemperatureMonitor : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.alive;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.alive.TagTransition(GameTags.Dead, this.dead, false).Update(new Action<ExternalTemperatureMonitor.Instance, float>(ExternalTemperatureMonitor.UpdateTemperatureTresholdModifiers), UpdateRate.SIM_1000ms, false).DefaultState(this.alive.comfortable);
		this.alive.comfortable.Transition(this.alive.transitionToTooWarm, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooHot() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).Transition(this.alive.transitionToTooCool, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooCold() && smi.timeinstate > 6f, UpdateRate.SIM_200ms);
		this.alive.transitionToTooWarm.Transition(this.alive.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooHot(), UpdateRate.SIM_200ms).Transition(this.alive.tooWarm, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooHot() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		this.alive.transitionToTooCool.Transition(this.alive.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooCold(), UpdateRate.SIM_200ms).Transition(this.alive.tooCool, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooCold() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		this.alive.tooWarm.ToggleTag(GameTags.FeelingWarm).Transition(this.alive.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooHot() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).EventHandlerTransition(GameHashes.EffectAdded, this.alive.comfortable, (ExternalTemperatureMonitor.Instance smi, object obj) => !smi.IsTooHot())
			.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort, true);
			});
		this.alive.tooCool.ToggleTag(GameTags.FeelingCold).Transition(this.alive.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooCold() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).EventHandlerTransition(GameHashes.EffectAdded, this.alive.comfortable, (ExternalTemperatureMonitor.Instance smi, object obj) => !smi.IsTooCold())
			.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort, true);
			});
		this.dead.DoNothing();
	}

	public static void UpdateTemperatureTresholdModifiers(ExternalTemperatureMonitor.Instance smi, float dt)
	{
		smi.UpdateTemperatureTresholdModifiers(dt);
	}

	public static float GetExternalColdThreshold(ExternalTemperatureMonitor.Instance smi)
	{
		if (smi == null)
		{
			return -0.039f;
		}
		return -0.039f * smi.CurrentColdResistanceModifier;
	}

	public const float EXTERNAL_WARM_THRESHOLD = 0.008f;

	public const float EXTERNAL_COLD_THRESHOLD = -0.039f;

	public const float EXTERNAL_COLD_THRESHOLD_RESISTANCE_DURATION = 5f;

	public const float EXTERNAL_COLD_THRESHOLD_RESISTANCE_MULTIPLIER = 10f;

	public const string CHILLY_SURROUNDINGS_EFFECT_NAME = "ColdAir";

	public const string TOASTY_SURROUNDINGS_EFFECT_NAME = "WarmAir";

	public static readonly float BASE_STRESS_TOLERANCE_COLD = DUPLICANTSTATS.STANDARD.BaseStats.DUPLICANT_WARMING_KILOWATTS * 0.2f;

	public static readonly float BASE_STRESS_TOLERANCE_WARM = DUPLICANTSTATS.STANDARD.BaseStats.DUPLICANT_COOLING_KILOWATTS * 0.2f;

	private const float START_GAME_AVERAGING_DELAY = 6f;

	private const float TRANSITION_TO_DELAY = 1f;

	private const float TRANSITION_OUT_DELAY = 6f;

	public ExternalTemperatureMonitor.AliveStates alive;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State dead;

	private StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.FloatParameter _ColdResistanceDurationRemaining;

	public class AliveStates : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State comfortable;

		public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooWarm;

		public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooWarm;

		public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooCool;

		public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooCool;
	}

	public new class Instance : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public float CurrentColdResistanceModifier
		{
			get
			{
				if (!this.HasColdResistance)
				{
					return 1f;
				}
				return 10f;
			}
		}

		public bool HasColdResistance
		{
			get
			{
				return this.ColdResistanceDurationRemaining > 0f;
			}
		}

		public float ColdResistanceDurationRemaining
		{
			get
			{
				return base.sm._ColdResistanceDurationRemaining.Get(this);
			}
		}

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.attributes = base.gameObject.GetAttributes();
			this.minionResume = base.gameObject.GetComponent<MinionResume>();
			this.internalTemperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			this.temperatureTransferer = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
			this.primaryElement = base.gameObject.GetComponent<PrimaryElement>();
			this.effects = base.gameObject.GetComponent<Effects>();
			this.traits = base.gameObject.GetComponent<Traits>();
		}

		public bool IsTooHot()
		{
			return !this.effects.HasEffect("RefreshingTouch") && !this.effects.HasImmunityTo(this.warmAirEffect) && this.temperatureTransferer.LastTemperatureRecordIsReliable && base.smi.temperatureTransferer.average_kilowatts_exchanged.GetUnweightedAverage > 0.008f;
		}

		public bool IsTooCold()
		{
			for (int i = 0; i < this.immunityToColdEffects.Length; i++)
			{
				if (this.effects.HasEffect(this.immunityToColdEffects[i]))
				{
					return false;
				}
			}
			return !this.effects.HasImmunityTo(this.coldAirEffect) && (!(this.traits != null) || !this.traits.IsEffectIgnored(this.coldAirEffect)) && !WarmthProvider.IsWarmCell(Grid.PosToCell(this)) && this.temperatureTransferer.LastTemperatureRecordIsReliable && base.smi.temperatureTransferer.average_kilowatts_exchanged.GetUnweightedAverage < ExternalTemperatureMonitor.GetExternalColdThreshold(this);
		}

		public void UpdateTemperatureTresholdModifiers(float dt)
		{
			int num = Grid.PosToCell(this);
			bool flag = this.minionResume != null && this.minionResume.HasPerk(Db.Get().SkillPerks.ImprovedLiquidTemperatureTolerance.Id);
			if (Grid.IsValidCell(num) && Grid.Element[num].IsLiquid && flag)
			{
				base.sm._ColdResistanceDurationRemaining.Set(5f, this, false);
				return;
			}
			float num2 = this.ColdResistanceDurationRemaining - dt;
			num2 = Mathf.Max(0f, num2);
			base.sm._ColdResistanceDurationRemaining.Set(num2, this, false);
		}

		public Effects effects;

		public Traits traits;

		public Attributes attributes;

		public AmountInstance internalTemperature;

		public CreatureSimTemperatureTransfer temperatureTransferer;

		public PrimaryElement primaryElement;

		public MinionResume minionResume;

		private Effect coldAirEffect = Db.Get().effects.Get("ColdAir");

		private Effect[] immunityToColdEffects = new Effect[]
		{
			Db.Get().effects.Get("WarmTouch"),
			Db.Get().effects.Get("WarmTouchFood")
		};

		private Effect warmAirEffect = Db.Get().effects.Get("WarmAir");
	}
}
