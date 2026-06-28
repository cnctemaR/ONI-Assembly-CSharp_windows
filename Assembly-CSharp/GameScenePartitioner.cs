using System;
using System.Collections.Generic;

public class GameScenePartitioner : KMonoBehaviour
{
	public static GameScenePartitioner Instance
	{
		get
		{
			return GameScenePartitioner.instance;
		}
	}

	protected override void OnPrefabInit()
	{
		GameScenePartitioner.instance = this;
		this.partitioner = new ScenePartitioner(16, 64, Grid.WidthInCells, Grid.HeightInCells);
		this.solidChangedLayer = this.partitioner.CreateMask(new HashedString("SolidChanged"));
		this.liquidChangedLayer = this.partitioner.CreateMask(new HashedString("LiquidChanged"));
		this.digDestroyedLayer = this.partitioner.CreateMask(new HashedString("DigDestroyed"));
		this.fogOfWarChangedLayer = this.partitioner.CreateMask(new HashedString("FogOfWarChanged"));
		this.decorProviderLayer = this.partitioner.CreateMask(new HashedString("DecorProviders"));
		this.attackableEntitiesLayer = this.partitioner.CreateMask(new HashedString("FactionedEntities"));
		this.fetchChoreLayer = this.partitioner.CreateMask(new HashedString("FetchChores"));
		this.pickupablesLayer = this.partitioner.CreateMask(new HashedString("Pickupables"));
		this.pickupablesChangedLayer = this.partitioner.CreateMask(new HashedString("PickupablesChanged"));
		this.gasConduitsLayer = this.partitioner.CreateMask(new HashedString("GasConduit"));
		this.liquidConduitsLayer = this.partitioner.CreateMask(new HashedString("LiquidConduit"));
		this.solidConduitsLayer = this.partitioner.CreateMask(new HashedString("SolidConduit"));
		this.wiresLayer = this.partitioner.CreateMask(new HashedString("Wire"));
		this.noisePolluterLayer = this.partitioner.CreateMask(new HashedString("NoisePolluters"));
		this.validNavCellChangedLayer = this.partitioner.CreateMask(new HashedString("validNavCellChangedLayer"));
		this.dirtyNavCellUpdateLayer = this.partitioner.CreateMask(new HashedString("dirtyNavCellUpdateLayer"));
		this.trapsLayer = this.partitioner.CreateMask(new HashedString("trapsLayer"));
		this.floorSwitchActivatorLayer = this.partitioner.CreateMask(new HashedString("FloorSwitchActivatorLayer"));
		this.floorSwitchActivatorChangedLayer = this.partitioner.CreateMask(new HashedString("FloorSwitchActivatorChangedLayer"));
		this.collisionLayer = this.partitioner.CreateMask(new HashedString("Collision"));
		this.objectLayers = new ScenePartitionerLayer[36];
		for (int i = 0; i < 36; i++)
		{
			ObjectLayer objectLayer = (ObjectLayer)i;
			this.objectLayers[i] = this.partitioner.CreateMask(new HashedString(objectLayer.ToString()));
		}
	}

	protected override void OnForcedCleanUp()
	{
		GameScenePartitioner.instance = null;
		this.partitioner.FreeResources();
		this.partitioner = null;
		this.solidChangedLayer = null;
		this.liquidChangedLayer = null;
		this.digDestroyedLayer = null;
		this.fogOfWarChangedLayer = null;
		this.decorProviderLayer = null;
		this.attackableEntitiesLayer = null;
		this.fetchChoreLayer = null;
		this.pickupablesLayer = null;
		this.pickupablesChangedLayer = null;
		this.gasConduitsLayer = null;
		this.liquidConduitsLayer = null;
		this.solidConduitsLayer = null;
		this.wiresLayer = null;
		this.noisePolluterLayer = null;
		this.validNavCellChangedLayer = null;
		this.dirtyNavCellUpdateLayer = null;
		this.trapsLayer = null;
		this.floorSwitchActivatorLayer = null;
		this.floorSwitchActivatorChangedLayer = null;
		this.objectLayers = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		NavGrid navGrid2 = navGrid;
		navGrid2.OnNavGridUpdateComplete = (Action<HashSet<int>>)Delegate.Combine(navGrid2.OnNavGridUpdateComplete, new Action<HashSet<int>>(this.OnNavGridUpdateComplete));
		NavTable navTable = navGrid.NavTable;
		navTable.OnValidCellChanged = (Action<int, NavType>)Delegate.Combine(navTable.OnValidCellChanged, new Action<int, NavType>(this.OnValidNavCellChanged));
	}

	public GameScenePartitionerEntry Add(string name, object obj, int x, int y, int width, int height, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		GameScenePartitionerEntry gameScenePartitionerEntry = new GameScenePartitionerEntry(name, obj, x, y, width, height, layer, this.partitioner, event_callback);
		this.partitioner.Add(gameScenePartitionerEntry);
		return gameScenePartitionerEntry;
	}

	public GameScenePartitionerEntry Add(string name, object obj, Extents extents, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		return this.Add(name, obj, extents.x, extents.y, extents.width, extents.height, layer, event_callback);
	}

	public GameScenePartitionerEntry Add(string name, object obj, int cell, ScenePartitionerLayer layer, Action<object> event_callback)
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

	public void TriggerEvent(HashSet<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		this.partitioner.TriggerEvent(cells, layer, event_data);
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

	public void GatherEntries(Extents extents, ScenePartitionerLayer layer, List<ScenePartitionerEntry> gathered_entries)
	{
		this.GatherEntries(extents.x, extents.y, extents.width, extents.height, layer, gathered_entries);
	}

	public void GatherEntries(int x_bottomLeft, int y_bottomLeft, int width, int height, ScenePartitionerLayer layer, List<ScenePartitionerEntry> gathered_entries)
	{
		this.partitioner.GatherEntries(x_bottomLeft, y_bottomLeft, width, height, layer, null, gathered_entries);
	}

	private void OnValidNavCellChanged(int cell, NavType nav_type)
	{
		this.changedCells.Add(cell);
	}

	private void OnNavGridUpdateComplete(HashSet<int> dirty_nav_cells)
	{
		if (dirty_nav_cells.Count > 0)
		{
			GameScenePartitioner.Instance.TriggerEvent(dirty_nav_cells, GameScenePartitioner.Instance.dirtyNavCellUpdateLayer, null);
		}
		if (this.changedCells.Count > 0)
		{
			GameScenePartitioner.Instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.validNavCellChangedLayer, null);
			this.changedCells.Clear();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.partitioner.Cleanup();
	}

	public ScenePartitionerLayer solidChangedLayer;

	public ScenePartitionerLayer liquidChangedLayer;

	public ScenePartitionerLayer digDestroyedLayer;

	public ScenePartitionerLayer fogOfWarChangedLayer;

	public ScenePartitionerLayer decorProviderLayer;

	public ScenePartitionerLayer attackableEntitiesLayer;

	public ScenePartitionerLayer fetchChoreLayer;

	public ScenePartitionerLayer pickupablesLayer;

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

	private ScenePartitioner partitioner;

	private static GameScenePartitioner instance;

	private List<int> changedCells = new List<int>();
}
