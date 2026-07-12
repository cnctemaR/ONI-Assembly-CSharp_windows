using System;
using Klei.AI;

public class TemperatureMonitor : GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.homeostatic;
		this.root.Enter(delegate(TemperatureMonitor.Instance smi)
		{
			smi.averageTemperature = smi.primaryElement.Temperature;
		}).Update("UpdateTemperature", delegate(TemperatureMonitor.Instance smi, float dt)
		{
			smi.UpdateTemperature(dt);
		}, UpdateRate.SIM_200ms, false);
		this.homeostatic.Transition(this.hyperthermic_pre, (TemperatureMonitor.Instance smi) => smi.IsHyperthermic(), UpdateRate.SIM_200ms).Transition(this.hypothermic_pre, (TemperatureMonitor.Instance smi) => smi.IsHypothermic(), UpdateRate.SIM_200ms).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null);
		this.hyperthermic_pre.Enter(delegate(TemperatureMonitor.Instance smi)
		{
			smi.GoTo(this.hyperthermic);
		});
		this.hypothermic_pre.Enter(delegate(TemperatureMonitor.Instance smi)
		{
			smi.GoTo(this.hypothermic);
		});
		this.hyperthermic.Transition(this.homeostatic, (TemperatureMonitor.Instance smi) => !smi.IsHyperthermic(), UpdateRate.SIM_200ms).ToggleUrge(Db.Get().Urges.CoolDown);
		this.hypothermic.Transition(this.homeostatic, (TemperatureMonitor.Instance smi) => !smi.IsHypothermic(), UpdateRate.SIM_200ms).ToggleUrge(Db.Get().Urges.WarmUp);
	}

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State homeostatic;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hyperthermic;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hypothermic;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hyperthermic_pre;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hypothermic_pre;

	private const float TEMPERATURE_AVERAGING_RANGE = 4f;

	public StateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.IntParameter warmUpCell;

	public StateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.IntParameter coolDownCell;

	public new class Instance : GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.primaryElement = base.GetComponent<PrimaryElement>();
			this.temperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			this.warmUpQuery = new SafetyQuery(Game.Instance.safetyConditions.WarmUpChecker, base.GetComponent<KMonoBehaviour>(), int.MaxValue);
			this.coolDownQuery = new SafetyQuery(Game.Instance.safetyConditions.CoolDownChecker, base.GetComponent<KMonoBehaviour>(), int.MaxValue);
			this.navigator = base.GetComponent<Navigator>();
		}

		public void UpdateTemperature(float dt)
		{
			base.smi.averageTemperature *= 1f - dt / 4f;
			base.smi.averageTemperature += base.smi.primaryElement.Temperature * (dt / 4f);
			base.smi.temperature.SetValue(base.smi.averageTemperature);
		}

		public bool IsHyperthermic()
		{
			return this.temperature.value > this.HyperthermiaThreshold;
		}

		public bool IsHypothermic()
		{
			return this.temperature.value < this.HypothermiaThreshold;
		}

		public float ExtremeTemperatureDelta()
		{
			if (this.temperature.value > this.HyperthermiaThreshold)
			{
				return this.temperature.value - this.HyperthermiaThreshold;
			}
			if (this.temperature.value < this.HypothermiaThreshold)
			{
				return this.temperature.value - this.HypothermiaThreshold;
			}
			return 0f;
		}

		public float IdealTemperatureDelta()
		{
			return this.temperature.value - 310.15f;
		}

		public int GetWarmUpCell()
		{
			return base.sm.warmUpCell.Get(base.smi);
		}

		public int GetCoolDownCell()
		{
			return base.sm.coolDownCell.Get(base.smi);
		}

		public void UpdateWarmUpCell()
		{
			this.warmUpQuery.Reset();
			this.navigator.RunQuery(this.warmUpQuery);
			base.sm.warmUpCell.Set(this.warmUpQuery.GetResultCell(), base.smi, false);
		}

		public void UpdateCoolDownCell()
		{
			this.coolDownQuery.Reset();
			this.navigator.RunQuery(this.coolDownQuery);
			base.sm.coolDownCell.Set(this.coolDownQuery.GetResultCell(), base.smi, false);
		}

		public AmountInstance temperature;

		public PrimaryElement primaryElement;

		private Navigator navigator;

		private SafetyQuery warmUpQuery;

		private SafetyQuery coolDownQuery;

		public float averageTemperature;

		public float HypothermiaThreshold = 307.15f;

		public float HyperthermiaThreshold = 313.15f;
	}
}
