using System;
using STRINGS;
using UnityEngine;

public class BeeForageStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.collect.findTarget;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.FORAGINGMATERIAL.NAME, CREATURES.STATUSITEMS.FORAGINGMATERIAL.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.UnreserveTarget)).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.DropAll));
		this.collect.findTarget.Enter(delegate(BeeForageStates.Instance smi)
		{
			BeeForageStates.FindTarget(smi);
			smi.targetHive = smi.master.GetComponent<Bee>().FindHiveInRoom();
			if (smi.targetHive != null)
			{
				if (smi.forageTarget != null)
				{
					BeeForageStates.ReserveTarget(smi);
					smi.GoTo(this.collect.forage.moveToTarget);
					return;
				}
				if (Grid.IsValidCell(smi.targetMiningCell))
				{
					smi.GoTo(this.collect.mine.moveToTarget);
					return;
				}
			}
			smi.GoTo(this.behaviourcomplete);
		});
		this.collect.forage.moveToTarget.MoveTo(new Func<BeeForageStates.Instance, int>(BeeForageStates.GetOreCell), this.collect.forage.pickupTarget, this.behaviourcomplete, false);
		this.collect.forage.pickupTarget.PlayAnim("pickup_pre").Enter(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.PickupComplete)).OnAnimQueueComplete(this.storage.moveToHive);
		this.collect.mine.moveToTarget.MoveTo((BeeForageStates.Instance smi) => smi.targetMiningCell, this.collect.mine.mineTarget, this.behaviourcomplete, false);
		this.collect.mine.mineTarget.PlayAnim("mining_pre").QueueAnim("mining_loop", false, null).QueueAnim("mining_pst", false, null)
			.Enter(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.MineTarget))
			.OnAnimQueueComplete(this.storage.moveToHive);
		this.storage.Enter(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(this.HoldOre)).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(this.DropOre));
		this.storage.moveToHive.Enter(delegate(BeeForageStates.Instance smi)
		{
			if (!smi.targetHive)
			{
				smi.targetHive = smi.master.GetComponent<Bee>().FindHiveInRoom();
			}
			if (!smi.targetHive)
			{
				smi.GoTo(this.storage.dropMaterial);
			}
		}).MoveTo((BeeForageStates.Instance smi) => Grid.OffsetCell(Grid.PosToCell(smi.targetHive.transform.GetPosition()), smi.hiveCellOffset), this.storage.storeMaterial, this.behaviourcomplete, false);
		this.storage.storeMaterial.PlayAnim("deposit").Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.StoreOre)).OnAnimQueueComplete(this.behaviourcomplete.pre);
		this.storage.dropMaterial.Enter(delegate(BeeForageStates.Instance smi)
		{
			smi.GoTo(this.behaviourcomplete);
		}).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.DropAll));
		this.behaviourcomplete.DefaultState(this.behaviourcomplete.pst);
		this.behaviourcomplete.pre.PlayAnim("spawn").OnAnimQueueComplete(this.behaviourcomplete.pst);
		this.behaviourcomplete.pst.BehaviourComplete(GameTags.Creatures.WantsToForage, false);
	}

	private static void FindTarget(BeeForageStates.Instance smi)
	{
		if (BeeForageStates.FindOre(smi))
		{
			return;
		}
		BeeForageStates.FindMineableCell(smi);
	}

	private void HoldOre(BeeForageStates.Instance smi)
	{
		GameObject gameObject = smi.GetComponent<Storage>().FindFirst(smi.def.oreTag);
		if (!gameObject)
		{
			return;
		}
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		KAnim.Build.Symbol symbol = gameObject.GetComponent<KBatchedAnimController>().CurrentAnim.animFile.build.symbols[0];
		component.GetComponent<SymbolOverrideController>().AddSymbolOverride(smi.oreSymbolHash, symbol, 5);
		component.SetSymbolVisiblity(smi.oreSymbolHash, true);
		component.SetSymbolVisiblity(smi.oreLegSymbolHash, true);
		component.SetSymbolVisiblity(smi.noOreLegSymbolHash, false);
	}

	private void DropOre(BeeForageStates.Instance smi)
	{
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity(smi.oreSymbolHash, false);
		component.SetSymbolVisiblity(smi.oreLegSymbolHash, false);
		component.SetSymbolVisiblity(smi.noOreLegSymbolHash, true);
	}

	private static void PickupComplete(BeeForageStates.Instance smi)
	{
		if (!smi.forageTarget)
		{
			global::Debug.LogWarningFormat("PickupComplete forageTarget {0} is null", new object[] { smi.forageTarget });
			return;
		}
		BeeForageStates.UnreserveTarget(smi);
		int num = Grid.PosToCell(smi.forageTarget);
		if (smi.forageTarget_cell != num)
		{
			global::Debug.LogWarningFormat("PickupComplete forageTarget {0} moved {1} != {2}", new object[] { smi.forageTarget, num, smi.forageTarget_cell });
			smi.forageTarget = null;
			return;
		}
		if (smi.forageTarget.HasTag(GameTags.Stored))
		{
			global::Debug.LogWarningFormat("PickupComplete forageTarget {0} was stored by {1}", new object[]
			{
				smi.forageTarget,
				smi.forageTarget.storage
			});
			smi.forageTarget = null;
			return;
		}
		smi.forageTarget = EntitySplitter.Split(smi.forageTarget, 10f, null);
		smi.GetComponent<Storage>().Store(smi.forageTarget.gameObject, false, false, true, false);
	}

	private static void MineTarget(BeeForageStates.Instance smi)
	{
		Storage storage = smi.master.GetComponent<Storage>();
		HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(delegate(Sim.MassConsumedCallback mass_cb_info, object data)
		{
			if (mass_cb_info.mass > 0f)
			{
				storage.AddOre(ElementLoader.elements[(int)mass_cb_info.elemIdx].id, mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount, false, true);
			}
		}, null, "BeetaMine");
		SimMessages.ConsumeMass(smi.cellToMine, Grid.Element[smi.cellToMine].id, smi.def.amountToMine, 1, handle.index);
	}

	private static void StoreOre(BeeForageStates.Instance smi)
	{
		smi.master.GetComponent<Storage>().Transfer(smi.targetHive.GetComponent<Storage>(), false, false);
		smi.forageTarget = null;
		smi.forageTarget_cell = Grid.InvalidCell;
		smi.targetHive = null;
	}

	private static void DropAll(BeeForageStates.Instance smi)
	{
		smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true, null);
	}

	private static bool FindMineableCell(BeeForageStates.Instance smi)
	{
		smi.targetMiningCell = Grid.InvalidCell;
		MineableCellQuery mineableCellQuery = PathFinderQueries.mineableCellQuery.Reset(smi.def.oreTag, 20);
		smi.GetComponent<Navigator>().RunQuery(mineableCellQuery);
		if (mineableCellQuery.result_cells.Count > 0)
		{
			smi.targetMiningCell = mineableCellQuery.result_cells[global::UnityEngine.Random.Range(0, mineableCellQuery.result_cells.Count)];
			foreach (Direction direction in MineableCellQuery.DIRECTION_CHECKS)
			{
				int cellInDirection = Grid.GetCellInDirection(smi.targetMiningCell, direction);
				if (Grid.IsValidCell(cellInDirection) && Grid.IsSolidCell(cellInDirection) && Grid.Element[cellInDirection].tag == smi.def.oreTag)
				{
					smi.cellToMine = cellInDirection;
					return true;
				}
			}
			return false;
		}
		return false;
	}

	private static bool FindOre(BeeForageStates.Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Vector3 position = smi.transform.GetPosition();
		Pickupable pickupable = null;
		int num = 100;
		Extents extents = new Extents((int)position.x, (int)position.y, 15);
		ListPool<ScenePartitionerEntry, BeeForageStates>.PooledList pooledList = ListPool<ScenePartitionerEntry, BeeForageStates>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		Element element = ElementLoader.GetElement(smi.def.oreTag);
		foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
		{
			Pickupable pickupable2 = scenePartitionerEntry.obj as Pickupable;
			if (pickupable2 && pickupable2.GetComponent<ElementChunk>() && pickupable2.GetComponent<PrimaryElement>() && pickupable2.GetComponent<PrimaryElement>().Element == element && !pickupable2.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				int navigationCost = component.GetNavigationCost(Grid.PosToCell(pickupable2));
				if (navigationCost != -1 && navigationCost < num)
				{
					pickupable = pickupable2.GetComponent<Pickupable>();
					num = navigationCost;
				}
			}
		}
		smi.forageTarget = pickupable;
		smi.forageTarget_cell = (smi.forageTarget ? Grid.PosToCell(smi.forageTarget) : Grid.InvalidCell);
		return smi.forageTarget != null;
	}

	private static void ReserveTarget(BeeForageStates.Instance smi)
	{
		GameObject gameObject = (smi.forageTarget ? smi.forageTarget.gameObject : null);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveTarget(BeeForageStates.Instance smi)
	{
		GameObject gameObject = (smi.forageTarget ? smi.forageTarget.gameObject : null);
		if (smi.forageTarget != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static int GetOreCell(BeeForageStates.Instance smi)
	{
		global::Debug.Assert(smi.forageTarget);
		global::Debug.Assert(smi.forageTarget_cell != Grid.InvalidCell);
		return smi.forageTarget_cell;
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	private const string oreSymbol = "snapto_thing";

	private const string oreLegSymbol = "legBeeOre";

	private const string noOreLegSymbol = "legBeeNoOre";

	public BeeForageStates.CollectionBehaviourStates collect;

	public BeeForageStates.StorageBehaviourStates storage;

	public BeeForageStates.ExitStates behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public Def(Tag tag, float amount_to_mine)
		{
			this.oreTag = tag;
			this.amountToMine = amount_to_mine;
		}

		public Tag oreTag;

		public float amountToMine;
	}

	public new class Instance : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.GameInstance
	{
		public Instance(Chore<BeeForageStates.Instance> chore, BeeForageStates.Def def)
			: base(chore, def)
		{
			this.oreSymbolHash = new KAnimHashedString("snapto_thing");
			this.oreLegSymbolHash = new KAnimHashedString("legBeeOre");
			this.noOreLegSymbolHash = new KAnimHashedString("legBeeNoOre");
			base.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(base.smi.oreSymbolHash, false);
			base.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(base.smi.oreLegSymbolHash, false);
			base.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(base.smi.noOreLegSymbolHash, true);
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToForage);
		}

		public int targetMiningCell = Grid.InvalidCell;

		public int cellToMine = Grid.InvalidCell;

		public Pickupable forageTarget;

		public int forageTarget_cell = Grid.InvalidCell;

		public KPrefabID targetHive;

		public KAnimHashedString oreSymbolHash;

		public KAnimHashedString oreLegSymbolHash;

		public KAnimHashedString noOreLegSymbolHash;

		public CellOffset hiveCellOffset = new CellOffset(1, 1);
	}

	public class ForageBehaviourStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
	{
		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State moveToTarget;

		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State pickupTarget;
	}

	public class MiningBehaviourStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
	{
		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State moveToTarget;

		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State mineTarget;
	}

	public class CollectionBehaviourStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
	{
		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State findTarget;

		public BeeForageStates.ForageBehaviourStates forage;

		public BeeForageStates.MiningBehaviourStates mine;
	}

	public class StorageBehaviourStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
	{
		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State moveToHive;

		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State storeMaterial;

		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State dropMaterial;
	}

	public class ExitStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
	{
		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State pre;

		public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State pst;
	}
}
