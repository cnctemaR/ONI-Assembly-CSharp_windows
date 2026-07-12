using System;
using UnityEngine;

public class AutoStorageDropper : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventTransition(GameHashes.OnStorageChange, this.pre_drop, null).ParamTransition<bool>(this.isBlocked, this.blocked, GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.IsTrue);
		this.pre_drop.ScheduleGoTo(0f, this.dropping);
		this.dropping.Enter(delegate(AutoStorageDropper.Instance smi)
		{
			smi.Drop();
		}).GoTo(this.idle);
		this.blocked.ParamTransition<bool>(this.isBlocked, this.pre_drop, GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked, null);
	}

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State idle;

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State pre_drop;

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State dropping;

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State blocked;

	private StateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.BoolParameter isBlocked;

	public class Def : StateMachine.BaseDef
	{
		public CellOffset dropOffset;

		public bool asOre;

		public SimHashes[] elementFilter;

		public bool invertElementFilter;

		public bool blockedBySubstantialLiquid;
	}

	public new class Instance : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, AutoStorageDropper.Def def)
			: base(master, def)
		{
			this.ScheduleNextFrame(new Action<object>(this.RegisterListeners), null);
		}

		private void RegisterListeners(object obj)
		{
			int num = Grid.PosToCell(base.smi.GetDropPosition());
			if (Grid.IsValidCell(num))
			{
				Extents extents = new Extents(num, new CellOffset[]
				{
					new CellOffset(0, 0)
				});
				this.partitionerEntrySolid = GameScenePartitioner.Instance.Add("AutoStorageDropper.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnOutpuTileChanged));
				if (base.def.blockedBySubstantialLiquid)
				{
					this.partitionerEntryLiquid = GameScenePartitioner.Instance.Add("AutoStorageDropper.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnOutpuTileChanged));
				}
				this.OnOutpuTileChanged(null);
			}
		}

		protected override void OnCleanUp()
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntrySolid);
			GameScenePartitioner.Instance.Free(ref this.partitionerEntryLiquid);
		}

		private void OnOutpuTileChanged(object data)
		{
			int num = Grid.PosToCell(base.smi.GetDropPosition());
			bool flag = Grid.IsSolidCell(num) || (base.def.blockedBySubstantialLiquid && Grid.IsLiquid(num));
			base.sm.isBlocked.Set(flag, base.smi);
		}

		private bool IsFilteredElement(SimHashes element)
		{
			for (int num = 0; num != base.def.elementFilter.Length; num++)
			{
				if (base.def.elementFilter[num] == element)
				{
					return true;
				}
			}
			return false;
		}

		private bool AllowedToDrop(SimHashes element)
		{
			return base.def.elementFilter == null || base.def.elementFilter.Length == 0 || (!base.def.invertElementFilter && this.IsFilteredElement(element)) || (base.def.invertElementFilter && !this.IsFilteredElement(element));
		}

		public void Drop()
		{
			for (int i = this.m_storage.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = this.m_storage.items[i];
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (this.AllowedToDrop(component.ElementID))
				{
					if (base.def.asOre)
					{
						this.m_storage.Drop(gameObject, true);
						gameObject.transform.SetPosition(this.GetDropPosition());
					}
					else
					{
						Dumpable component2 = gameObject.GetComponent<Dumpable>();
						if (!component2.IsNullOrDestroyed())
						{
							component2.Dump(this.GetDropPosition());
						}
					}
				}
			}
		}

		public Vector3 GetDropPosition()
		{
			if (!(this.m_rotatable != null))
			{
				return base.transform.GetPosition() + base.def.dropOffset.ToVector3();
			}
			return base.transform.GetPosition() + this.m_rotatable.GetRotatedCellOffset(base.def.dropOffset).ToVector3();
		}

		[MyCmpGet]
		private Storage m_storage;

		[MyCmpGet]
		private Rotatable m_rotatable;

		private HandleVector<int>.Handle partitionerEntrySolid;

		private HandleVector<int>.Handle partitionerEntryLiquid;
	}
}
