using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class RocketModuleHexCellCollector : GameStateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.Never;
		default_state = this.ground;
		this.ground.TagTransition(GameTags.RocketNotOnGround, this.space, false).Enter(new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.ClearHexCellInventoryChangeCallbacks));
		this.space.TagTransition(GameTags.RocketNotOnGround, this.ground, true).Enter(new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.RefreshHexCellInventoryChangeCallbacks)).EventHandler(GameHashes.ClusterLocationChanged, new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.RefreshHexCellInventoryChangeCallbacks))
			.EventHandler(GameHashes.ClusterDestinationReached, new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.RefreshHexCellInventoryChangeCallbacks))
			.DefaultState(this.space.idle);
		this.space.idle.OnSignal(this.HexCellInventoryChangedSignal, this.space.collecting, new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.Parameter<StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.SignalParameter>.Callback(RocketModuleHexCellCollector.CanCollect)).EventHandlerTransition(GameHashes.ClusterLocationChanged, this.space.collecting, new Func<RocketModuleHexCellCollector.Instance, object, bool>(RocketModuleHexCellCollector.CanCollect)).EventHandlerTransition(GameHashes.ClusterDestinationReached, this.space.collecting, new Func<RocketModuleHexCellCollector.Instance, object, bool>(RocketModuleHexCellCollector.CanCollect))
			.Target(this.ClusterCraft)
			.EventHandlerTransition(GameHashes.ClusterDestinationChanged, this.space.collecting, new Func<RocketModuleHexCellCollector.Instance, object, bool>(RocketModuleHexCellCollector.CanCollect));
		this.space.collecting.Toggle("ToggleCollectingTag", new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.AddCollectingTag), new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.RemoveCollectingTag)).UpdateTransition(this.space.idle, new Func<RocketModuleHexCellCollector.Instance, float, bool>(RocketModuleHexCellCollector.CollectUpdate), UpdateRate.SIM_1000ms, false).Exit(new StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State.Callback(RocketModuleHexCellCollector.ClearMassCharge));
	}

	public static void ClearHexCellInventoryChangeCallbacks(RocketModuleHexCellCollector.Instance smi)
	{
		GameObject gameObject = smi.sm.HexCellInventory.Get(smi);
		if (gameObject != null)
		{
			gameObject.Unsubscribe(-1697596308, new Action<object>(smi.TriggerHexCellStorageChangeEvent));
			smi.sm.HexCellInventory.Set(null, smi);
		}
	}

	public static void RefreshHexCellInventoryChangeCallbacks(RocketModuleHexCellCollector.Instance smi)
	{
		GameObject gameObject = smi.sm.HexCellInventory.Get(smi);
		if (gameObject != null)
		{
			gameObject.Unsubscribe(-1697596308, new Action<object>(smi.TriggerHexCellStorageChangeEvent));
		}
		StarmapHexCellInventory starmapHexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(smi.StarmapLocation);
		smi.sm.HexCellInventory.Set(starmapHexCellInventory.gameObject, smi, false);
		if (starmapHexCellInventory != null)
		{
			starmapHexCellInventory.gameObject.Subscribe(-1697596308, new Action<object>(smi.TriggerHexCellStorageChangeEvent));
		}
	}

	public static bool CanCollect(RocketModuleHexCellCollector.Instance smi, object o)
	{
		return RocketModuleHexCellCollector.CanCollect(smi);
	}

	public static bool CanCollect(RocketModuleHexCellCollector.Instance smi)
	{
		if (smi.storage.RemainingCapacity() <= 0f)
		{
			return false;
		}
		StarmapHexCellInventory starmapHexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(smi.StarmapLocation);
		bool flag = starmapHexCellInventory.TotalMass > 0f;
		if (smi.IsSpaceshipMoving)
		{
			return false;
		}
		if (!flag)
		{
			return false;
		}
		using (List<StarmapHexCellInventory.SerializedItem>.Enumerator enumerator = starmapHexCellInventory.Items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				bool flag2;
				float num;
				if (RocketModuleHexCellCollector.CanHexCellItemBeStored(enumerator.Current, smi, out flag2, out num))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool CollectUpdate(RocketModuleHexCellCollector.Instance smi, float dt)
	{
		if (dt == 0f)
		{
			return false;
		}
		Storage storage = smi.storage;
		float num = storage.RemainingCapacity();
		if (num <= 0f)
		{
			return true;
		}
		StarmapHexCellInventory starmapHexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(smi.StarmapLocation);
		bool flag = starmapHexCellInventory.TotalMass > 0f;
		if (smi.IsSpaceshipMoving)
		{
			return true;
		}
		if (!flag)
		{
			return true;
		}
		float num2 = smi.MassCharge + dt * smi.def.collectSpeed;
		smi.MassCharge = 0f;
		num2 = Mathf.Min(num, num2);
		int count = starmapHexCellInventory.Items.Count;
		float num3 = num2;
		float num4 = 0f;
		bool flag2 = false;
		RocketModuleHexCellCollector.ClearAllItemData();
		if (RocketModuleHexCellCollector.ItemDataObjects.Count < count)
		{
			int num5 = count - RocketModuleHexCellCollector.ItemDataObjects.Count;
			for (int i = 0; i < num5; i++)
			{
				RocketModuleHexCellCollector.ItemDataObjects.Add(new RocketModuleHexCellCollector.ItemData());
			}
		}
		float num6 = 0f;
		for (int j = 0; j < count; j++)
		{
			RocketModuleHexCellCollector.ItemData itemData = RocketModuleHexCellCollector.ItemDataObjects[j];
			itemData.Clear();
			StarmapHexCellInventory.SerializedItem serializedItem = starmapHexCellInventory.Items[j];
			bool flag3 = false;
			float num7 = 1f;
			bool flag4 = RocketModuleHexCellCollector.CanHexCellItemBeStored(serializedItem, smi, out flag3, out num7);
			itemData.ItemID = serializedItem.ID;
			itemData.Mass = serializedItem.Mass;
			itemData.massPerUnit = num7;
			itemData.usesUnits = flag3;
			itemData.isValid = flag4;
			num6 += (flag4 ? serializedItem.Mass : 0f);
		}
		for (int k = 0; k < count; k++)
		{
			RocketModuleHexCellCollector.ItemData itemData2 = RocketModuleHexCellCollector.ItemDataObjects[k];
			if (itemData2.isValid)
			{
				itemData2.Proportion = itemData2.Mass / num6;
				float num8 = itemData2.Proportion * num2;
				if (!itemData2.usesUnits || num8 >= itemData2.massPerUnit)
				{
					float num9 = num8;
					if (itemData2.usesUnits)
					{
						num9 = (float)Mathf.FloorToInt(num8 / itemData2.massPerUnit) * itemData2.massPerUnit;
					}
					float num10 = starmapHexCellInventory.ExtractAndStoreItemMass(itemData2.ItemID, num9, storage);
					num3 -= num10;
					num4 += num10;
				}
				else
				{
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			smi.MassCharge += num3;
		}
		return storage.RemainingCapacity() <= 0f || (!flag2 && num4 <= 0f);
	}

	private static bool CanHexCellItemBeStored(StarmapHexCellInventory.SerializedItem item, RocketModuleHexCellCollector.Instance smi, out bool itemUsesUnits, out float massPerUnit)
	{
		itemUsesUnits = false;
		massPerUnit = 1f;
		GameObject prefab = Assets.GetPrefab(item.ID);
		if (prefab != null)
		{
			KPrefabID component = prefab.GetComponent<KPrefabID>();
			bool flag = false;
			if (smi.treeFilterable != null)
			{
				using (HashSet<Tag>.Enumerator enumerator = smi.treeFilterable.GetTags().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Tag tag = enumerator.Current;
						if (component.HasTag(tag))
						{
							flag = true;
							break;
						}
					}
					goto IL_008E;
				}
			}
			flag = component.HasAnyTags(smi.storage.storageFilters);
			IL_008E:
			if (flag)
			{
				Element element = ElementLoader.GetElement(component.PrefabID());
				PrimaryElement component2 = prefab.GetComponent<PrimaryElement>();
				itemUsesUnits = element == null && component2 != null && GameTags.DisplayAsUnits.Contains(item.ID);
				massPerUnit = ((component2 == null) ? 1f : component2.MassPerUnit);
				if (!itemUsesUnits || (item.Mass >= component2.MassPerUnit && smi.storage.RemainingCapacity() >= component2.MassPerUnit))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void RemoveCollectingTag(RocketModuleHexCellCollector.Instance smi)
	{
		RocketModuleHexCellCollector.ToggleCollectingTag(smi, false);
	}

	public static void AddCollectingTag(RocketModuleHexCellCollector.Instance smi)
	{
		RocketModuleHexCellCollector.ToggleCollectingTag(smi, true);
	}

	public static void ToggleCollectingTag(RocketModuleHexCellCollector.Instance smi, bool v)
	{
		smi.ToggleCollectingTag(v);
	}

	public static void ClearMassCharge(RocketModuleHexCellCollector.Instance smi)
	{
		smi.MassCharge = 0f;
	}

	private static void ClearAllItemData()
	{
		foreach (RocketModuleHexCellCollector.ItemData itemData in RocketModuleHexCellCollector.ItemDataObjects)
		{
			itemData.Clear();
		}
	}

	public GameStateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State ground;

	public RocketModuleHexCellCollector.InSpaceStates space;

	public StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.Signal HexCellInventoryChangedSignal;

	public StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.TargetParameter ClusterCraft;

	public StateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.TargetParameter HexCellInventory;

	private static List<RocketModuleHexCellCollector.ItemData> ItemDataObjects = new List<RocketModuleHexCellCollector.ItemData>
	{
		new RocketModuleHexCellCollector.ItemData(),
		new RocketModuleHexCellCollector.ItemData(),
		new RocketModuleHexCellCollector.ItemData(),
		new RocketModuleHexCellCollector.ItemData(),
		new RocketModuleHexCellCollector.ItemData(),
		new RocketModuleHexCellCollector.ItemData()
	};

	public class Def : StateMachine.BaseDef
	{
		public float collectSpeed;

		public bool formatCapacityBarAsUnits;
	}

	public class InSpaceStates : GameStateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State
	{
		public GameStateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State idle;

		public GameStateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.State collecting;
	}

	private class ItemData
	{
		public void Clear()
		{
			this.ItemID = null;
			this.Mass = 0f;
			this.Proportion = 0f;
			this.isValid = false;
			this.usesUnits = false;
			this.massPerUnit = 1f;
		}

		public Tag ItemID;

		public float Mass;

		public float Proportion;

		public float massPerUnit;

		public bool usesUnits;

		public bool isValid;
	}

	public new class Instance : GameStateMachine<RocketModuleHexCellCollector, RocketModuleHexCellCollector.Instance, IStateMachineTarget, RocketModuleHexCellCollector.Def>.GameInstance, IHexCellCollector
	{
		public bool IsCollecting
		{
			get
			{
				return base.IsInsideState(base.sm.space.collecting);
			}
		}

		public bool IsSpaceshipMoving
		{
			get
			{
				return this.clustercraft.IsFlightInProgress();
			}
		}

		public AxialI StarmapLocation
		{
			get
			{
				return this.clustercraft.Location;
			}
		}

		public Instance(IStateMachineTarget master, RocketModuleHexCellCollector.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<Storage>();
			this.treeFilterable = null;
		}

		public override void StartSM()
		{
			this.clustercraft = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			base.sm.ClusterCraft.Set(this.clustercraft.gameObject, this, false);
			base.StartSM();
		}

		public void TriggerHexCellStorageChangeEvent(object o)
		{
			base.sm.HexCellInventoryChangedSignal.Trigger(base.smi);
		}

		public void ToggleCollectingTag(bool collecting)
		{
			if (collecting)
			{
				this.clustercraft.AddTag(GameTags.RocketCollectingResources);
				return;
			}
			List<RocketModuleHexCellCollector.Instance> allHexCellCollectorModules = this.clustercraft.GetAllHexCellCollectorModules();
			bool flag = false;
			foreach (RocketModuleHexCellCollector.Instance instance in allHexCellCollectorModules)
			{
				if (instance != this && instance != null && instance.IsCollecting)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.clustercraft.RemoveTag(GameTags.RocketCollectingResources);
			}
		}

		public bool CheckIsCollecting()
		{
			return base.IsInsideState(base.sm.space.collecting);
		}

		public string GetProperName()
		{
			return base.GetComponent<RocketModuleCluster>().GetProperName();
		}

		public Sprite GetUISprite()
		{
			return global::Def.GetUISprite(base.master.gameObject.GetComponent<KPrefabID>().PrefabID(), "ui", false).first;
		}

		public float GetCapacity()
		{
			return this.storage.Capacity();
		}

		public float GetMassStored()
		{
			return this.storage.MassStored();
		}

		public float TimeInState()
		{
			return this.timeinstate;
		}

		public string GetCapacityBarText()
		{
			if (base.def.formatCapacityBarAsUnits)
			{
				return GameUtil.GetFormattedUnits(this.GetMassStored(), GameUtil.TimeSlice.None, true, "") + " / " + GameUtil.GetFormattedUnits(this.GetCapacity(), GameUtil.TimeSlice.None, true, "");
			}
			return GameUtil.GetFormattedMass(this.GetMassStored(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") + " / " + GameUtil.GetFormattedMass(this.GetCapacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		}

		[Serialize]
		public int LastCollectedIndex;

		[Serialize]
		public float MassCharge;

		public Storage storage;

		public TreeFilterable treeFilterable;

		private Clustercraft clustercraft;
	}
}
