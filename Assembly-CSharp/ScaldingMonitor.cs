using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ScaldingMonitor : GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.root.Enter(new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.State.Callback(ScaldingMonitor.SetInitialAverageExternalTemperature)).Update(new Action<ScaldingMonitor.Instance, float>(ScaldingMonitor.AverageExternalTemperatureUpdate), UpdateRate.SIM_200ms, false);
		this.idle.Transition(this.transitionToScalding, new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.IsScalding), UpdateRate.SIM_200ms).Transition(this.transitionToScolding, new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.IsScolding), UpdateRate.SIM_200ms);
		this.transitionToScalding.Transition(this.idle, GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Not(new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.IsScalding)), UpdateRate.SIM_200ms).Transition(this.scalding, new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.IsScaldingTimed), UpdateRate.SIM_200ms);
		this.transitionToScolding.Transition(this.idle, GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Not(new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.IsScolding)), UpdateRate.SIM_200ms).Transition(this.scolding, new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.IsScoldingTimed), UpdateRate.SIM_200ms);
		this.scalding.Transition(this.idle, new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.CanEscapeScalding), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Hot, null).ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Scalding, (ScaldingMonitor.Instance smi) => smi)
			.Update(new Action<ScaldingMonitor.Instance, float>(ScaldingMonitor.TakeScaldDamage), UpdateRate.SIM_1000ms, false);
		this.scolding.Transition(this.idle, new StateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.Transition.ConditionCallback(ScaldingMonitor.CanEscapeScolding), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Cold, null).ToggleThought(Db.Get().Thoughts.Cold, null)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Scolding, (ScaldingMonitor.Instance smi) => smi)
			.Update(new Action<ScaldingMonitor.Instance, float>(ScaldingMonitor.TakeColdDamage), UpdateRate.SIM_1000ms, false);
	}

	public static void SetInitialAverageExternalTemperature(ScaldingMonitor.Instance smi)
	{
		smi.AverageExternalTemperature = smi.GetCurrentExternalTemperature();
	}

	public static bool CanEscapeScalding(ScaldingMonitor.Instance smi)
	{
		return !smi.IsScalding() && smi.timeinstate > 1f;
	}

	public static bool CanEscapeScolding(ScaldingMonitor.Instance smi)
	{
		return !smi.IsScolding() && smi.timeinstate > 1f;
	}

	public static bool IsScaldingTimed(ScaldingMonitor.Instance smi)
	{
		return smi.IsScalding() && smi.timeinstate > 1f;
	}

	public static bool IsScalding(ScaldingMonitor.Instance smi)
	{
		return smi.IsScalding();
	}

	public static bool IsScolding(ScaldingMonitor.Instance smi)
	{
		return smi.IsScolding();
	}

	public static bool IsScoldingTimed(ScaldingMonitor.Instance smi)
	{
		return smi.IsScolding() && smi.timeinstate > 1f;
	}

	public static void TakeScaldDamage(ScaldingMonitor.Instance smi, float dt)
	{
		smi.TemperatureDamage(dt);
	}

	public static void TakeColdDamage(ScaldingMonitor.Instance smi, float dt)
	{
		smi.TemperatureDamage(dt);
	}

	public static void AverageExternalTemperatureUpdate(ScaldingMonitor.Instance smi, float dt)
	{
		smi.AverageExternalTemperature *= Mathf.Max(0f, 1f - dt / 6f);
		smi.AverageExternalTemperature += smi.GetCurrentExternalTemperature() * (dt / 6f);
	}

	private const float TRANSITION_TO_DELAY = 1f;

	private const float TEMPERATURE_AVERAGING_RANGE = 6f;

	private const float MIN_SCALD_INTERVAL = 5f;

	private const float SCALDING_DAMAGE_AMOUNT = 10f;

	public GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.State idle;

	public GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.State transitionToScalding;

	public GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.State transitionToScolding;

	public GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.State scalding;

	public GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.State scolding;

	public class Def : StateMachine.BaseDef
	{
		public float defaultScaldingTreshold = 345f;

		public float defaultScoldingTreshold = 183f;
	}

	public new class Instance : GameStateMachine<ScaldingMonitor, ScaldingMonitor.Instance, IStateMachineTarget, ScaldingMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ScaldingMonitor.Def def)
			: base(master, def)
		{
			this.internalTemperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			this.baseScalindingThreshold = new AttributeModifier("ScaldingThreshold", def.defaultScaldingTreshold, DUPLICANTS.STATS.SKIN_DURABILITY.NAME, false, false, true);
			this.baseScoldingThreshold = new AttributeModifier("ScoldingThreshold", def.defaultScoldingTreshold, DUPLICANTS.STATS.SKIN_DURABILITY.NAME, false, false, true);
			this.attributes = base.gameObject.GetAttributes();
		}

		public override void StartSM()
		{
			base.smi.attributes.Get(Db.Get().Attributes.ScaldingThreshold).Add(this.baseScalindingThreshold);
			base.smi.attributes.Get(Db.Get().Attributes.ScoldingThreshold).Add(this.baseScoldingThreshold);
			base.StartSM();
		}

		public bool IsScalding()
		{
			int num = Grid.PosToCell(base.gameObject);
			return Grid.IsValidCell(num) && Grid.Element[num].id != SimHashes.Vacuum && Grid.Element[num].id != SimHashes.Void && this.AverageExternalTemperature > this.GetScaldingThreshold();
		}

		public float GetScaldingThreshold()
		{
			return base.smi.attributes.GetValue("ScaldingThreshold");
		}

		public bool IsScolding()
		{
			int num = Grid.PosToCell(base.gameObject);
			return Grid.IsValidCell(num) && Grid.Element[num].id != SimHashes.Vacuum && Grid.Element[num].id != SimHashes.Void && this.AverageExternalTemperature < this.GetScoldingThreshold();
		}

		public float GetScoldingThreshold()
		{
			return base.smi.attributes.GetValue("ScoldingThreshold");
		}

		public void TemperatureDamage(float dt)
		{
			if (this.health != null && Time.time - this.lastScaldTime > 5f)
			{
				this.lastScaldTime = Time.time;
				this.health.Damage(dt * 10f);
			}
		}

		public float GetCurrentExternalTemperature()
		{
			int num = Grid.PosToCell(base.gameObject);
			if (this.occupyArea != null)
			{
				float num2 = 0f;
				int num3 = 0;
				for (int i = 0; i < this.occupyArea.OccupiedCellsOffsets.Length; i++)
				{
					int num4 = Grid.OffsetCell(num, this.occupyArea.OccupiedCellsOffsets[i]);
					if (Grid.IsValidCell(num4))
					{
						bool flag = Grid.Element[num4].id == SimHashes.Vacuum || Grid.Element[num4].id == SimHashes.Void;
						num3++;
						num2 += (flag ? this.internalTemperature.value : Grid.Temperature[num4]);
					}
				}
				return num2 / (float)Mathf.Max(1, num3);
			}
			if (Grid.Element[num].id != SimHashes.Vacuum && Grid.Element[num].id != SimHashes.Void)
			{
				return Grid.Temperature[num];
			}
			return this.internalTemperature.value;
		}

		public float AverageExternalTemperature;

		private float lastScaldTime;

		private Attributes attributes;

		[MyCmpGet]
		private Health health;

		[MyCmpGet]
		private OccupyArea occupyArea;

		private AttributeModifier baseScalindingThreshold;

		private AttributeModifier baseScoldingThreshold;

		public AmountInstance internalTemperature;
	}
}
