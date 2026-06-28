using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ExternalTemperatureMonitor : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance>
{
	public static float GetExternalColdThreshold(Attributes affected_attributes)
	{
		if (affected_attributes == null)
		{
			return -0.36261335f;
		}
		return -(0.36261335f - affected_attributes.GetValue(Db.Get().Attributes.RoomTemperaturePreference.Id));
	}

	public static float GetExternalWarmThreshold(Attributes affected_attributes)
	{
		if (affected_attributes == null)
		{
			return 0.19525334f;
		}
		return -(-0.19525334f - affected_attributes.GetValue(Db.Get().Attributes.RoomTemperaturePreference.Id));
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.comfortable;
		this.root.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
		{
			smi.AverageExternalTemperature = smi.GetCurrentExternalTemperature;
		}).Update(delegate(ExternalTemperatureMonitor.Instance smi)
		{
			smi.AverageExternalTemperature *= Mathf.Max(0f, 1f - smi.dt / 6f);
			smi.AverageExternalTemperature += smi.GetCurrentExternalTemperature * (smi.dt / 6f);
		});
		this.comfortable.Transition(this.transitionToTooWarm, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooHot() && smi.timeinstate > 6f).Transition(this.transitionToTooCool, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooCold() && smi.timeinstate > 6f);
		this.transitionToTooWarm.Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooHot()).Transition(this.tooWarm, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooHot() && smi.timeinstate > 1f);
		this.transitionToTooCool.Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooCold()).Transition(this.tooCool, (ExternalTemperatureMonitor.Instance smi) => smi.IsTooCold() && smi.timeinstate > 1f);
		this.transitionToScalding.Transition(this.tooWarm, (ExternalTemperatureMonitor.Instance smi) => !smi.IsScalding()).Transition(this.scalding, (ExternalTemperatureMonitor.Instance smi) => smi.IsScalding() && smi.timeinstate > 1f);
		this.tooWarm.Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooHot() && smi.timeinstate > 6f).Transition(this.transitionToScalding, (ExternalTemperatureMonitor.Instance smi) => smi.IsScalding()).ToggleExpression(Db.Get().Expressions.Hot, null)
			.ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.Hot, (ExternalTemperatureMonitor.Instance smi) => smi)
			.ToggleEffect("WarmAir")
			.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort);
			});
		this.scalding.Transition(this.tooWarm, (ExternalTemperatureMonitor.Instance smi) => !smi.IsScalding() && smi.timeinstate > 6f).ToggleExpression(Db.Get().Expressions.Hot, null).ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Scalding, (ExternalTemperatureMonitor.Instance smi) => smi)
			.ToggleSchedulePeriodic("ScaldDamage", 2f, delegate(ExternalTemperatureMonitor.Instance smi)
			{
				smi.ScaldDamage(2f);
			});
		this.tooCool.Transition(this.comfortable, (ExternalTemperatureMonitor.Instance smi) => !smi.IsTooCold() && smi.timeinstate > 6f).ToggleExpression(Db.Get().Expressions.Cold, null).ToggleThought(Db.Get().Thoughts.Cold, null)
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.Cold, (ExternalTemperatureMonitor.Instance smi) => smi)
			.ToggleEffect("ColdAir")
			.Enter(delegate(ExternalTemperatureMonitor.Instance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort);
			});
	}

	private const float SCALD_DAMAGE_INTERVAL = 2f;

	private const float SCALDING_DAMAGE_AMOUNT = 10f;

	private const float BODY_TEMPERATURE_AFFECT_EXTERNAL_FEEL_THRESHOLD = 0.5f;

	public const float BASE_STRESS_TOLERANCE_COLD = 0.27893335f;

	public const float BASE_STRESS_TOLERANCE_WARM = 0.27893335f;

	private const float START_GAME_AVERAGING_DELAY = 6f;

	private const float TRANSITION_TO_DELAY = 1f;

	private const float TRANSITION_OUT_DELAY = 6f;

	private const float TEMPERATURE_AVERAGING_RANGE = 6f;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State comfortable;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooWarm;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooWarm;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooCool;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooCool;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToScalding;

	public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State scalding;

	public new class Instance : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.health = base.GetComponent<Health>();
			this.occupyArea = base.GetComponent<OccupyArea>();
			this.internalTemperatureMonitor = base.gameObject.GetSMI<TemperatureMonitor.Instance>();
			this.internalTemperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			this.temperatureTransferer = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
			this.primaryElement = base.gameObject.GetComponent<PrimaryElement>();
			this.attributes = base.gameObject.GetAttributes();
		}

		public float GetCurrentExternalTemperature
		{
			get
			{
				int num = Grid.PosToCell(base.gameObject);
				if (this.occupyArea != null)
				{
					float num2 = 0f;
					for (int i = 0; i < this.occupyArea.OccupiedCellsOffsets.Length; i++)
					{
						num2 += Grid.Temperature[Grid.OffsetCell(num, this.occupyArea.OccupiedCellsOffsets[i])];
					}
					return num2 / (float)this.occupyArea.OccupiedCellsOffsets.Length;
				}
				return Grid.Temperature[num];
			}
		}

		public override void StartSM()
		{
			base.StartSM();
			base.smi.attributes.Get("ScaldingThreshold").Add("base", this.baseScalindingThreshold);
		}

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

		public float GetScaldingThreshold()
		{
			return base.smi.attributes.GetValue("ScaldingThreshold");
		}

		public float GetCurrentHotThreshold
		{
			get
			{
				return this.HotThreshold;
			}
		}

		public bool IsTooHot()
		{
			return this.internalTemperatureMonitor.IdealTemperatureDelta() >= -0.5f && base.smi.temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage > ExternalTemperatureMonitor.GetExternalWarmThreshold(base.smi.attributes);
		}

		public bool IsTooCold()
		{
			return this.internalTemperatureMonitor.IdealTemperatureDelta() <= 0.5f && base.smi.temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage < ExternalTemperatureMonitor.GetExternalColdThreshold(base.smi.attributes);
		}

		public bool IsScalding()
		{
			return this.AverageExternalTemperature > base.smi.attributes.GetValue("ScaldingThreshold");
		}

		public void ScaldDamage(float deltaTime)
		{
			if (this.health != null)
			{
				this.health.Damage(this.deltatime * 10f);
			}
		}

		public float CurrentWorldTransferWattage()
		{
			return this.temperatureTransferer.currentExchangeWattage;
		}

		public float AverageExternalTemperature;

		public float ColdThreshold = 283.15f;

		public float HotThreshold = 306.15f;

		private AttributeModifier baseScalindingThreshold = new AttributeModifier("ScaldingThreshold", 345f, DUPLICANTS.STATS.SKIN_DURABILITY.NAME, false, false);

		public Attributes attributes;

		public OccupyArea occupyArea;

		public AmountInstance internalTemperature;

		private TemperatureMonitor.Instance internalTemperatureMonitor;

		public CreatureSimTemperatureTransfer temperatureTransferer;

		public Health health;

		public PrimaryElement primaryElement;
	}
}
