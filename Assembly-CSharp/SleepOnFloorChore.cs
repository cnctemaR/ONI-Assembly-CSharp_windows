using System;
using UnityEngine;

public class SleepOnFloorChore : Chore<SleepOnFloorChore.StatesInstance>
{
	public SleepOnFloorChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.SleepOnFloor, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new SleepOnFloorChore.StatesInstance(this, target.gameObject);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
		base.AddPrecondition(SleepOnFloorChore.IsNarcolepsingOrIsSleepTime, null);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static SleepOnFloorChore()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsNarcolepsingOrIsSleepTime";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumer.GetComponent<Narcolepsy>();
			bool flag = component != null && component.IsNarcolepsing();
			bool flag2 = ChorePreconditions.IsScheduledTime.fn(ref context, Db.Get().ScheduleBlockTypes.Sleep);
			return flag || flag2;
		};
		SleepOnFloorChore.IsNarcolepsingOrIsSleepTime = precondition;
	}

	public static Chore.Precondition IsNarcolepsingOrIsSleepTime;

	public class StatesInstance : GameStateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore, object>.GameInstance
	{
		public StatesInstance(SleepOnFloorChore master, GameObject sleeper)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
		}

		public void CreateLocator()
		{
			int num = base.sm.sleeper.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>().GetCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.sleeper.Get<Transform>(base.smi).position);
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			Grid.Reserved[num] = true;
			GameObject gameObject = ChoreHelpers.CreateLocator("SleepLocator", vector);
			gameObject.AddComponent<Restable>();
			base.sm.locator.Set(gameObject, this);
			this.locatorCell = num;
		}

		public void DestroyLocator()
		{
			Grid.Reserved[this.locatorCell] = false;
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		private int locatorCell;
	}

	public class States : GameStateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.sleeper);
			this.root.Enter("CreateLocator", delegate(SleepOnFloorChore.StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(SleepOnFloorChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.approach.InitializeStates(this.sleeper, this.locator, this.sleep, null, null, null);
			this.sleep.Use<Restable>(this.locator, this.success, null);
			this.success.ReturnSuccess();
		}

		public StateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore, object>.TargetParameter locator;

		public StateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore, object>.TargetParameter sleeper;

		public GameStateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore, object>.ApproachSubState<Approachable> approach;

		public GameStateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore, object>.State sleep;

		public GameStateMachine<SleepOnFloorChore.States, SleepOnFloorChore.StatesInstance, SleepOnFloorChore, object>.State success;
	}
}
