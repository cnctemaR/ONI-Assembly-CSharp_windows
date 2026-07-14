using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GameScenePartitioner")]
public class GameScenePartitioner : KMonoBehaviour
{
	public static GameScenePartitioner Instance
	{
		get
		{
			global::Debug.Assert(GameScenePartitioner.instance != null);
			return GameScenePartitioner.instance;
		}
	}

	public bool Lookup(HandleVector<int>.Handle handle, out ScenePartitionerEntry entry)
	{
		if (!this.scenePartitionerEntries.IsValid(handle) || !this.scenePartitionerEntries.IsVersionValid(handle))
		{
			entry = null;
			return false;
		}
		entry = this.scenePartitionerEntries.GetData(handle);
		return true;
	}

	protected override void OnPrefabInit()
	{
		global::Debug.Assert(GameScenePartitioner.instance == null);
		GameScenePartitioner.instance = this;
		this.partitioner = new ScenePartitioner(16, 68, Grid.WidthInCells, Grid.HeightInCells);
		this.solidChangedLayer = this.partitioner.CreateMask("SolidChanged");
		this.backwallChangedLayer = this.partitioner.CreateMask("BackwallChanged");
		this.liquidChangedLayer = this.partitioner.CreateMask("LiquidChanged");
		this.digDestroyedLayer = this.partitioner.CreateMask("DigDestroyed");
		this.fogOfWarChangedLayer = this.partitioner.CreateMask("FogOfWarChanged");
		this.decorProviderLayer = this.partitioner.CreateMask("DecorProviders");
		this.attackableEntitiesLayer = this.partitioner.CreateMask("FactionedEntities");
		this.fetchChoreLayer = this.partitioner.CreateMask("FetchChores");
		this.pickupablesLayer = this.partitioner.CreateMask("Pickupables");
		this.storedPickupablesLayer = this.partitioner.CreateMask("StoredPickupables");
		this.pickupablesChangedLayer = this.partitioner.CreateMask("PickupablesChanged");
		this.plantsChangedLayer = this.partitioner.CreateMask("PlantsChanged");
		this.gasConduitsLayer = this.partitioner.CreateMask("GasConduit");
		this.liquidConduitsLayer = this.partitioner.CreateMask("LiquidConduit");
		this.solidConduitsLayer = this.partitioner.CreateMask("SolidConduit");
		this.noisePolluterLayer = this.partitioner.CreateMask("NoisePolluters");
		this.validNavCellChangedLayer = this.partitioner.CreateMask("validNavCellChangedLayer");
		this.dirtyNavCellUpdateLayer = this.partitioner.CreateMask("dirtyNavCellUpdateLayer");
		this.trapsLayer = this.partitioner.CreateMask("trapsLayer");
		this.floorSwitchActivatorLayer = this.partitioner.CreateMask("FloorSwitchActivatorLayer");
		this.floorSwitchActivatorChangedLayer = this.partitioner.CreateMask("FloorSwitchActivatorChangedLayer");
		this.collisionLayer = this.partitioner.CreateMask("Collision");
		this.lure = this.partitioner.CreateMask("Lure");
		this.plants = this.partitioner.CreateMask("Plants");
		this.industrialBuildings = this.partitioner.CreateMask("IndustrialBuildings");
		this.completeBuildings = this.partitioner.CreateMask("CompleteBuildings");
		this.prioritizableObjects = this.partitioner.CreateMask("PrioritizableObjects");
		this.contactConductiveLayer = this.partitioner.CreateMask("ContactConductiveLayer");
		this.objectLayers = new ScenePartitionerLayer[45];
		for (int i = 0; i < 45; i++)
		{
			ObjectLayer objectLayer = (ObjectLayer)i;
			this.objectLayers[i] = this.partitioner.CreateMask(objectLayer.ToString());
		}
	}

