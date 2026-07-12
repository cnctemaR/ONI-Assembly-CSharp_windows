using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MournChore : Chore<MournChore.StatesInstance>
{
	private static int GetStandableCell(int cell, Navigator navigator)
	{
		foreach (CellOffset cellOffset in MournChore.ValidStandingOffsets)
		{
			if (Grid.IsCellOffsetValid(cell, cellOffset))
			{
				int num = Grid.OffsetCell(cell, cellOffset);
				if (!Grid.Reserved[num] && navigator.NavGrid.NavTable.IsValid(num, NavType.Floor) && navigator.GetNavigationCost(num) != -1)
				{
					return num;
				}
			}
		}
		return -1;
	}

	public MournChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Mourn, master, master.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.high, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new MournChore.StatesInstance(this);
		base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		base.AddPrecondition(ChorePreconditions.instance.NoDeadBodies, null);
		base.AddPrecondition(MournChore.HasValidMournLocation, master);
	}

	public static Grave FindGraveToMournAt()
	{
		Grave grave = null;
		float num = -1f;
		foreach (object obj in Components.Graves)
		{
			Grave grave2 = (Grave)obj;
			if (grave2.burialTime > num)
			{
				num = grave2.burialTime;
				grave = grave2;
			}
		}
		return grave;
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("MournChore null context.consumer");
			return;
		}
		if (base.smi == null)
		{
			global::Debug.LogError("MournChore null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			global::Debug.LogError("MournChore null smi.sm");
			return;
		}
		if (MournChore.FindGraveToMournAt() == null)
		{
			global::Debug.LogError("MournChore no grave");
			return;
		}
		base.smi.sm.mourner.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}

	private static readonly CellOffset[] ValidStandingOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private static readonly Chore.Precondition HasValidMournLocation = new Chore.Precondition
	{
		id = "HasPlaceToStand",
		description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_PLACE_TO_STAND,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Navigator component = ((IStateMachineTarget)data).GetComponent<Navigator>();
			bool flag = false;
			Grave grave = MournChore.FindGraveToMournAt();
			if (grave != null && Grid.IsValidCell(MournChore.GetStandableCell(Grid.PosToCell(grave), component)))
			{
				flag = true;
			}
			return flag;
		}
	};

	public class StatesInstance : GameStateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.GameInstance
	{
		public StatesInstance(MournChore master)
			: base(master)
		{
		}

		public void CreateLocator()
		{
			int num = Grid.PosToCell(MournChore.FindGraveToMournAt().transform.GetPosition());
			Navigator component = base.master.GetComponent<Navigator>();
			int standableCell = MournChore.GetStandableCell(num, component);
			if (standableCell < 0)
			{
				base.smi.GoTo(null);
				return;
			}
			Grid.Reserved[standableCell] = true;
			Vector3 vector = Grid.CellToPosCBC(standableCell, Grid.SceneLayer.Move);
			GameObject gameObject = ChoreHelpers.CreateLocator("MournLocator", vector);
			base.smi.sm.locator.Set(gameObject, base.smi);
			this.locatorCell = standableCell;
			base.smi.GoTo(base.sm.moveto);
		}

		public void DestroyLocator()
		{
			if (this.locatorCell >= 0)
			{
				Grid.Reserved[this.locatorCell] = false;
				ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
				base.sm.locator.Set(null, this);
				this.locatorCell = -1;
			}
		}

		private int locatorCell = -1;
	}

	public class States : GameStateMachine<MournChore.States, MournChore.StatesInstance, MournChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.findOffset;
			base.Target(this.mourner);
			this.root.ToggleAnims("anim_react_mourning_kanim", 0f, "").Exit("DestroyLocator", delegate(MournChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.findOffset.Enter("CreateLocator", delegate(MournChore.StatesInstance smi)
			{
				smi.CreateLocator();
			});
			this.moveto.InitializeStates(this.mourner, this.locator, this.mourn, null, null, null);
			this.mourn.PlayAnims((MournChore.StatesInstance smi) => MournChore.States.WORK_ANIMS, KAnim.PlayMode.Loop).ScheduleGoTo(10f, this.completed);
			this.completed.PlayAnim("working_pst").OnAnimQueueComplete(null).Exit(delegate(MournChore.StatesInstance smi)
			{
				this.mourner.Get<Effects>(smi).Remove(Db.Get().effects.Get("Mourning"));
			});
		}

		public StateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.TargetParameter mourner;

		public StateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.TargetParameter locator;

		public GameStateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.State findOffset;

		public GameStateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.ApproachSubState<IApproachable> moveto;

		public GameStateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.State mourn;

		public GameStateMachine<MournChore.States, MournChore.StatesInstance, MournChore, object>.State completed;

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "working_pre", "working_loop" };
	}
}
