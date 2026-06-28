using System;
using UnityEngine;

public class IdleChore : Chore<IdleChore.StatesInstance>
{
	public IdleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Idle, target, target.GetComponent<ChoreProvider>(), false, null, null, null, -1, false, true, 0)
	{
		this.smi = new IdleChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.GameInstance
	{
		public StatesInstance(IdleChore master, GameObject idler)
			: base(master)
		{
			base.sm.idler.Set(idler, base.smi);
			this.idleCellSensor = base.GetComponent<Sensors>().GetSensor<IdleCellSensor>();
		}

		public void UpdateIsOnLadder()
		{
			base.sm.isOnLadder.Set(base.GetComponent<Navigator>().CurrentNavType == NavType.Ladder, this);
		}

		public int GetIdleCell()
		{
			return this.idleCellSensor.GetCell();
		}

		public bool HasIdleCell()
		{
			return this.idleCellSensor.GetCell() != Grid.InvalidCell;
		}

		private IdleCellSensor idleCellSensor;
	}

	public class States : GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.Target(this.idler);
			this.idle.DefaultState(this.idle.onfloor).Enter("UpdateIsOnLadder", delegate(IdleChore.StatesInstance smi)
			{
				smi.UpdateIsOnLadder();
			}).Update("UpdateIsOnLadder", delegate(IdleChore.StatesInstance smi)
			{
				smi.UpdateIsOnLadder();
			})
				.ToggleSchedulePeriodic("Log idle time", 1f, delegate(IdleChore.StatesInstance smi)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.IdleTime, 1f, this.idler.Get(smi).GetProperName(), null);
				})
				.ToggleStateMachine((IdleChore.StatesInstance smi) => new TaskAvailabilityMonitor.Instance(smi.master));
			this.idle.onfloor.PlayAnim("idle_default", KAnim.PlayMode.Loop, null).ParamTransition<bool>(this.isOnLadder, this.idle.onladder, (IdleChore.StatesInstance smi, bool p) => p).ToggleSchedulePeriodic("IdleMove", (IdleChore.StatesInstance smi) => (float)global::UnityEngine.Random.Range(5, 15), delegate(IdleChore.StatesInstance smi)
			{
				if (smi.HasIdleCell())
				{
					smi.GoTo(this.idle.move);
				}
			}, null);
			this.idle.onladder.PlayAnim("ladder_idle", KAnim.PlayMode.Loop, null).ParamTransition<bool>(this.isOnLadder, this.idle.onfloor, (IdleChore.StatesInstance smi, bool p) => !p).ToggleSchedulePeriodic("IdleMove", (IdleChore.StatesInstance smi) => (float)global::UnityEngine.Random.Range(5, 15), delegate(IdleChore.StatesInstance smi)
			{
				if (smi.HasIdleCell())
				{
					smi.GoTo(this.idle.move);
				}
			}, null);
			this.idle.move.TriggerOnEnter(GameHashes.BeginWalk, null).TriggerOnExit(GameHashes.EndWalk).ToggleAnims("anim_loco_walk_kanim", 0f)
				.MoveTo((IdleChore.StatesInstance smi) => smi.GetIdleCell(), this.idle, this.idle, false);
		}

		public StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.BoolParameter isOnLadder;

		public StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.TargetParameter idler;

		public IdleChore.States.IdleState idle;

		public class IdleState : GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State
		{
			public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State onfloor;

			public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State onladder;

			public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State move;
		}
	}
}
