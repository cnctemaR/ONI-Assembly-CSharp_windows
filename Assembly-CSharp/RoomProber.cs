using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomProber
{
	public bool isBuilderDirty()
	{
		return this.builderDirty;
	}

	public void MarkBuilderDirty()
	{
		this.builderDirty = true;
	}

	public void Init()
	{
		this.CellCavityID = new HandleVector<int>.Handle[Grid.CellCount];
		for (int i = 0; i < this.CellCavityID.Length; i++)
		{
			this.CellCavityID[i] = HandleVector<int>.InvalidHandle;
		}
		this.floodFiller = new RoomProber.CavityFloodFiller(this.CellCavityID);
		for (int j = 0; j < this.CellCavityID.Length; j++)
		{
			this.solidChanges.Add(j);
		}
		this.ProcessSolidChanges();
		this.RefreshRooms();
		World instance = World.Instance;
		instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.SolidChangedEvent));
	}

	private void SolidChangedEvent(int cell)
	{
		this.SolidChangedEvent(cell, true);
	}

	public void SolidChangedEvent(int cell, bool ignoreDoors)
	{
		if (ignoreDoors && Grid.HasDoor[cell])
		{
			return;
		}
		this.solidChanges.Add(cell);
		this.MarkBuilderDirty();
	}

	private CavityInfo CreateNewCavity()
	{
		CavityInfo cavityInfo = new CavityInfo();
		cavityInfo.handle = this.cavityInfos.Allocate(cavityInfo);
		return cavityInfo;
	}

	private unsafe void ProcessSolidChanges()
	{
		int* ptr = stackalloc int[checked(5 * 4)];
		*ptr = 0;
		ptr[1] = -Grid.WidthInCells;
		ptr[2] = -1;
		ptr[3] = 1;
		ptr[4] = Grid.WidthInCells;
		foreach (int num in this.solidChanges)
		{
			for (int i = 0; i < 5; i++)
			{
				int num2 = num + ptr[i];
				if (Grid.IsValidCell(num2))
				{
					this.floodFillSet.Add(num2);
					HandleVector<int>.Handle handle = this.CellCavityID[num2];
					if (handle.IsValid())
					{
						this.CellCavityID[num2] = HandleVector<int>.InvalidHandle;
						this.releasedIDs.Add(handle);
					}
				}
			}
		}
		CavityInfo cavityInfo = this.CreateNewCavity();
		foreach (int num3 in this.floodFillSet)
		{
			if (!this.visitedCells.Contains(num3))
			{
				HandleVector<int>.Handle handle2 = this.CellCavityID[num3];
				if (!handle2.IsValid())
				{
					CavityInfo cavityInfo2 = cavityInfo;
					this.floodFiller.Reset(cavityInfo2.handle);
					GameUtil.FloodFillConditional(num3, new Func<int, bool>(this.floodFiller.ShouldContinue), this.visitedCells);
					if (this.floodFiller.NumCells > 0)
					{
						cavityInfo2.numCells = this.floodFiller.NumCells;
						cavityInfo = this.CreateNewCavity();
					}
				}
			}
		}
		if (cavityInfo.numCells == 0)
		{
			this.releasedIDs.Add(cavityInfo.handle);
		}
		foreach (HandleVector<int>.Handle handle3 in this.releasedIDs)
		{
			CavityInfo data = this.cavityInfos.GetData(handle3);
			data.ReleaseResources();
			this.cavityInfos.Free(handle3);
		}
		this.RebuildDirtyCavities(this.visitedCells);
		this.releasedIDs.Clear();
		this.visitedCells.Clear();
		this.solidChanges.Clear();
		this.floodFillSet.Clear();
	}

	private void RebuildDirtyCavities(ICollection<int> visited_cells)
	{
		foreach (int num in visited_cells)
		{
			HandleVector<int>.Handle handle = this.CellCavityID[num];
			if (handle.IsValid())
			{
				CavityInfo data = this.cavityInfos.GetData(handle);
				if (0 < data.numCells && data.numCells <= RoomProber.MaxRoomSize)
				{
					GameObject gameObject = Grid.Objects[num, 1];
					if (gameObject != null)
					{
						BuildingComplete component = gameObject.GetComponent<BuildingComplete>();
						if (component != null && !data.buildings.Contains(component))
						{
							data.AddBuilding(component);
						}
					}
				}
			}
		}
		visited_cells.Clear();
	}

	public void AddBuilding(BuildingComplete bc)
	{
		HandleVector<int>.Handle handle = this.CellCavityID[Grid.PosToCell(bc.gameObject)];
		if (handle.IsValid())
		{
			CavityInfo data = this.cavityInfos.GetData(handle);
			data.AddBuilding(bc);
			this.MarkBuilderDirty();
		}
	}

	public void RemoveBuilding(BuildingComplete bc)
	{
		HandleVector<int>.Handle handle = this.CellCavityID[Grid.PosToCell(bc.gameObject)];
		if (handle.IsValid())
		{
			CavityInfo data = this.cavityInfos.GetData(handle);
			data.RemoveBuilding(bc);
			this.MarkBuilderDirty();
		}
	}

	public void Update()
	{
		if (this.builderDirty)
		{
			this.ProcessSolidChanges();
			this.RefreshRooms();
		}
	}

	private void ClearRooms()
	{
		this.UnassignBuildingsFromRooms();
		foreach (Room room in this.rooms)
		{
			room.CleanUp();
		}
		this.rooms.Clear();
	}

	public void ClearRoom(Room room)
	{
		if (room == null || room.id == 65535)
		{
			return;
		}
		foreach (BuildingComplete buildingComplete in room.buildings)
		{
			if (!(buildingComplete == null))
			{
				Assignable assignable = buildingComplete.assignable;
				if (assignable != null && assignable.assignee == room)
				{
					assignable.Unassign();
					assignable.Trigger(2070884250, null);
				}
			}
		}
		room.CleanUp();
		room.cavity.SetRoom(null);
	}

	public void AddDoor(Door door, ICollection<int> door_cells, ICollection<int> adjacent_cells)
	{
		this.doors.Add(door);
		foreach (int num in door_cells)
		{
			this.SolidChangedEvent(num, false);
		}
		foreach (int num2 in adjacent_cells)
		{
			this.SolidChangedEvent(num2, false);
		}
		this.RefreshDoors();
	}

	public void RemoveDoor(Door door)
	{
		this.doors.Remove(door);
		foreach (CellOffset cellOffset in door.GetComponent<OccupyArea>().OccupiedCellsOffsets)
		{
			this.SolidChangedEvent(Grid.OffsetCell(Grid.PosToCell(door), cellOffset), false);
		}
	}

	public void RefreshDoors()
	{
		foreach (CavityInfo cavityInfo in this.cavityInfos.GetDataList())
		{
			cavityInfo.hasDoor = false;
		}
		foreach (Door door in this.doors)
		{
			if (door.GetComponent<Rotatable>().IsRotated)
			{
				this.SetHasDoor(this.CellCavityID[Grid.CellAbove(Grid.PosToCell(door))], true);
				this.SetHasDoor(this.CellCavityID[Grid.CellBelow(Grid.PosToCell(door))], true);
			}
			else
			{
				this.SetHasDoor(this.CellCavityID[Grid.CellLeft(Grid.PosToCell(door))], true);
				this.SetHasDoor(this.CellCavityID[Grid.CellRight(Grid.PosToCell(door))], true);
			}
		}
	}

	private void SetHasDoor(HandleVector<int>.Handle id, bool value)
	{
		if (!id.IsValid())
		{
			return;
		}
		CavityInfo data = this.cavityInfos.GetData(id);
		data.hasDoor = value;
	}

	private void RefreshRooms()
	{
		this.RefreshDoors();
		foreach (CavityInfo cavityInfo in this.cavityInfos.GetDataList())
		{
			if (cavityInfo.dirty)
			{
				this.ClearRoom(cavityInfo.room);
				if (cavityInfo.numCells > 0)
				{
					if (cavityInfo.numCells <= RoomProber.MaxRoomSize && cavityInfo.hasDoor)
					{
						this.rooms.Add(this.BuildRoom(cavityInfo));
					}
					this.AssignBuildingsToRoom(cavityInfo.room);
					foreach (BuildingComplete buildingComplete in cavityInfo.buildings)
					{
						buildingComplete.Trigger(144050788, cavityInfo.room);
					}
				}
				cavityInfo.dirty = false;
			}
		}
		this.builderDirty = false;
	}

	private void AssignBuildingsToRoom(Room room)
	{
		if (room == null || room.id == 65535)
		{
			return;
		}
		RoomTypes.RoomType roomType = RoomTypes.GetRoomType(room);
		if (roomType == RoomTypes.neutral_type)
		{
			return;
		}
		foreach (BuildingComplete buildingComplete in room.buildings)
		{
			Assignable assignable = buildingComplete.assignable;
			if (assignable != null && (roomType.primary_constraint == null || !roomType.primary_constraint.building_criteria(buildingComplete)))
			{
				assignable.Assign(room);
				assignable.Trigger(2070884250, null);
			}
		}
	}

	private void UnassignBuildingsFromRooms()
	{
		foreach (CavityInfo cavityInfo in this.cavityInfos.GetDataList())
		{
			if (cavityInfo.room != null && cavityInfo.room.id != 65535)
			{
				RoomTypes.RoomType roomType = RoomTypes.GetRoomType(cavityInfo.room);
				if (roomType != RoomTypes.neutral_type)
				{
					this.UnassignBuildingsFromRoom(cavityInfo.room);
				}
			}
		}
	}

	private void UnassignBuildingsFromRoom(Room room)
	{
		if (room == null)
		{
			return;
		}
		foreach (BuildingComplete buildingComplete in room.buildings)
		{
			if (!(buildingComplete == null))
			{
				Assignable assignable = buildingComplete.assignable;
				if (assignable != null && assignable.assignee == room)
				{
					assignable.Unassign();
					assignable.Trigger(2070884250, null);
				}
			}
		}
	}

	private Room BuildRoom(CavityInfo cavity)
	{
		Room room = new Room();
		cavity.SetRoom(room);
		return room;
	}

	public Room GetRoomOfBuilding(BuildingComplete bc)
	{
		foreach (CavityInfo cavityInfo in this.cavityInfos.GetDataList())
		{
			if (cavityInfo.numCells != 0)
			{
				if (cavityInfo.buildings.Contains(bc))
				{
					return cavityInfo.room;
				}
			}
		}
		return null;
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
		HandleVector<int>.Handle handle = this.CellCavityID[cell];
		return this.GetCavityInfo(handle);
	}

	public const ushort InvalidRoomID = 65535;

	private bool builderDirty = true;

	public List<Room> rooms = new List<Room>();

	private List<Door> doors = new List<Door>();

	public static int MaxRoomSize = 128;

	private KCompactedVector<CavityInfo> cavityInfos = new KCompactedVector<CavityInfo>(1024);

	private HandleVector<int>.Handle[] CellCavityID;

	private HashSet<int> solidChanges = new HashSet<int>();

	private HashSet<int> visitedCells = new HashSet<int>();

	private HashSet<int> floodFillSet = new HashSet<int>();

	private HashSet<HandleVector<int>.Handle> releasedIDs = new HashSet<HandleVector<int>.Handle>();

	private RoomProber.CavityFloodFiller floodFiller;

	private class CavityFloodFiller
	{
		public CavityFloodFiller(HandleVector<int>.Handle[] grid)
		{
			this.grid = grid;
		}

		public void Reset(HandleVector<int>.Handle search_id)
		{
			this.cavityID = search_id;
			this.numCells = 0;
		}

		private static bool IsWall(int cell)
		{
			return Grid.Solid[cell] || Grid.HasDoor[cell] || Grid.Foundation[cell];
		}

		public bool ShouldContinue(int flood_cell)
		{
			bool flag = false;
			if (!RoomProber.CavityFloodFiller.IsWall(flood_cell))
			{
				flag = true;
				this.grid[flood_cell] = this.cavityID;
				this.numCells++;
			}
			else
			{
				this.grid[flood_cell] = HandleVector<int>.InvalidHandle;
			}
			return flag;
		}

		public int NumCells
		{
			get
			{
				return this.numCells;
			}
		}

		private HandleVector<int>.Handle[] grid;

		private HandleVector<int>.Handle cavityID;

		private int numCells;
	}
}
