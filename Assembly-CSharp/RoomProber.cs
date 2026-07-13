using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomProber : ISim1000ms
{
	public RoomProber()
	{
		CavityInfo cavityInfo = this.CreateNewCavity();
		cavityInfo.cells = new List<int>
		{
			Capacity = Grid.CellCount
		};
		for (int i = 0; i < Grid.CellCount; i++)
		{
			cavityInfo.cells.Add(i);
		}
		this.CellCavityID = new HandleVector<int>.Handle[Grid.CellCount];
		Array.Fill<HandleVector<int>.Handle>(this.CellCavityID, cavityInfo.handle);
		this.solidChanges.Add(0);
		this.refresh = new RoomProber.RefreshModule(this);
		this.refresh.Initialize();
		this.Refresh();
		Game instance = Game.Instance;
		instance.OnSpawnComplete = (global::System.Action)Delegate.Combine(instance.OnSpawnComplete, new global::System.Action(this.Refresh));
		World instance2 = World.Instance;
		instance2.OnSolidChanged = (Action<int>)Delegate.Combine(instance2.OnSolidChanged, new Action<int>(this.SolidChangedEvent));
		GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], new Action<int, object>(this.OnBuildingsChanged));
		GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[2], new Action<int, object>(this.OnBuildingsChanged));
	}

	private void SolidChangedEvent(int cell)
	{
		this.SolidChangedEvent(cell, true);
	}

	private void OnBuildingsChanged(int cell, object building)
	{
		if (this.GetCavityForCell(cell) != null)
		{
			this.solidChanges.Add(cell);
			this.dirty = true;
		}
	}

	public void TriggerBuildingChangedEvent(int cell, object building)
	{
		this.OnBuildingsChanged(cell, building);
	}

	public void SolidChangedEvent(int cell, bool ignoreDoors)
	{
		if (ignoreDoors && Grid.HasDoor[cell])
		{
			return;
		}
		this.solidChanges.Add(cell);
		this.dirty = true;
	}

	private CavityInfo CreateNewCavity()
	{
		CavityInfo cavityInfo = new CavityInfo();
		cavityInfo.handle = this.cavityInfos.Allocate(cavityInfo);
		return cavityInfo;
	}

	private static bool IsCavityBoundary(int cell)
	{
		return (Grid.BuildMasks[cell] & (Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation)) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor) || Grid.HasDoor[cell];
	}

	public void Refresh()
	{
		this.refresh.Run();
	}

	public void Sim1000ms(float dt)
	{
		if (this.dirty)
		{
			this.Refresh();
		}
	}

	private void CreateRoom(CavityInfo cavity)
	{
		global::Debug.Assert(cavity.room == null);
		Room room = new Room
		{
			cavity = cavity
		};
		cavity.room = room;
		this.rooms.Add(room);
		room.roomType = Db.Get().RoomTypes.GetRoomType(room);
		this.AssignBuildingsToRoom(room);
	}

	private void ClearRoom(Room room)
	{
		this.UnassignBuildingsToRoom(room);
		room.CleanUp();
		this.rooms.Remove(room);
	}

	private void RefreshRooms(List<KPrefabID> dirtyEntities)
	{
		int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
		foreach (CavityInfo cavityInfo in this.cavityInfos.GetDataList())
		{
			if (cavityInfo.dirty)
			{
				global::Debug.Assert(cavityInfo.room == null, "I expected info.room to always be null by this point");
				if (cavityInfo.NumCells > 0)
				{
					if (cavityInfo.NumCells <= maxRoomSize)
					{
						this.CreateRoom(cavityInfo);
					}
					foreach (KPrefabID kprefabID in cavityInfo.buildings)
					{
						kprefabID.Trigger(144050788, cavityInfo.room);
					}
					foreach (KPrefabID kprefabID2 in cavityInfo.plants)
					{
						kprefabID2.Trigger(144050788, cavityInfo.room);
					}
				}
				cavityInfo.dirty = false;
			}
		}
		foreach (KPrefabID kprefabID3 in dirtyEntities)
		{
			if (kprefabID3 != null)
			{
				kprefabID3.Trigger(144050788, null);
			}
		}
		this.dirty = false;
	}

	private void AssignBuildingsToRoom(Room room)
	{
		global::Debug.Assert(room != null);
		RoomType roomType = room.roomType;
		if (roomType == Db.Get().RoomTypes.Neutral)
		{
			return;
		}
		foreach (KPrefabID kprefabID in room.buildings)
		{
			Assignable assignable;
			if (!(kprefabID == null) && !kprefabID.HasTag(GameTags.NotRoomAssignable) && kprefabID.TryGetComponent<Assignable>(out assignable) && (roomType.primary_constraint == null || !roomType.primary_constraint.building_criteria(kprefabID)))
			{
				assignable.Assign(room);
			}
		}
	}

	private void UnassignKPrefabIDs(Room room, List<KPrefabID> buildings)
	{
		foreach (KPrefabID kprefabID in buildings)
		{
			if (!(kprefabID == null))
			{
				kprefabID.Trigger(144050788, null);
				Assignable assignable;
				if (kprefabID.TryGetComponent<Assignable>(out assignable) && assignable.assignee == room)
				{
					assignable.Unassign();
				}
			}
		}
	}

	private void UnassignBuildingsToRoom(Room room)
	{
		global::Debug.Assert(room != null);
		this.UnassignKPrefabIDs(room, room.buildings);
		this.UnassignKPrefabIDs(room, room.plants);
	}

	public void UpdateRoom(CavityInfo cavity)
	{
		if (cavity == null)
		{
			return;
		}
		if (cavity.room != null)
		{
			this.ClearRoom(cavity.room);
			cavity.room = null;
		}
		this.CreateRoom(cavity);
		foreach (KPrefabID kprefabID in cavity.buildings)
		{
			if (kprefabID != null)
			{
				kprefabID.Trigger(144050788, cavity.room);
			}
		}
		foreach (KPrefabID kprefabID2 in cavity.plants)
		{
			if (kprefabID2 != null)
			{
				kprefabID2.Trigger(144050788, cavity.room);
			}
		}
	}

	public Room GetRoomOfGameObject(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		int num = Grid.PosToCell(go);
		if (!Grid.IsValidCell(num))
		{
			return null;
		}
		CavityInfo cavityForCell = this.GetCavityForCell(num);
		if (cavityForCell == null)
		{
			return null;
		}
		return cavityForCell.room;
	}

	public bool IsInRoomType(GameObject go, RoomType checkType)
	{
		Room roomOfGameObject = this.GetRoomOfGameObject(go);
		if (roomOfGameObject != null)
		{
			RoomType roomType = roomOfGameObject.roomType;
			return checkType == roomType;
		}
		return false;
	}

	private CavityInfo GetCavityInfo(HandleVector<int>.Handle id)
	{
		CavityInfo cavityInfo = null;
		if (id.IsValid())
		{
			cavityInfo = this.cavityInfos.GetData(id);
		}
		return cavityInfo;
	}

	public CavityInfo GetCavityForCell(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return null;
		}
		HandleVector<int>.Handle handle = this.CellCavityID[cell];
		return this.GetCavityInfo(handle);
	}

	public List<Room> rooms = new List<Room>();

	private readonly KCompactedVector<CavityInfo> cavityInfos = new KCompactedVector<CavityInfo>(1024);

	private readonly HandleVector<int>.Handle[] CellCavityID;

	private readonly RoomProber.RefreshModule refresh;

	private readonly HashSet<int> solidChanges = new HashSet<int>();

	private bool dirty = true;

	public class Tuning : TuningData<RoomProber.Tuning>
	{
		public int maxRoomSize;
	}

	private struct RefreshModule
	{
		public RefreshModule(RoomProber roomProber)
		{
			this.roomProber = roomProber;
			this.cavityBuilder = new RoomProber.RefreshModule.CavityBuilder();
			this.dirtyCells = new List<int>();
			this.condemnedCavities = new List<HandleVector<int>.Handle>();
			this.newCavities = new List<CavityInfo>();
			this.dirtyEntities = new List<KPrefabID>();
			this.visitedCells = new HashSet<int>();
			this.visitedCavities = new HashSet<HandleVector<int>.Handle>();
			this.visitedBuildings = new HashSet<RoomProber.RefreshModule.BuildingId>();
			this.addCellToGrid = null;
		}

		public void Initialize()
		{
			this.addCellToGrid = new Func<int, bool>(this.AddCellToGrid);
		}

		public void Run()
		{
			this.CollectDirtyCells();
			this.CollectCondemnedCavities();
			this.BuildNewCavities();
			foreach (HandleVector<int>.Handle handle in this.condemnedCavities)
			{
				CavityInfo data = this.roomProber.cavityInfos.GetData(handle);
				this.dirtyEntities.Capacity = Math.Max(this.dirtyEntities.Capacity, this.dirtyEntities.Count + data.creatures.Count + data.otherEntities.Count);
				foreach (KPrefabID kprefabID in data.creatures)
				{
					this.dirtyEntities.Add(kprefabID);
				}
				foreach (KPrefabID kprefabID2 in data.otherEntities)
				{
					this.dirtyEntities.Add(kprefabID2);
				}
				if (data.room != null)
				{
					this.roomProber.ClearRoom(data.room);
				}
				this.roomProber.cavityInfos.Free(handle);
			}
			this.AddRoomContentsToCavities();
			this.roomProber.RefreshRooms(this.dirtyEntities);
			this.Recycle();
		}

		private readonly void Recycle()
		{
			this.dirtyCells.Clear();
			this.condemnedCavities.Clear();
			this.newCavities.Clear();
			this.dirtyEntities.Clear();
		}

		private readonly bool AddCellToGrid(int flood_cell)
		{
			if (RoomProber.IsCavityBoundary(flood_cell))
			{
				this.roomProber.CellCavityID[flood_cell] = HandleVector<int>.InvalidHandle;
				return false;
			}
			this.cavityBuilder.AddCell(flood_cell);
			this.roomProber.CellCavityID[flood_cell] = this.cavityBuilder.CavityID;
			return true;
		}

		private unsafe readonly void CollectDirtyCells()
		{
			int* ptr = stackalloc int[(UIntPtr)20];
			*ptr = 0;
			ptr[1] = -Grid.WidthInCells;
			ptr[2] = -1;
			ptr[3] = 1;
			ptr[4] = Grid.WidthInCells;
			foreach (int num in this.roomProber.solidChanges)
			{
				for (int i = 0; i < 5; i++)
				{
					int num2 = num + ptr[i];
					if (Grid.IsValidCell(num2) && this.visitedCells.Add(num2))
					{
						this.dirtyCells.Add(num2);
					}
				}
			}
			this.visitedCells.Clear();
			this.roomProber.solidChanges.Clear();
		}

		private readonly void CollectCondemnedCavities()
		{
			foreach (int num in this.dirtyCells)
			{
				if (!this.visitedCells.Contains(num))
				{
					HandleVector<int>.Handle handle = this.roomProber.CellCavityID[num];
					if (!handle.IsValid())
					{
						this.visitedCells.Add(num);
					}
					else
					{
						if (this.visitedCavities.Add(handle))
						{
							this.condemnedCavities.Add(handle);
						}
						CavityInfo data = this.roomProber.cavityInfos.GetData(handle);
						this.visitedCells.EnsureCapacity(this.visitedCells.Count + data.cells.Count);
						foreach (int num2 in data.cells)
						{
							this.roomProber.CellCavityID[num2] = HandleVector<int>.InvalidHandle;
							this.visitedCells.Add(num2);
						}
					}
				}
			}
			this.visitedCells.Clear();
			this.visitedCavities.Clear();
		}

		private readonly void BuildNewCavities()
		{
			int num = 0;
			foreach (HandleVector<int>.Handle handle in this.condemnedCavities)
			{
				num += this.roomProber.cavityInfos.GetData(handle).NumCells;
			}
			this.dirtyCells.Capacity = Math.Max(this.dirtyCells.Capacity, this.dirtyCells.Count + num);
			foreach (HandleVector<int>.Handle handle2 in this.condemnedCavities)
			{
				foreach (int num2 in this.roomProber.cavityInfos.GetData(handle2).cells)
				{
					this.dirtyCells.Add(num2);
				}
			}
			int num3 = ((this.condemnedCavities.Count > 0) ? 0 : (-1));
			foreach (int num4 in this.dirtyCells)
			{
				if (!this.visitedCells.Contains(num4))
				{
					HandleVector<int>.Handle handle3 = this.roomProber.CellCavityID[num4];
					if (!handle3.IsValid())
					{
						if (RoomProber.IsCavityBoundary(num4))
						{
							this.visitedCells.Add(num4);
							this.roomProber.CellCavityID[num4] = HandleVector<int>.InvalidHandle;
						}
						else
						{
							CavityInfo cavityInfo = this.roomProber.CreateNewCavity();
							if (num3 >= 0)
							{
								CavityInfo data = this.roomProber.cavityInfos.GetData(this.condemnedCavities[num3]);
								cavityInfo.cells = data.cells;
								cavityInfo.cells.Clear();
								data.cells = null;
								num3++;
								if (num3 >= this.condemnedCavities.Count)
								{
									num3 = -1;
								}
							}
							else
							{
								cavityInfo.cells = new List<int>();
							}
							this.cavityBuilder.Reset(cavityInfo.handle);
							GameUtil.FloodFillConditional(num4, this.addCellToGrid, this.visitedCells, cavityInfo.cells);
							DebugUtil.DevAssert(this.cavityBuilder.NumCells > 0, "Degenerate cavities should have been detected and rejected prior to this point", null);
							cavityInfo.minX = this.cavityBuilder.MinX;
							cavityInfo.minY = this.cavityBuilder.MinY;
							cavityInfo.maxX = this.cavityBuilder.MaxX;
							cavityInfo.maxY = this.cavityBuilder.MaxY;
							this.newCavities.Add(cavityInfo);
						}
					}
				}
			}
			this.visitedCells.Clear();
		}

		private void AddRoomContentsToCavities()
		{
			int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
			foreach (CavityInfo cavityInfo in this.newCavities)
			{
				if (cavityInfo.NumCells <= maxRoomSize)
				{
					foreach (int num in cavityInfo.cells)
					{
						GameObject gameObject = Grid.Objects[num, 1];
						if (!(gameObject == null))
						{
							KPrefabID component = gameObject.GetComponent<KPrefabID>();
							RoomProber.RefreshModule.BuildingId buildingId = new RoomProber.RefreshModule.BuildingId
							{
								prefab = component.GetHashCode(),
								instance = component.InstanceID
							};
							if (this.visitedBuildings.Add(buildingId))
							{
								if (component.HasTag(GameTags.RoomProberBuilding))
								{
									cavityInfo.AddBuilding(component);
								}
								else if (component.HasTag(GameTags.Plant))
								{
									cavityInfo.AddPlants(component);
								}
							}
						}
					}
				}
			}
			this.visitedBuildings.Clear();
		}

		private readonly RoomProber.RefreshModule.CavityBuilder cavityBuilder;

		private readonly RoomProber roomProber;

		private readonly List<int> dirtyCells;

		private readonly List<HandleVector<int>.Handle> condemnedCavities;

		private readonly List<CavityInfo> newCavities;

		private readonly List<KPrefabID> dirtyEntities;

		private readonly HashSet<int> visitedCells;

		private readonly HashSet<HandleVector<int>.Handle> visitedCavities;

		private readonly HashSet<RoomProber.RefreshModule.BuildingId> visitedBuildings;

		private Func<int, bool> addCellToGrid;

		private class CavityBuilder
		{
			public HandleVector<int>.Handle CavityID { get; private set; }

			public int MinX { get; private set; }

			public int MinY { get; private set; }

			public int MaxX { get; private set; }

			public int MaxY { get; private set; }

			public int NumCells { get; private set; }

			public void Reset(HandleVector<int>.Handle search_id)
			{
				this.CavityID = search_id;
				this.NumCells = 0;
				this.MinX = int.MaxValue;
				this.MinY = int.MaxValue;
				this.MaxX = 0;
				this.MaxY = 0;
			}

			public void AddCell(int flood_cell)
			{
				int num;
				int num2;
				Grid.CellToXY(flood_cell, out num, out num2);
				this.MinX = Math.Min(num, this.MinX);
				this.MinY = Math.Min(num2, this.MinY);
				this.MaxX = Math.Max(num, this.MaxX);
				this.MaxY = Math.Max(num2, this.MaxY);
				int num3 = this.NumCells + 1;
				this.NumCells = num3;
			}
		}

		private struct BuildingId
		{
			public int prefab;

			public int instance;
		}
	}
}
