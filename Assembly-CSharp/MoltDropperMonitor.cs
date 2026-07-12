using System;
using KSerialization;
using UnityEngine;

public class MoltDropperMonitor : GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.EventHandler(GameHashes.NewDay, (MoltDropperMonitor.Instance smi) => GameClock.Instance, delegate(MoltDropperMonitor.Instance smi)
		{
			smi.spawnedThisCycle = false;
		});
		this.satisfied.OnSignal(this.cellChangedSignal, this.drop, (MoltDropperMonitor.Instance smi) => smi.ShouldDropElement());
		this.drop.Enter(delegate(MoltDropperMonitor.Instance smi)
		{
			smi.Drop();
		}).EventTransition(GameHashes.NewDay, (MoltDropperMonitor.Instance smi) => GameClock.Instance, this.satisfied, null);
	}

	public StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.BoolParameter droppedThisCycle = new StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.BoolParameter(false);

	public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State satisfied;

	public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State drop;

	public StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.Signal cellChangedSignal;

	public class Def : StateMachine.BaseDef
	{
		public string onGrowDropID;

		public float massToDrop;

		public SimHashes blockedElement;
	}

	public new class Instance : GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MoltDropperMonitor.Def def)
			: base(master, def)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "ElementDropperMonitor.Instance");
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		}

		private void OnCellChange()
		{
			base.sm.cellChangedSignal.Trigger(this);
		}

		public bool ShouldDropElement()
		{
			return this.IsValidTimeToDrop() && !base.smi.HasTag(GameTags.Creatures.Hungry) && base.smi.HasTag(GameTags.Creatures.Happy) && this.IsValidDropCell();
		}

		public void Drop()
		{
			GameObject gameObject = Scenario.SpawnPrefab(this.GetDropSpawnLocation(), 0, 0, base.def.onGrowDropID, Grid.SceneLayer.Ore);
			gameObject.SetActive(true);
			gameObject.GetComponent<PrimaryElement>().Mass = base.def.massToDrop;
			this.spawnedThisCycle = true;
			this.timeOfLastDrop = GameClock.Instance.GetTime();
		}

		private int GetDropSpawnLocation()
		{
			int num = Grid.PosToCell(base.gameObject);
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				return num2;
			}
			return num;
		}

		public bool IsValidTimeToDrop()
		{
			return !this.spawnedThisCycle && (this.timeOfLastDrop <= 0f || GameClock.Instance.GetTime() - this.timeOfLastDrop > 600f);
		}

		public bool IsValidDropCell()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			return Grid.IsValidCell(num) && Grid.Element[num].id != base.def.blockedElement;
		}

		[Serialize]
		public bool spawnedThisCycle;

		[Serialize]
		public float timeOfLastDrop;
	}
}
