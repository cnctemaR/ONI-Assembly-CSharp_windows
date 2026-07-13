using System;
using Klei.AI;
using TUNING;

public class ExternalTemperatureMonitor : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance>
{
	public static float GetExternalColdThreshold(Attributes affected_attributes)
	{
		return -0.039f;
	}

	public static float GetExternalWarmThreshold(Attributes affected_attributes)
	{
		return 0.008f;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.comfortable;
		this.comfortable.Transition(this.transitionToTooWarm, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooHot() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).Transition(this.transitionToTooCool, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooCold() && smi.timeinstate > 6f, UpdateRate.SIM_200ms);
		this.transitionToTooWarm.Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooHot(), UpdateRate.SIM_200ms).Transition(this.tooWarm, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooHot() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		this.transitionToTooCool.Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooCold(), UpdateRate.SIM_200ms).Transition(this.tooCool, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooCold() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		this.tooWarm.ToggleTag(GameTags.FeelingWarm).Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooHot() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).EventHandlerTransition(GameHashes.EffectAdded, this.comfortable, (ExternalTemperatureMonitor.Instance smi, object obj) => !smi.IsTooHot())
			.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort, true);
			});
		this.tooCool.ToggleTag(GameTags.FeelingCold).Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooCold() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).EventHandlerTransition(GameHashes.EffectAdded, this.comfortable, (ExternalTemperatureMonitor.Instance smi, object obj) => !smi.IsTooCold())
			.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort, true);
			});
	}

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State comfortable;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooWarm;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooWarm;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooCool;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooCool;

	private const float BODY_TEMPERATURE_AFFECT_EXTERNAL_FEEL_THRESHOLD = 0.5f;

	public const string CHILLY_SURROUNDINGS_EFFECT_NAME = "ColdAir";

	public const string TOASTY_SURROUNDINGS_EFFECT_NAME = "WarmAir";

	public static readonly float BASE_STRESS_TOLERANCE_COLD = DUPLICANTSTATS.STANDARD.BaseStats.DUPLICANT_WARMING_KILOWATTS * 0.2f;

	public static readonly float BASE_STRESS_TOLERANCE_WARM = DUPLICANTSTATS.STANDARD.BaseStats.DUPLICANT_COOLING_KILOWATTS * 0.2f;

	private const float START_GAME_AVERAGING_DELAY = 6f;

	private const float TRANSITION_TO_DELAY = 1f;

	private const float TRANSITION_OUT_DELAY = 6f;

	public new class Instance : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public float GetCurrentColdThreshold
		{
			get
			{
				if (this.internalTemperatureMonitor.IdealTemperatureDelta() > 0.5f)
				{
					return 0f;
				}
				return CreatureSimTemperatureTransfer.PotentialEnergyFlowToCreature(Grid.PosToCell(base.gameObject), this.primaryElement, this.temperatureTransferer, 1f);
			}
		}

		public float GetCurrentHotThreshold
		{
			get
			{
				return this.HotThreshold;
			}
		}

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.attributes = base.gameObject.GetAttributes();
			this.internalTemperatureMonitor = base.gameObject.GetSMI<TemperatureMonitor.Instance>();
			this.internalTemperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			this.temperatureTransferer = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
			this.primaryElement = base.gameObject.GetComponent<PrimaryElement>();
			this.effects = base.gameObject.GetComponent<Effects>();
			this.traits = base.gameObject.GetComponent<Traits>();
		}

		public bool IsTooHot()
		{
			return !this.effects.HasEffect("RefreshingTouch") && !this.effects.HasImmunityTo(this.warmAirEffect) && this.temperatureTransferer.LastTemperatureRecordIsReliable && base.smi.temperatureTransferer.average_kilowatts_exchanged.GetUnweightedAverage > ExternalTemperatureMonitor.GetExternalWarmThreshold(base.smi.attributes);
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
			return !this.effects.HasImmunityTo(this.coldAirEffect) && (!(this.traits != null) || !this.traits.IsEffectIgnored(this.coldAirEffect)) && !WarmthProvider.IsWarmCell(Grid.PosToCell(this)) && this.temperatureTransferer.LastTemperatureRecordIsReliable && base.smi.temperatureTransferer.average_kilowatts_exchanged.GetUnweightedAverage < ExternalTemperatureMonitor.GetExternalColdThreshold(base.smi.attributes);
		}

		public float HotThreshold = 306.15f;

		public Effects effects;

		public Traits traits;

		public Attributes attributes;

		public AmountInstance internalTemperature;

		private TemperatureMonitor.Instance internalTemperatureMonitor;

		public CreatureSimTemperatureTransfer temperatureTransferer;

		public PrimaryElement primaryElement;

		private Effect warmAirEffect = Db.Get().effects.Get("WarmAir");

		private Effect coldAirEffect = Db.Get().effects.Get("ColdAir");

		private Effect[] immunityToColdEffects = new Effect[]
		{
			Db.Get().effects.Get("WarmTouch"),
			Db.Get().effects.Get("WarmTouchFood")
		};
	}
}
