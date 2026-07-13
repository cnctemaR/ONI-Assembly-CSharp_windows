using System;
using UnityEngine;

public class AquaticCreatureSuffocationMonitor : GameStateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.safe;
		this.root.TagTransition(GameTags.Dead, this.dead, false);
		this.safe.Transition(this.suffocating, new StateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.Transition.ConditionCallback(AquaticCreatureSuffocationMonitor.IsSuffocating), UpdateRate.SIM_1000ms).Update(new Action<AquaticCreatureSuffocationMonitor.Instance, float>(AquaticCreatureSuffocationMonitor.RecoveryDeathTimerUpdate), UpdateRate.SIM_200ms, false);
		this.suffocating.ParamTransition<float>(this.DeathTimer, this.die, new StateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.Parameter<float>.Callback(AquaticCreatureSuffocationMonitor.CanNotHoldAnymore)).Transition(this.safe, new StateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.Transition.ConditionCallback(AquaticCreatureSuffocationMonitor.CanBreath), UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.AquaticCreatureSuffocating, null)
			.Update(new Action<AquaticCreatureSuffocationMonitor.Instance, float>(AquaticCreatureSuffocationMonitor.DeathTimerUpdate), UpdateRate.SIM_200ms, false);
		this.die.Enter(new StateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.State.Callback(AquaticCreatureSuffocationMonitor.Kill));
		this.dead.DoNothing();
	}

	public static bool IsSuffocating(AquaticCreatureSuffocationMonitor.Instance smi)
	{
		return !smi.CanBreath();
	}

	public static bool CanBreath(AquaticCreatureSuffocationMonitor.Instance smi)
	{
		return smi.CanBreath();
	}

	public static bool CanNotHoldAnymore(AquaticCreatureSuffocationMonitor.Instance smi, float deathTimerValue)
	{
		return deathTimerValue > smi.def.DeathTimerDuration;
	}

	public static void DeathTimerUpdate(AquaticCreatureSuffocationMonitor.Instance smi, float dt)
	{
		smi.sm.DeathTimer.Set(smi.DeathTimerValue + dt, smi, false);
	}

	public static void RecoveryDeathTimerUpdate(AquaticCreatureSuffocationMonitor.Instance smi, float dt)
	{
		if (smi.DeathTimerValue > 0f)
		{
			smi.sm.DeathTimer.Set(Mathf.Max(smi.DeathTimerValue - dt * smi.def.RecoveryModifier, 0f), smi, false);
		}
	}

	public static void Kill(AquaticCreatureSuffocationMonitor.Instance smi)
	{
		smi.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
	}

	public GameStateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.State safe;

	public GameStateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.State suffocating;

	public GameStateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.State die;

	public GameStateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.State dead;

	public StateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.FloatParameter DeathTimer;

	public class Def : StateMachine.BaseDef
	{
		public float DeathTimerDuration = 2400f;

		public float RecoveryModifier = 4f;
	}

	public new class Instance : GameStateMachine<AquaticCreatureSuffocationMonitor, AquaticCreatureSuffocationMonitor.Instance, IStateMachineTarget, AquaticCreatureSuffocationMonitor.Def>.GameInstance
	{
		public float DeathTimerValue
		{
			get
			{
				return base.sm.DeathTimer.Get(this);
			}
		}

		public float TimeUntilDeath
		{
			get
			{
				return Mathf.Max(base.smi.def.DeathTimerDuration - this.DeathTimerValue, 0f);
			}
		}

		public Instance(IStateMachineTarget master, AquaticCreatureSuffocationMonitor.Def def)
			: base(master, def)
		{
			this.pickupable = base.GetComponent<Pickupable>();
		}

		public bool CanBreath()
		{
			int num = Grid.PosToCell(this);
			return !(this.pickupable.storage == null) || Grid.IsSubstantialLiquid(num, 0.35f);
		}

		private Pickupable pickupable;
	}
}