	protected override void OnForcedCleanUp()
	{
		GameScenePartitioner.instance = null;
		this.partitioner.FreeResources();
		this.partitioner = null;
		this.solidChangedLayer = null;
		this.backwallChangedLayer = null;
		this.liquidChangedLayer = null;
		this.digDestroyedLayer = null;
		this.fogOfWarChangedLayer = null;
		this.decorProviderLayer = null;
		this.attackableEntitiesLayer = null;
		this.fetchChoreLayer = null;
		this.pickupablesLayer = null;
		this.storedPickupablesLayer = null;
		this.plantsChangedLayer = null;
		this.pickupablesChangedLayer = null;
		this.gasConduitsLayer = null;
		this.liquidConduitsLayer = null;
		this.solidConduitsLayer = null;
		this.noisePolluterLayer = null;
		this.validNavCellChangedLayer = null;
		this.dirtyNavCellUpdateLayer = null;
		this.trapsLayer = null;
		this.floorSwitchActivatorLayer = null;
		this.floorSwitchActivatorChangedLayer = null;
		this.contactConductiveLayer = null;
		this.objectLayers = null;
		this.scenePartitionerEntries.Clear();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		navGrid.OnNavGridUpdateComplete = (Action<List<int>>)Delegate.Combine(navGrid.OnNavGridUpdateComplete, new Action<List<int>>(this.OnNavGridUpdateComplete));
		NavTable navTable = navGrid.NavTable;
		navTable.OnValidCellChanged = (Action<int, NavType>)Delegate.Combine(navTable.OnValidCellChanged, new Action<int, NavType>(this.OnValidNavCellChanged));
	}

	public HandleVector<int>.Handle Add(string name, object obj, int x, int y, int width, int height, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		ScenePartitionerEntry scenePartitionerEntry = ScenePartitionerEntry.EntryPool.Get();
		scenePartitionerEntry.Init(name, obj, x, y, width, height, layer, this.partitioner, event_callback);
		HandleVector<int>.Handle handle = this.scenePartitionerEntries.Allocate(scenePartitionerEntry);
		this.partitioner.Add(handle);
		return handle;
	}

	public HandleVector<int>.Handle Add(string name, object obj, Extents extents, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		return this.Add(name, obj, extents.x, extents.y, extents.width, extents.height, layer, event_callback);
	}

	public HandleVector<int>.Handle Add(string name, object obj, int cell, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		return this.Add(name, obj, num, num2, 1, 1, layer, event_callback);
	}

	public void AddGlobalLayerListener(ScenePartitionerLayer layer, Action<int, object> action)
	{
		layer.OnEvent = (Action<int, object>)Delegate.Combine(layer.OnEvent, action);
	}

	public void RemoveGlobalLayerListener(ScenePartitionerLayer layer, Action<int, object> action)
	{
		layer.OnEvent = (Action<int, object>)Delegate.Remove(layer.OnEvent, action);
	}

	public void TriggerEvent(List<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		this.partitioner.TriggerEvent(cells, layer, event_data);
	}

	public void TriggerEvent(Extents extents, ScenePartitionerLayer layer, object event_data)
	{
		this.partitioner.TriggerEvent(extents.x, extents.y, extents.width, extents.height, layer, event_data);
	}

