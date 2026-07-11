using System;
using UnityEngine;

public class EggIncubatorStates : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.empty;
		this.empty.PlayAnim("off", KAnim.PlayMode.Loop).EventTransition(GameHashes.OccupantChanged, this.egg, new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.HasEgg)).EventTransition(GameHashes.OccupantChanged, this.baby, new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.HasBaby));
		this.egg.DefaultState(this.egg.unpowered).EventTransition(GameHashes.OccupantChanged, this.empty, GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Not(new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.HasAny))).EventTransition(GameHashes.OccupantChanged, this.baby, new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.HasBaby))
			.ToggleStatusItem(Db.Get().BuildingStatusItems.IncubatorProgress, (EggIncubatorStates.Instance smi) => smi.master.GetComponent<EggIncubator>());
		this.egg.lose_power.PlayAnim("no_power_pre").EventTransition(GameHashes.OperationalChanged, this.egg.incubating, new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.IsOperational)).OnAnimQueueComplete(this.egg.unpowered);
		this.egg.unpowered.PlayAnim("no_power_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.egg.incubating, new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.IsOperational));
		this.egg.incubating.PlayAnim("no_power_pst").QueueAnim("working_loop", true, null).EventTransition(GameHashes.OperationalChanged, this.egg.lose_power, GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Not(new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.IsOperational)));
		this.baby.DefaultState(this.baby.idle).EventTransition(GameHashes.OccupantChanged, this.empty, GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Not(new StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(EggIncubatorStates.HasBaby)));
		this.baby.idle.PlayAnim("no_power_pre").QueueAnim("no_power_loop", true, null);
	}

	public static bool IsOperational(EggIncubatorStates.Instance smi)
	{
		return smi.GetComponent<Operational>().IsOperational;
	}

	public static bool HasEgg(EggIncubatorStates.Instance smi)
	{
		GameObject occupant = smi.GetComponent<EggIncubator>().Occupant;
		return occupant && occupant.HasTag(GameTags.Egg);
	}

	public static bool HasBaby(EggIncubatorStates.Instance smi)
	{
		GameObject occupant = smi.GetComponent<EggIncubator>().Occupant;
		return occupant && occupant.HasTag(GameTags.Creature);
	}

	public static bool HasAny(EggIncubatorStates.Instance smi)
	{
		return smi.GetComponent<EggIncubator>().Occupant;
	}

	public StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.BoolParameter readyToHatch;

	public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State empty;

	public EggIncubatorStates.EggStates egg;

	public EggIncubatorStates.BabyStates baby;

	public class EggStates : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State incubating;

		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State lose_power;

		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State unpowered;
	}

	public class BabyStates : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State idle;
	}

	public new class Instance : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
