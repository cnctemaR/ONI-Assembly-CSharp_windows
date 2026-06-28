using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class TemperatureMonitor : GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.homeostatic;
		this.root.Enter(delegate(TemperatureMonitor.Instance smi)
		{
			smi.averageTemperature = smi.primaryElement.Temperature;
			DiseaseTrigger component = smi.master.GetComponent<DiseaseTrigger>();
			if (component != null)
			{
				component.AddTrigger(GameHashes.TooHotDisease, new string[] { "HeatRash" }, (GameObject s, GameObject t) => DUPLICANTS.DISEASES.INFECTIONSOURCES.INTERNAL_TEMPERATURE);
				component.AddTrigger(GameHashes.TooColdDisease, new string[] { "ColdBrain" }, (GameObject s, GameObject t) => DUPLICANTS.DISEASES.INFECTIONSOURCES.INTERNAL_TEMPERATURE);
			}
		}).Update(delegate(TemperatureMonitor.Instance smi)
		{
		});
		this.homeostatic.Transition(this.hyperthermic_pre, (TemperatureMonitor.Instance smi) => smi.IsHyperthermic()).Transition(this.hypothermic_pre, (TemperatureMonitor.Instance smi) => smi.IsHypothermic()).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null);
		this.hyperthermic_pre.Enter(delegate(TemperatureMonitor.Instance smi)
		{
			smi.master.Trigger(534694243, smi.master.gameObject);
			smi.GoTo(this.hyperthermic);
		});
		this.hypothermic_pre.Enter(delegate(TemperatureMonitor.Instance smi)
		{
			smi.master.Trigger(1662224548, smi.master.gameObject);
			smi.GoTo(this.hypothermic);
		});
		this.hyperthermic.Transition(this.homeostatic, (TemperatureMonitor.Instance smi) => !smi.IsHyperthermic()).ToggleUrge(Db.Get().Urges.CoolDown);
		this.hypothermic.Transition(this.homeostatic, (TemperatureMonitor.Instance smi) => !smi.IsHypothermic()).ToggleUrge(Db.Get().Urges.WarmUp);
		this.deathcold.Enter("KillCold", delegate(TemperatureMonitor.Instance smi)
		{
			smi.KillCold();
		}).TriggerOnEnter(GameHashes.TooColdFatal, null);
		this.deathhot.Enter("KillHot", delegate(TemperatureMonitor.Instance smi)
		{
			smi.KillHot();
		}).TriggerOnEnter(GameHashes.TooHotFatal, null);
	}

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State homeostatic;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hyperthermic;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hypothermic;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hyperthermic_pre;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State hypothermic_pre;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State deathcold;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget, object>.State deathhot;

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

		public void UpdateTemperatureOnSimUpdate(float dt)
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

		public bool IsFatalHypothermic()
		{
			return this.temperature.value < this.FatalHypothermia;
		}

		public bool IsFatalHyperthermic()
		{
			return this.temperature.value > this.FatalHyperthermia;
		}

		public void KillHot()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Overheating);
		}

		public void KillCold()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Frozen);
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
			base.sm.warmUpCell.Set(this.warmUpQuery.GetResultCell(), base.smi);
		}

		public void UpdateCoolDownCell()
		{
			this.coolDownQuery.Reset();
			this.navigator.RunQuery(this.coolDownQuery);
			base.sm.coolDownCell.Set(this.coolDownQuery.GetResultCell(), base.smi);
		}

		public AmountInstance temperature;

		public PrimaryElement primaryElement;

		private Navigator navigator;

		private SafetyQuery warmUpQuery;

		private SafetyQuery coolDownQuery;

		public float averageTemperature;

		public float HypothermiaThreshold = 307.15f;

		public float HyperthermiaThreshold = 313.15f;

		public float FatalHypothermia = 305.15f;

		public float FatalHyperthermia = 315.15f;
	}
}