	public void TriggerEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		this.partitioner.TriggerEvent(x, y, width, height, layer, event_data);
	}

	public void TriggerEvent(int cell, ScenePartitionerLayer layer, object event_data)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		this.TriggerEvent(num, num2, 1, 1, layer, event_data);
	}

	[Obsolete("use Visit pattern instead")]
	public void GatherEntries(Extents extents, ScenePartitionerLayer layer, List<ScenePartitionerEntry> gathered_entries)
	{
		this.GatherEntries(extents.x, extents.y, extents.width, extents.height, layer, gathered_entries);
	}

	public void GatherEntries(int x_bottomLeft, int y_bottomLeft, int width, int height, ScenePartitionerLayer layer, List<ScenePartitionerEntry> gathered_entries)
	{
		this.partitioner.GatherEntries(x_bottomLeft, y_bottomLeft, width, height, layer, null, gathered_entries);
	}

	public void VisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, Func<object, ContextType, Util.IterationInstruction> visitor, ContextType context) where ContextType : class
	{
		this.partitioner.VisitEntries<ContextType>(x, y, width, height, layer, visitor, context);
	}

	public void VisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, GameScenePartitioner.VisitorRef<ContextType> visitor, ref ContextType context) where ContextType : struct
	{
		this.partitioner.VisitEntries<ContextType>(x, y, width, height, layer, visitor, ref context);
	}

	public void ReadonlyVisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, Func<object, ContextType, Util.IterationInstruction> visitor, ContextType context) where ContextType : class
	{
		this.partitioner.ReadonlyVisitEntries<ContextType>(x, y, width, height, layer, visitor, context);
	}

	public void ReadonlyVisitEntries<ContextType>(int x, int y, int width, int height, ScenePartitionerLayer layer, GameScenePartitioner.VisitorRef<ContextType> visitor, ref ContextType context) where ContextType : struct
	{
		this.partitioner.ReadonlyVisitEntries<ContextType>(x, y, width, height, layer, visitor, ref context);
	}

	private void OnValidNavCellChanged(int cell, NavType nav_type)
	{
		this.changedCells.Add(cell);
	}

	private void OnNavGridUpdateComplete(List<int> dirty_nav_cells)
	{
		GameScenePartitioner.Instance.TriggerEvent(dirty_nav_cells, GameScenePartitioner.Instance.dirtyNavCellUpdateLayer, null);
		if (this.changedCells.Count > 0)
		{
			GameScenePartitioner.Instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.validNavCellChangedLayer, null);
			this.changedCells.Clear();
		}
	}

	public void UpdatePosition(HandleVector<int>.Handle handle, int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		this.UpdatePosition(handle, vector2I.x, vector2I.y);
	}

	public void UpdatePosition(HandleVector<int>.Handle handle, int x, int y)
	{
		if (!handle.IsValid())
		{
			return;
		}
		this.scenePartitionerEntries.GetData(handle).UpdatePosition(handle, x, y);
	}

	public void UpdatePosition(HandleVector<int>.Handle handle, Extents ext)
	{
		if (!handle.IsValid())
		{
			return;
		}
		this.scenePartitionerEntries.GetData(handle).UpdatePosition(handle, ext);
	}

	public void Free(ref HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return;
		}
		ScenePartitionerEntry data = this.scenePartitionerEntries.GetData(handle);
		data.Release(handle);
		this.scenePartitionerEntries.Free(handle);
		handle.Clear();
		ScenePartitionerEntry.EntryPool.Release(data);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.partitioner.Cleanup();
	}

	public bool DoDebugLayersContainItemsOnCell(int cell)
	{
		return this.partitioner.DoDebugLayersContainItemsOnCell(cell);
	}

	public List<ScenePartitionerLayer> GetLayers()
	{
		return this.partitioner.layers;
	}

	public void SetToggledLayers(HashSet<ScenePartitionerLayer> toggled_layers)
	{
		this.partitioner.toggledLayers = toggled_layers;
	}

	public ScenePartitionerLayer solidChangedLayer;

	public ScenePartitionerLayer backwallChangedLayer;

	public ScenePartitionerLayer liquidChangedLayer;

	public ScenePartitionerLayer digDestroyedLayer;

	public ScenePartitionerLayer fogOfWarChangedLayer;

	public ScenePartitionerLayer decorProviderLayer;

	public ScenePartitionerLayer attackableEntitiesLayer;

	public ScenePartitionerLayer fetchChoreLayer;

	public ScenePartitionerLayer pickupablesLayer;

	public ScenePartitionerLayer storedPickupablesLayer;

	public ScenePartitionerLayer pickupablesChangedLayer;

	public ScenePartitionerLayer gasConduitsLayer;

	public ScenePartitionerLayer liquidConduitsLayer;

	public ScenePartitionerLayer solidConduitsLayer;

	public ScenePartitionerLayer wiresLayer;

	public ScenePartitionerLayer[] objectLayers;

	public ScenePartitionerLayer noisePolluterLayer;

	public ScenePartitionerLayer validNavCellChangedLayer;

	public ScenePartitionerLayer dirtyNavCellUpdateLayer;

	public ScenePartitionerLayer trapsLayer;

	public ScenePartitionerLayer floorSwitchActivatorLayer;

	public ScenePartitionerLayer floorSwitchActivatorChangedLayer;

	public ScenePartitionerLayer collisionLayer;

	public ScenePartitionerLayer lure;

	public ScenePartitionerLayer plants;

	public ScenePartitionerLayer plantsChangedLayer;

	public ScenePartitionerLayer industrialBuildings;

	public ScenePartitionerLayer completeBuildings;

	public ScenePartitionerLayer prioritizableObjects;

	public ScenePartitionerLayer contactConductiveLayer;

	private ScenePartitioner partitioner;

	private static GameScenePartitioner instance;

	private KCompactedVector<ScenePartitionerEntry> scenePartitionerEntries = new KCompactedVector<ScenePartitionerEntry>(0);

	private List<int> changedCells = new List<int>();

	public delegate Util.IterationInstruction VisitorRef<ContextType>(object obj, ref ContextType context);
}
