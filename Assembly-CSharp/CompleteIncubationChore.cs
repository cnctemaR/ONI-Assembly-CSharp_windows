using System;

public class CompleteIncubationChore : Chore<CompleteIncubationChore.States.Instance>
{
	public CompleteIncubationChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.CreatureHatch, master, null, true, null, null, null, PriorityScreen.PriorityClass.basic, 0, false, true, 0, null)
	{
		this.smi = new CompleteIncubationChore.States.Instance(master);
		Workable workable = base.target as Workable;
		base.AddPrecondition(ChorePreconditions.instance.IsNotTransferArm, null);
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, workable);
		base.AddPrecondition(ChorePreconditions.instance.IsOperational, base.target.GetComponent<Operational>());
		base.AddPrecondition(ChorePreconditions.instance.HasRolePerk, workable.requiredRolePerk);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.rancher.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public class States : GameStateMachine<CompleteIncubationChore.States, CompleteIncubationChore.States.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.moveto;
			base.Target(this.rancher);
			this.moveto.MoveTo(new Func<CompleteIncubationChore.States.Instance, int>(Grid.PosToCell), this.hatch, null, false);
			this.hatch.Enter("FaceEgg", delegate(CompleteIncubationChore.States.Instance smi)
			{
				this.rancher.Get<Facing>(smi).Face(smi.transform.GetPosition().x + 1f);
			}).ToggleAnims("anim_interacts_incubator_kanim", 0f).PlayAnim("hatching")
				.OnAnimQueueComplete(this.complete)
				.Enter("StartHatch", delegate(CompleteIncubationChore.States.Instance smi)
				{
					smi.incubator.StartHatch();
				});
			this.complete.ReturnSuccess();
		}

		public StateMachine<CompleteIncubationChore.States, CompleteIncubationChore.States.Instance, IStateMachineTarget, object>.TargetParameter rancher;

		private GameStateMachine<CompleteIncubationChore.States, CompleteIncubationChore.States.Instance, IStateMachineTarget, object>.State moveto;

		private GameStateMachine<CompleteIncubationChore.States, CompleteIncubationChore.States.Instance, IStateMachineTarget, object>.State hatch;

		private GameStateMachine<CompleteIncubationChore.States, CompleteIncubationChore.States.Instance, IStateMachineTarget, object>.State complete;

		public new class Instance : GameStateMachine<CompleteIncubationChore.States, CompleteIncubationChore.States.Instance, IStateMachineTarget, object>.GameInstance
		{
			public Instance(IStateMachineTarget master)
				: base(master)
			{
				this.incubator = (EggIncubator)master;
			}

			public EggIncubator incubator;
		}
	}
}
