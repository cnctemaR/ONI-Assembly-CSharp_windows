using System;

public class VisibilityMonitor : GameStateMachine<VisibilityMonitor, VisibilityMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.revealed;
		base.serializable = false;
		this.root.Enter("AddObject", delegate(VisibilityMonitor.Instance smi)
		{
			smi.AddObject();
		}).Exit("RemoveObject", delegate(VisibilityMonitor.Instance smi)
		{
			smi.RemoveObject();
		});
		this.revealed.DefaultState(this.revealed.visible).ParamTransition<bool>(this.isHidden, this.hidden, (VisibilityMonitor.Instance smi, bool p) => p).Enter("AddCullable", delegate(VisibilityMonitor.Instance smi)
		{
			smi.AddCullable();
		})
			.Exit("RemoveCullable", delegate(VisibilityMonitor.Instance smi)
			{
				smi.RemoveCullable();
			});
		this.revealed.visible.DoNothing();
		this.revealed.culled.DoNothing();
		this.hidden.ParamTransition<bool>(this.isHidden, this.revealed, (VisibilityMonitor.Instance smi, bool p) => !p).Enter("RegisterFogOfWarListener", delegate(VisibilityMonitor.Instance smi)
		{
			smi.RegisterFogOfWarListener();
		}).Exit("UnregisterFogOfWarListener", delegate(VisibilityMonitor.Instance smi)
		{
			smi.UnregisterFogOfWarListener();
		});
	}

	public VisibilityMonitor.RevealedState revealed;

	public GameStateMachine<VisibilityMonitor, VisibilityMonitor.Instance, IStateMachineTarget, object>.State hidden;

	public StateMachine<VisibilityMonitor, VisibilityMonitor.Instance, IStateMachineTarget, object>.BoolParameter isHidden;

	public class RevealedState : GameStateMachine<VisibilityMonitor, VisibilityMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<VisibilityMonitor, VisibilityMonitor.Instance, IStateMachineTarget, object>.State visible;

		public GameStateMachine<VisibilityMonitor, VisibilityMonitor.Instance, IStateMachineTarget, object>.State culled;
	}

	public new class Instance : GameStateMachine<VisibilityMonitor, VisibilityMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.RefreshHidden();
		}

		public void AddObject()
		{
			SceneCuller.Instance.AddObject(this);
		}

		public void RemoveObject()
		{
			SceneCuller.Instance.RemoveObject(this);
		}

		public void AddCullable()
		{
			SceneCuller.Instance.AddCullable(this);
		}

		public void RemoveCullable()
		{
			SceneCuller.Instance.RemoveCullable(this);
		}

		private void OnFogOfWarChanged(object data)
		{
			this.RefreshHidden();
		}

		private void RefreshHidden()
		{
			int num = Grid.PosToCell(base.gameObject.transform.position);
			for (int i = 0; i < this.offsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, this.offsets[i]);
				if (Grid.Visible[num2] >= 255)
				{
					base.sm.isHidden.Set(false, base.smi);
					return;
				}
			}
			base.sm.isHidden.Set(true, base.smi);
		}

		public void RegisterFogOfWarListener()
		{
			int num = Grid.PosToCell(base.gameObject.transform.position);
			Extents extents = new Extents(num, this.offsets);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("VisiblityMonitor.RegisterFogOfWarListener", base.gameObject, extents, GameScenePartitioner.Instance.fogOfWarChanged.mask, new Action<object>(this.OnFogOfWarChanged));
			Game.Instance.Trigger(40674218, base.gameObject);
			CellChangeMonitor.Instance.Add(base.gameObject.transform, new Action<int, int>(this.OnCellChange), false);
			SceneCuller.Instance.AddHidden(this);
		}

		public void UnregisterFogOfWarListener()
		{
			if (base.gameObject != null)
			{
				Game.Instance.Trigger(1357164944, base.gameObject);
			}
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
			CellChangeMonitor.Instance.Remove(base.gameObject.transform, new Action<int, int>(this.OnCellChange), false);
			SceneCuller.Instance.RemoveHidden(this);
		}

		private void OnCellChange(int previous_cell, int new_cell)
		{
			if (this.partitionerEntry != null)
			{
				this.partitionerEntry.UpdatePosition(new_cell);
			}
			this.RefreshHidden();
		}

		private CellOffset[] offsets = new CellOffset[]
		{
			new CellOffset(0, 0)
		};

		private GameScenePartitionerEntry partitionerEntry;
	}
}
