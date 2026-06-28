using System;
using UnityEngine;

public class NarcolepsyChore : Chore<NarcolepsyChore.StatesInstance>
{
	public NarcolepsyChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Narcolepsy, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new NarcolepsyChore.StatesInstance(this, target.gameObject);
		base.AddPrecondition(NarcolepsyChore.IsNarcolepsing, null);
	}

	public static Chore.Precondition IsNarcolepsing = new Chore.Precondition
	{
		id = "IsNarcolepsing",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumer.GetComponent<Narcolepsy>();
			return component != null && component.IsNarcolepsing();
		}
	};

	public class StatesInstance : GameStateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.GameInstance
	{
		public StatesInstance(NarcolepsyChore master, GameObject sleeper)
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
			gameObject.AddComponent<Sleepable>();
			base.sm.locator.Set(gameObject, this);
			this.locatorCell = num;
		}

		public void DestroyLocator()
		{
			Grid.Reserved[this.locatorCell] = false;
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		private KAnimFile GetAnims()
		{
			NavType currentNavType = base.sm.sleeper.Get<Navigator>(base.smi).CurrentNavType;
			string text;
			if (currentNavType != NavType.Ladder)
			{
				if (currentNavType != NavType.Pole)
				{
					text = "anim_sleep_floor_kanim";
				}
				else
				{
					text = "anim_sleep_pole_kanim";
				}
			}
			else
			{
				text = "anim_sleep_ladder_kanim";
			}
			return Assets.GetAnim(text);
		}

		public void ApplyAnims()
		{
			Sleepable sleepable = base.sm.locator.Get<Sleepable>(base.smi);
			if (sleepable != null)
			{
				sleepable.overrideAnims = new KAnimFile[] { this.GetAnims() };
			}
		}

		private int locatorCell;
	}

	public class States : GameStateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.sleeper);
			this.root.Enter("CreateLocator", delegate(NarcolepsyChore.StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(NarcolepsyChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.approach.InitializeStates(this.sleeper, this.locator, this.sleep, null, null, null);
			this.sleep.Enter("ApplyAnims", delegate(NarcolepsyChore.StatesInstance smi)
			{
				smi.ApplyAnims();
			}).DoSleep(this.sleeper, this.locator, this.pst, null).ToggleEffect("NarcolepticSleep")
				.PlayAnim("working_pre")
				.QueueAnim("working_loop", true, null);
			this.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.success);
			this.success.ReturnSuccess();
		}

		public StateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.TargetParameter locator;

		public StateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.TargetParameter sleeper;

		public GameStateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.ApproachSubState<IApproachable> approach;

		public GameStateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.State sleep;

		public GameStateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.State pst;

		public GameStateMachine<NarcolepsyChore.States, NarcolepsyChore.StatesInstance, NarcolepsyChore, object>.State success;
	}
}
