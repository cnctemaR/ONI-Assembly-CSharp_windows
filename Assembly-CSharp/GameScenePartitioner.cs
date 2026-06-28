using System;
using System.Collections.Generic;

public class GameScenePartitioner : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		GameScenePartitioner.Instance = this;
		this.partitioner = new ScenePartitioner(16, Grid.WidthInCells, Grid.HeightInCells);
		this.solidChangedMask = this.partitioner.CreateMask(new HashedString("SolidChanged"));
		this.liquidChangedMask = this.partitioner.CreateMask(new HashedString("LiquidChanged"));
		this.digDestroyedMask = this.partitioner.CreateMask(new HashedString("DigDestroyed"));
		this.navCellChangedMask = this.partitioner.CreateMask(new HashedString("NavCellChanged"));
		this.fogOfWarChanged = this.partitioner.CreateMask(new HashedString("FogOfWarChanged"));
		this.decorProviders = this.partitioner.CreateMask(new HashedString("DecorProviders"));
		this.attackableEntities = this.partitioner.CreateMask(new HashedString("FactionedEntities"));
		this.fetchChores = this.partitioner.CreateMask(new HashedString("FetchChores"));
		this.pickupables = this.partitioner.CreateMask(new HashedString("Pickupables"));
		this.gasConduits = this.partitioner.CreateMask(new HashedString("GasConduit"));
		this.liquidConduits = this.partitioner.CreateMask(new HashedString("LiquidConduit"));
		this.wires = this.partitioner.CreateMask(new HashedString("Wire"));
		this.objectLayerMasks = new ScenePartitionerMask[23];
		for (int i = 0; i < 23; i++)
		{
			ObjectLayer objectLayer = (ObjectLayer)i;
			this.objectLayerMasks[i] = this.partitioner.CreateMask(new HashedString(objectLayer.ToString()));
		}
	}

	protected override void OnSpawn()
	{
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		NavTable navTable = navGrid.NavTable;
		navTable.OnValidCellChanged = (Action<int, NavType>)Delegate.Combine(navTable.OnValidCellChanged, new Action<int, NavType>(this.OnValidNavCellChanged));
	}

	private void OnValidNavCellChanged(int cell, NavType nav_type)
	{
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.navCellChangedMask.mask, null);
	}

	public GameScenePartitionerEntry Add(string name, object obj, int x, int y, int width, int height, int masks, Action<object> event_callback)
	{
		GameScenePartitionerEntry gameScenePartitionerEntry = new GameScenePartitionerEntry(name, obj, x, y, width, height, masks, this.partitioner, event_callback);
		this.partitioner.Add(gameScenePartitionerEntry);
		return gameScenePartitionerEntry;
	}

	public GameScenePartitionerEntry Add(string name, object obj, Extents extents, int masks, Action<object> event_callback)
	{
		return this.Add(name, obj, extents.x, extents.y, extents.width, extents.height, masks, event_callback);
	}

	public GameScenePartitionerEntry Add(string name, object obj, int cell, int masks, Action<object> event_callback)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		return this.Add(name, obj, num, num2, 1, 1, masks, event_callback);
	}

	public void TriggerEvent(List<int> cells, int masks, object event_data)
	{
		this.partitioner.TriggerEvent(cells, masks, event_data);
	}

	public void TriggerEvent(int x, int y, int width, int height, int masks, object event_data)
	{
		this.partitioner.TriggerEvent(x, y, width, height, masks, event_data);
	}

	public void TriggerEvent(int cell, int masks, object event_data)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		this.TriggerEvent(num, num2, 1, 1, masks, event_data);
	}

	protected override void OnCleanUp()
	{
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		if (navGrid != null)
		{
			NavTable navTable = navGrid.NavTable;
			navTable.OnValidCellChanged = (Action<int, NavType>)Delegate.Remove(navTable.OnValidCellChanged, new Action<int, NavType>(this.OnValidNavCellChanged));
		}
	}

	public void GatherEntries(int x_bottomLeft, int y_bottomLeft, int width, int height, int masks, List<ScenePartitionerEntry> gathered_entries)
	{
		this.partitioner.GatherEntries(x_bottomLeft, y_bottomLeft, width, height, masks, null, gathered_entries);
	}

	public List<ScenePartitionerEntry> ReserveList()
	{
		return this.partitioner.ReserveList();
	}

	public void ReleaseList(List<ScenePartitionerEntry> list)
	{
		this.partitioner.ReleaseList(list);
	}

	public static GameScenePartitioner Instance;

	public ScenePartitionerMask solidChangedMask;

	public ScenePartitionerMask liquidChangedMask;

	public ScenePartitionerMask digDestroyedMask;

	public ScenePartitionerMask navCellChangedMask;

	public ScenePartitionerMask fogOfWarChanged;

	public ScenePartitionerMask decorProviders;

	public ScenePartitionerMask attackableEntities;

	public ScenePartitionerMask fetchChores;

	public ScenePartitionerMask pickupables;

	public ScenePartitionerMask gasConduits;

	public ScenePartitionerMask liquidConduits;

	public ScenePartitionerMask wires;

	public ScenePartitionerMask[] objectLayerMasks;

	private ScenePartitioner partitioner;
}
