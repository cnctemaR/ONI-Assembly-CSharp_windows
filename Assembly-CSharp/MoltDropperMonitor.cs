using System;
using Klei.AI;
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
		this.satisfied.UpdateTransition(this.drop, (MoltDropperMonitor.Instance smi, float dt) => smi.ShouldDropElement(), UpdateRate.SIM_4000ms, false);
		this.drop.DefaultState(this.drop.dropping);
		this.drop.dropping.EnterTransition(this.drop.complete, (MoltDropperMonitor.Instance smi) => !smi.def.synchWithBehaviour).ToggleBehaviour(GameTags.Creatures.ReadyToMolt, (MoltDropperMonitor.Instance smi) => true, delegate(MoltDropperMonitor.Instance smi)
		{
			smi.GoTo(this.drop.complete);
		});
		this.drop.complete.Enter(delegate(MoltDropperMonitor.Instance smi)
		{
			smi.Drop();
		}).TriggerOnEnter(GameHashes.Molt, null).EventTransition(GameHashes.NewDay, (MoltDropperMonitor.Instance smi) => GameClock.Instance, this.satisfied, null);
	}

	public StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.BoolParameter droppedThisCycle = new StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.BoolParameter(false);

	public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State satisfied;

	public MoltDropperMonitor.DropStates drop;

	public class Def : StateMachine.BaseDef
	{
		public bool synchWithBehaviour;

		public string onGrowDropID;

		public float massToDrop;

		public string amountName;

		public Func<MoltDropperMonitor.Instance, bool> isReadyToMolt;
	}

	public class DropStates : GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State
	{
		public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State dropping;

		public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State complete;
	}

	public new class Instance : GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MoltDropperMonitor.Def def)
			: base(master, def)
		{
			if (!string.IsNullOrEmpty(def.amountName))
			{
				AmountInstance amountInstance = Db.Get().Amounts.Get(def.amountName).Lookup(base.smi.gameObject);
				amountInstance.OnMaxValueReached = (global::System.Action)Delegate.Combine(amountInstance.OnMaxValueReached, new global::System.Action(this.OnAmountMaxValueReached));
			}
		}

		private void OnAmountMaxValueReached()
		{
			this.lastTineAmountReachedMax = GameClock.Instance.GetTime();
		}

		protected override void OnCleanUp()
		{
			if (!string.IsNullOrEmpty(base.def.amountName))
			{
				AmountInstance amountInstance = Db.Get().Amounts.Get(base.def.amountName).Lookup(base.smi.gameObject);
				amountInstance.OnMaxValueReached = (global::System.Action)Delegate.Remove(amountInstance.OnMaxValueReached, new global::System.Action(this.OnAmountMaxValueReached));
			}
			base.OnCleanUp();
		}

		public bool ShouldDropElement()
		{
			return base.def.isReadyToMolt(this);
		}

		public void Drop()
		{
			GameObject gameObject = Scenario.SpawnPrefab(this.GetDropSpawnLocation(), 0, 0, base.def.onGrowDropID, Grid.SceneLayer.Ore);
			gameObject.SetActive(true);
			gameObject.GetComponent<PrimaryElement>().Mass = base.def.massToDrop;
			this.spawnedThisCycle = true;
			this.timeOfLastDrop = GameClock.Instance.GetTime();
			if (!string.IsNullOrEmpty(base.def.amountName))
			{
				AmountInstance amountInstance = Db.Get().Amounts.Get(base.def.amountName).Lookup(base.smi.gameObject);
				amountInstance.value = amountInstance.GetMin();
			}
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

		[MyCmpGet]
		public KPrefabID prefabID;

		[Serialize]
		public bool spawnedThisCycle;

		[Serialize]
		public float timeOfLastDrop;

		[Serialize]
		public float lastTineAmountReachedMax;
	}
}
