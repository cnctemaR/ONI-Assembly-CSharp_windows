using System;
using STRINGS;

public class GrowUpStates : GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grow_up_pre;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.GROWINGUP.NAME, CREATURES.STATUSITEMS.GROWINGUP.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.grow_up_pre.QueueAnim("growup_pre", false, null).OnAnimQueueComplete(this.spawn_adult);
		this.spawn_adult.Enter(new StateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>.State.Callback(GrowUpStates.SpawnAdult));
	}

	private static void SpawnAdult(GrowUpStates.Instance smi)
	{
		smi.GetSMI<BabyMonitor.Instance>().SpawnAdult();
	}

	public GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>.State grow_up_pre;

	public GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>.State spawn_adult;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>.GameInstance
	{
		public Instance(Chore<GrowUpStates.Instance> chore, GrowUpStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.GrowUpBehaviour);
		}
	}
}
