using System;
using System.Collections.Generic;

public class RoomProber
{
	public RoomProber(int numCells)
	{
		this.cells = new RoomProber.ProbeCell[numCells];
	}

	public void AddFoundationCell(int cell)
	{
		RoomProber.ProbeCell[] array = this.cells;
		array[cell].flags = array[cell].flags | RoomProber.Flags.Foundation;
	}

	public void RemoveFoundationCell(int cell)
	{
		RoomProber.ProbeCell[] array = this.cells;
		array[cell].flags = array[cell].flags & ~RoomProber.Flags.Foundation;
	}

	public void AddDoor(Door door)
	{
		this.doors.Add(door);
		this.BuildRooms();
	}

	public void RemoveDoor(Door door)
	{
		this.doors.Remove(door);
		this.BuildRooms();
	}

	public unsafe void BuildRooms()
	{
		foreach (RoomProber.Room room in this.rooms)
		{
			foreach (int num in room.cells)
			{
				Grid.Room[num] = ushort.MaxValue;
				RoomProber.ProbeCell[] array = this.cells;
				int num2 = num;
				array[num2].flags = array[num2].flags & ~RoomProber.Flags.Visited;
			}
		}
		this.rooms.Clear();
		int* ptr = stackalloc int[checked(2 * 4)];
		*ptr = -1;
		ptr[1] = 1;
		foreach (Door door in this.doors)
		{
			int num3 = ((!door.IsRotated) ? 1 : 0);
			int num4 = ((!door.IsRotated) ? 0 : 1);
			for (int i = 0; i < 2; i++)
			{
				int num5 = ptr[i];
				this.activeCells.Clear();
				foreach (int num6 in door.GetComponent<Building>().PlacementCells)
				{
					int num7 = Grid.OffsetCell(num6, num5 * num3, num5 * num4);
					if (Grid.IsValidCell(num7) && (byte)(this.cells[num7].flags & RoomProber.Flags.Visited) == 0 && (byte)(this.cells[num7].flags & RoomProber.Flags.Foundation) == 0)
					{
						RoomProber.ProbeCell[] array2 = this.cells;
						int num8 = num7;
						array2[num8].flags = array2[num8].flags | RoomProber.Flags.Visited;
						this.activeCells.Add(num7);
					}
				}
				RoomProber.Room room2 = this.BuildRoom(this.activeCells);
				if (room2 != null)
				{
					this.rooms.Add(room2);
				}
			}
		}
		for (int k = 0; k < this.rooms.Count; k++)
		{
			foreach (int num9 in this.rooms[k].cells)
			{
				Grid.Room[num9] = (ushort)k;
			}
		}
	}

	private unsafe RoomProber.Room BuildRoom(List<int> activeCells)
	{
		if (activeCells.Count == 0)
		{
			return null;
		}
		int* ptr = stackalloc int[checked(4 * 4)];
		RoomProber.Room room = new RoomProber.Room();
		while (activeCells.Count > 0)
		{
			int num = activeCells[0];
			if (Grid.Solid[num] && (byte)(this.cells[num].flags & RoomProber.Flags.Foundation) == 0)
			{
				foreach (int num2 in room.cells)
				{
					RoomProber.ProbeCell[] array = this.cells;
					int num3 = num2;
					array[num3].flags = array[num3].flags & ~RoomProber.Flags.Visited;
				}
				foreach (int num4 in activeCells)
				{
					RoomProber.ProbeCell[] array2 = this.cells;
					int num5 = num4;
					array2[num5].flags = array2[num5].flags & ~RoomProber.Flags.Visited;
				}
				return null;
			}
			activeCells.RemoveAt(0);
			room.cells.Add(num);
			*ptr = Grid.OffsetCell(num, -1, 0);
			ptr[1] = Grid.OffsetCell(num, 1, 0);
			ptr[2] = Grid.OffsetCell(num, 0, -1);
			ptr[3] = Grid.OffsetCell(num, 0, 1);
			for (int i = 0; i < 4; i++)
			{
				int num6 = ptr[i];
				if (Grid.IsValidCell(num6) && (byte)(this.cells[num6].flags & RoomProber.Flags.Visited) == 0 && (byte)(this.cells[num6].flags & RoomProber.Flags.Foundation) == 0)
				{
					RoomProber.ProbeCell[] array3 = this.cells;
					int num7 = num6;
					array3[num7].flags = array3[num7].flags | RoomProber.Flags.Visited;
					activeCells.Add(num6);
				}
			}
		}
		return room;
	}

	public RoomProber.Room GetContainingRoom(int cell)
	{
		RoomProber.Room room = null;
		ushort num = Grid.Room[cell];
		if (num != 65535)
		{
			room = this.rooms[(int)num];
		}
		return room;
	}

	public const ushort InvalidID = 65535;

	public List<RoomProber.Room> rooms = new List<RoomProber.Room>();

	private List<Door> doors = new List<Door>();

	private RoomProber.ProbeCell[] cells;

	private List<int> activeCells = new List<int>();

	[Flags]
	public enum Flags : byte
	{
		None = 0,
		Visited = 1,
		Foundation = 2
	}

	public struct ProbeCell
	{
		public RoomProber.Flags flags;
	}

	public class Room
	{
		public HashSet<int> cells = new HashSet<int>();
	}
}
