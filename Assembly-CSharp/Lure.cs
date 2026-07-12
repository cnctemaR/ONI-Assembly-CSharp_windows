using System;

public class Lure : GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.off;
		this.off.DoNothing();
		this.on.Enter(new StateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State.Callback(this.AddToScenePartitioner)).Exit(new StateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State.Callback(this.RemoveFromScenePartitioner));
	}

	private void AddToScenePartitioner(Lure.Instance smi)
	{
		Extents extents = new Extents(smi.cell, smi.def.radius);
		smi.partitionerEntry = GameScenePartitioner.Instance.Add(this.name, smi, extents, GameScenePartitioner.Instance.lure, null);
	}

	private void RemoveFromScenePartitioner(Lure.Instance smi)
	{
		GameScenePartitioner.Instance.Free(ref smi.partitionerEntry);
	}

	public GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State off;

	public GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State on;

	public class Def : StateMachine.BaseDef
	{
		public CellOffset[] defaultLurePoints = new CellOffset[1];

		public int radius = 50;

		public Tag[] initialLures;
	}

	public new class Instance : GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.GameInstance
	{
		public int cell
		{
			get
			{
				if (this._cell == -1)
				{
					this._cell = Grid.PosToCell(base.transform.GetPosition());
				}
				return this._cell;
			}
		}

		public CellOffset[] LurePoints
		{
			get
			{
				if (this._lurePoints == null)
				{
					return base.def.defaultLurePoints;
				}
				return this._lurePoints;
			}
			set
			{
				this._lurePoints = value;
			}
		}

		public Instance(IStateMachineTarget master, Lure.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			if (base.def.initialLures != null)
			{
				this.SetActiveLures(base.def.initialLures);
			}
		}

		public void ChangeLureCellPosition(int newCell)
		{
			bool flag = base.IsInsideState(base.sm.on);
			if (flag)
			{
				this.GoTo(base.sm.off);
			}
			this.LurePoints = new CellOffset[] { Grid.GetOffset(Grid.PosToCell(base.smi.transform.GetPosition()), newCell) };
			this._cell = newCell;
			if (flag)
			{
				this.GoTo(base.sm.on);
			}
		}

		public void SetActiveLures(Tag[] lures)
		{
			this.lures = lures;
			if (lures == null || lures.Length == 0)
			{
				this.GoTo(base.sm.off);
				return;
			}
			this.GoTo(base.sm.on);
		}

		public bool IsActive()
		{
			return this.GetCurrentState() == base.sm.on;
		}

		public bool HasAnyLure(Tag[] creature_lures)
		{
			if (this.lures == null || creature_lures == null)
			{
				return false;
			}
			foreach (Tag tag in creature_lures)
			{
				foreach (Tag tag2 in this.lures)
				{
					if (tag == tag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		private int _cell = -1;

		private Tag[] lures;

		public HandleVector<int>.Handle partitionerEntry;

		private CellOffset[] _lurePoints;
	}
}
