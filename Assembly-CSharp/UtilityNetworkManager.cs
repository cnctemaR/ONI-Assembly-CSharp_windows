using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilityNetworkManager<NetworkType, ItemType> : IUtilityNetworkMgr where NetworkType : UtilityNetwork, new() where ItemType : MonoBehaviour
{
	public bool IsDirty
	{
		get
		{
			return this.dirty;
		}
	}

	public UtilityNetworkManager(int game_width, int game_height, int tile_layer)
	{
		this.tileLayer = tile_layer;
		this.networks = new List<UtilityNetwork>();
		this.Initialize(game_width, game_height);
	}

	public void Initialize(int game_width, int game_height)
	{
		this.networks.Clear();
		this.physicalGrid = new UtilityNetworkGridNode[game_width * game_height];
		this.visualGrid = new UtilityNetworkGridNode[game_width * game_height];
		this.stashedVisualGrid = new UtilityNetworkGridNode[game_width * game_height];
		this.physicalNodes = new HashSet<int>();
		this.visualNodes = new HashSet<int>();
		this.visitedCells = new HashSet<int>();
		for (int i = 0; i < this.visualGrid.Length; i++)
		{
			this.visualGrid[i] = new UtilityNetworkGridNode
			{
				networkIdx = -1,
				connections = (UtilityConnections)0
			};
			this.physicalGrid[i] = new UtilityNetworkGridNode
			{
				networkIdx = -1,
				connections = (UtilityConnections)0
			};
		}
	}

	public void Update()
	{
		if (this.dirty)
		{
			this.dirty = false;
			for (int i = 0; i < this.networks.Count; i++)
			{
				this.networks[i].Reset(this.physicalGrid);
			}
			this.networks.Clear();
			this.RebuildNetworks(this.tileLayer, false);
			this.RebuildNetworks(this.tileLayer, true);
			if (this.onNetworksRebuilt != null)
			{
				this.onNetworksRebuilt(this.networks, this.GetNodes(true));
			}
		}
	}

	protected UtilityNetworkGridNode[] GetGrid(bool is_physical_building)
	{
		if (!is_physical_building)
		{
			return this.visualGrid;
		}
		return this.physicalGrid;
	}

	private HashSet<int> GetNodes(bool is_physical_building)
	{
		if (!is_physical_building)
		{
			return this.visualNodes;
		}
		return this.physicalNodes;
	}

	public void ClearCell(int cell, bool is_physical_building)
	{
		if (Game.IsQuitting())
		{
			return;
		}
		UtilityNetworkGridNode[] grid = this.GetGrid(is_physical_building);
		HashSet<int> nodes = this.GetNodes(is_physical_building);
		UtilityConnections connections = grid[cell].connections;
		grid[cell].connections = (UtilityConnections)0;
		Vector2I vector2I = Grid.CellToXY(cell);
		if (vector2I.x > 1 && (connections & UtilityConnections.Left) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array = grid;
			int num = Grid.CellLeft(cell);
			array[num].connections = array[num].connections & ~UtilityConnections.Right;
		}
		if (vector2I.x < Grid.WidthInCells - 1 && (connections & UtilityConnections.Right) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array2 = grid;
			int num2 = Grid.CellRight(cell);
			array2[num2].connections = array2[num2].connections & ~UtilityConnections.Left;
		}
		if (vector2I.y > 1 && (connections & UtilityConnections.Down) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array3 = grid;
			int num3 = Grid.CellBelow(cell);
			array3[num3].connections = array3[num3].connections & ~UtilityConnections.Up;
		}
		if (vector2I.y < Grid.HeightInCells - 1 && (connections & UtilityConnections.Up) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array4 = grid;
			int num4 = Grid.CellAbove(cell);
			array4[num4].connections = array4[num4].connections & ~UtilityConnections.Down;
		}
		nodes.Remove(cell);
		if (is_physical_building)
		{
			this.dirty = true;
			this.ClearCell(cell, false);
		}
	}

	private void QueueCellForVisit(UtilityNetworkGridNode[] grid, int dest_cell, UtilityConnections direction)
	{
		if (!Grid.IsValidCell(dest_cell))
		{
			return;
		}
		if (this.visitedCells.Contains(dest_cell))
		{
			return;
		}
		if (direction != (UtilityConnections)0 && (grid[dest_cell].connections & direction.InverseDirection()) == (UtilityConnections)0)
		{
			return;
		}
		if (Grid.Objects[dest_cell, this.tileLayer] != null)
		{
			this.visitedCells.Add(dest_cell);
			this.queued.Enqueue(dest_cell);
		}
	}

	public void ForceRebuildNetworks()
	{
		this.dirty = true;
	}

	public void AddToNetworks(int cell, object item, bool is_endpoint)
	{
		if (item != null)
		{
			if (is_endpoint)
			{
				if (this.endpoints.ContainsKey(cell))
				{
					DebugUtil.DevLogError(string.Format("Cell {0} already has a utility network endpoint assigned. Adding {1} will stomp previous endpoint, destroying the object that's already there.", cell, item.ToString()));
					Util.KDestroyGameObject(((KMonoBehaviour)this.endpoints[cell]).gameObject);
				}
				this.endpoints[cell] = item;
			}
			else
			{
				if (this.items.ContainsKey(cell))
				{
					DebugUtil.DevLogError(string.Format("Cell {0} already has a utility network connector assigned. Adding {1} will stomp previous item, destroying the object that's already there.", cell, item.ToString()));
					Util.KDestroyGameObject(((KMonoBehaviour)this.items[cell]).gameObject);
				}
				this.items[cell] = item;
			}
		}
		this.dirty = true;
	}

	private unsafe void Reconnect(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		int* ptr = stackalloc int[(UIntPtr)16];
		int* ptr2 = stackalloc int[(UIntPtr)16];
		int* ptr3 = stackalloc int[(UIntPtr)16];
		int num = 0;
		if (vector2I.y < Grid.HeightInCells - 1)
		{
			ptr[num] = Grid.CellAbove(cell);
			ptr2[num] = 4;
			ptr3[num] = 8;
			num++;
		}
		if (vector2I.y > 1)
		{
			ptr[num] = Grid.CellBelow(cell);
			ptr2[num] = 8;
			ptr3[num] = 4;
			num++;
		}
		if (vector2I.x > 1)
		{
			ptr[num] = Grid.CellLeft(cell);
			ptr2[num] = 1;
			ptr3[num] = 2;
			num++;
		}
		if (vector2I.x < Grid.WidthInCells - 1)
		{
			ptr[num] = Grid.CellRight(cell);
			ptr2[num] = 2;
			ptr3[num] = 1;
			num++;
		}
		UtilityConnections connections = this.physicalGrid[cell].connections;
		UtilityConnections connections2 = this.visualGrid[cell].connections;
		for (int i = 0; i < num; i++)
		{
			int num2 = ptr[i];
			UtilityConnections utilityConnections = (UtilityConnections)ptr2[i];
			UtilityConnections utilityConnections2 = (UtilityConnections)ptr3[i];
			if ((connections & utilityConnections) != (UtilityConnections)0)
			{
				if (this.physicalNodes.Contains(num2))
				{
					UtilityNetworkGridNode[] array = this.physicalGrid;
					int num3 = num2;
					array[num3].connections = array[num3].connections | utilityConnections2;
				}
				if (this.visualNodes.Contains(num2))
				{
					UtilityNetworkGridNode[] array2 = this.visualGrid;
					int num4 = num2;
					array2[num4].connections = array2[num4].connections | utilityConnections2;
				}
			}
			else if ((connections2 & utilityConnections) != (UtilityConnections)0 && (this.physicalNodes.Contains(num2) || this.visualNodes.Contains(num2)))
			{
				UtilityNetworkGridNode[] array3 = this.visualGrid;
				int num5 = num2;
				array3[num5].connections = array3[num5].connections | utilityConnections2;
			}
		}
	}

	public void RemoveFromNetworks(int cell, object item, bool is_endpoint)
	{
		if (Game.IsQuitting())
		{
			return;
		}
		this.dirty = true;
		if (item != null)
		{
			if (is_endpoint)
			{
				this.endpoints.Remove(cell);
				int networkIdx = this.physicalGrid[cell].networkIdx;
				if (networkIdx != -1)
				{
					this.networks[networkIdx].RemoveItem(cell, item);
					return;
				}
			}
			else
			{
				int networkIdx2 = this.physicalGrid[cell].networkIdx;
				this.physicalGrid[cell].connections = (UtilityConnections)0;
				this.physicalGrid[cell].networkIdx = -1;
				this.items.Remove(cell);
				this.Disconnect(cell);
				object obj;
				if (this.endpoints.TryGetValue(cell, out obj) && networkIdx2 != -1)
				{
					this.networks[networkIdx2].DisconnectItem(cell, obj);
				}
			}
		}
	}

	private unsafe void Disconnect(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		int num = 0;
		int* ptr = stackalloc int[(UIntPtr)16];
		int* ptr2 = stackalloc int[(UIntPtr)16];
		if (vector2I.y < Grid.HeightInCells - 1)
		{
			ptr[num] = Grid.CellAbove(cell);
			ptr2[num] = -9;
			num++;
		}
		if (vector2I.y > 1)
		{
			ptr[num] = Grid.CellBelow(cell);
			ptr2[num] = -5;
			num++;
		}
		if (vector2I.x > 1)
		{
			ptr[num] = Grid.CellLeft(cell);
			ptr2[num] = -3;
			num++;
		}
		if (vector2I.x < Grid.WidthInCells - 1)
		{
			ptr[num] = Grid.CellRight(cell);
			ptr2[num] = -2;
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = ptr[i];
			int num3 = ptr2[i];
			int num4 = (int)(this.physicalGrid[num2].connections & (UtilityConnections)num3);
			this.physicalGrid[num2].connections = (UtilityConnections)num4;
		}
	}

	private unsafe void RebuildNetworks(int layer, bool is_physical)
	{
		UtilityNetworkGridNode[] grid = this.GetGrid(is_physical);
		HashSet<int> nodes = this.GetNodes(is_physical);
		this.visitedCells.Clear();
		this.queued.Clear();
		int* ptr = stackalloc int[(UIntPtr)16];
		int* ptr2 = stackalloc int[(UIntPtr)16];
		foreach (int num in nodes)
		{
			UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
			if (!this.visitedCells.Contains(num))
			{
				this.queued.Enqueue(num);
				this.visitedCells.Add(num);
				NetworkType networkType = new NetworkType();
				networkType.id = this.networks.Count;
				this.networks.Add(networkType);
				while (this.queued.Count > 0)
				{
					int num2 = this.queued.Dequeue();
					utilityNetworkGridNode = grid[num2];
					object obj = null;
					object obj2 = null;
					if (is_physical)
					{
						if (this.items.TryGetValue(num2, out obj))
						{
							if (obj is IDisconnectable && (obj as IDisconnectable).IsDisconnected())
							{
								continue;
							}
							if (obj != null)
							{
								networkType.AddItem(num2, obj);
							}
						}
						if (this.endpoints.TryGetValue(num2, out obj2) && obj2 != null)
						{
							networkType.AddItem(num2, obj2);
						}
					}
					grid[num2].networkIdx = networkType.id;
					if (obj != null && obj2 != null)
					{
						networkType.ConnectItem(num2, obj2);
					}
					Vector2I vector2I = Grid.CellToXY(num2);
					int num3 = 0;
					if (vector2I.x >= 0)
					{
						ptr[num3] = Grid.CellLeft(num2);
						ptr2[num3] = 1;
						num3++;
					}
					if (vector2I.x < Grid.WidthInCells)
					{
						ptr[num3] = Grid.CellRight(num2);
						ptr2[num3] = 2;
						num3++;
					}
					if (vector2I.y >= 0)
					{
						ptr[num3] = Grid.CellBelow(num2);
						ptr2[num3] = 8;
						num3++;
					}
					if (vector2I.y < Grid.HeightInCells)
					{
						ptr[num3] = Grid.CellAbove(num2);
						ptr2[num3] = 4;
						num3++;
					}
					for (int i = 0; i < num3; i++)
					{
						int num4 = ptr2[i];
						if ((utilityNetworkGridNode.connections & (UtilityConnections)num4) != (UtilityConnections)0)
						{
							int num5 = ptr[i];
							this.QueueCellForVisit(grid, num5, (UtilityConnections)num4);
						}
					}
					int num6;
					if (this.links.TryGetValue(num2, out num6))
					{
						this.QueueCellForVisit(grid, num6, (UtilityConnections)0);
					}
				}
			}
		}
	}

	public UtilityNetwork GetNetworkByID(int id)
	{
		UtilityNetwork utilityNetwork = null;
		if (0 <= id && id < this.networks.Count)
		{
			utilityNetwork = this.networks[id];
		}
		return utilityNetwork;
	}

	public UtilityNetwork GetNetworkForCell(int cell)
	{
		UtilityNetwork utilityNetwork = null;
		if (Grid.IsValidCell(cell) && 0 <= this.physicalGrid[cell].networkIdx && this.physicalGrid[cell].networkIdx < this.networks.Count)
		{
			utilityNetwork = this.networks[this.physicalGrid[cell].networkIdx];
		}
		return utilityNetwork;
	}

	public UtilityNetwork GetNetworkForDirection(int cell, Direction direction)
	{
		cell = Grid.GetCellInDirection(cell, direction);
		if (!Grid.IsValidCell(cell))
		{
			return null;
		}
		UtilityNetworkGridNode utilityNetworkGridNode = this.GetGrid(true)[cell];
		UtilityNetwork utilityNetwork = null;
		if (utilityNetworkGridNode.networkIdx != -1 && utilityNetworkGridNode.networkIdx < this.networks.Count)
		{
			utilityNetwork = this.networks[utilityNetworkGridNode.networkIdx];
		}
		return utilityNetwork;
	}

	private UtilityConnections GetNeighboursAsConnections(int cell, HashSet<int> nodes)
	{
		UtilityConnections utilityConnections = (UtilityConnections)0;
		Vector2I vector2I = Grid.CellToXY(cell);
		if (vector2I.x > 1 && nodes.Contains(Grid.CellLeft(cell)))
		{
			utilityConnections |= UtilityConnections.Left;
		}
		if (vector2I.x < Grid.WidthInCells - 1 && nodes.Contains(Grid.CellRight(cell)))
		{
			utilityConnections |= UtilityConnections.Right;
		}
		if (vector2I.y > 1 && nodes.Contains(Grid.CellBelow(cell)))
		{
			utilityConnections |= UtilityConnections.Down;
		}
		if (vector2I.y < Grid.HeightInCells - 1 && nodes.Contains(Grid.CellAbove(cell)))
		{
			utilityConnections |= UtilityConnections.Up;
		}
		return utilityConnections;
	}

	public virtual void SetConnections(UtilityConnections connections, int cell, bool is_physical_building)
	{
		HashSet<int> nodes = this.GetNodes(is_physical_building);
		nodes.Add(cell);
		this.visualGrid[cell].connections = connections;
		if (is_physical_building)
		{
			this.dirty = true;
			UtilityConnections utilityConnections = (is_physical_building ? (connections & this.GetNeighboursAsConnections(cell, nodes)) : connections);
			this.physicalGrid[cell].connections = utilityConnections;
		}
		this.Reconnect(cell);
	}

	public UtilityConnections GetConnections(int cell, bool is_physical_building)
	{
		UtilityNetworkGridNode[] array = this.GetGrid(is_physical_building);
		UtilityConnections utilityConnections = array[cell].connections;
		if (!is_physical_building)
		{
			array = this.GetGrid(true);
			utilityConnections |= array[cell].connections;
		}
		return utilityConnections;
	}

	public UtilityConnections GetDisplayConnections(int cell)
	{
		UtilityConnections utilityConnections = (UtilityConnections)0;
		UtilityNetworkGridNode[] array = this.GetGrid(false);
		UtilityConnections utilityConnections2 = utilityConnections | array[cell].connections;
		array = this.GetGrid(true);
		return utilityConnections2 | array[cell].connections;
	}

	public virtual bool CanAddConnection(UtilityConnections new_connection, int cell, bool is_physical_building, out string fail_reason)
	{
		fail_reason = null;
		return true;
	}

	public void AddConnection(UtilityConnections new_connection, int cell, bool is_physical_building)
	{
		string text;
		if (this.CanAddConnection(new_connection, cell, is_physical_building, out text))
		{
			if (is_physical_building)
			{
				this.dirty = true;
			}
			UtilityNetworkGridNode[] grid = this.GetGrid(is_physical_building);
			UtilityConnections connections = grid[cell].connections;
			grid[cell].connections = connections | new_connection;
		}
	}

	public void StashVisualGrids()
	{
		Array.Copy(this.visualGrid, this.stashedVisualGrid, this.visualGrid.Length);
	}

	public void UnstashVisualGrids()
	{
		Array.Copy(this.stashedVisualGrid, this.visualGrid, this.visualGrid.Length);
	}

	public string GetVisualizerString(int cell)
	{
		UtilityConnections displayConnections = this.GetDisplayConnections(cell);
		return this.GetVisualizerString(displayConnections);
	}

	public string GetVisualizerString(UtilityConnections connections)
	{
		string text = "";
		if ((connections & UtilityConnections.Left) != (UtilityConnections)0)
		{
			text += "L";
		}
		if ((connections & UtilityConnections.Right) != (UtilityConnections)0)
		{
			text += "R";
		}
		if ((connections & UtilityConnections.Up) != (UtilityConnections)0)
		{
			text += "U";
		}
		if ((connections & UtilityConnections.Down) != (UtilityConnections)0)
		{
			text += "D";
		}
		if (text == "")
		{
			text = "None";
		}
		return text;
	}

	public object GetEndpoint(int cell)
	{
		object obj = null;
		this.endpoints.TryGetValue(cell, out obj);
		return obj;
	}

	public void AddLink(int cell1, int cell2)
	{
		this.links[cell1] = cell2;
		this.links[cell2] = cell1;
		this.dirty = true;
	}

	public void RemoveLink(int cell1, int cell2)
	{
		this.links.Remove(cell1);
		this.links.Remove(cell2);
		this.dirty = true;
	}

	public void AddNetworksRebuiltListener(Action<IList<UtilityNetwork>, ICollection<int>> listener)
	{
		this.onNetworksRebuilt = (Action<IList<UtilityNetwork>, ICollection<int>>)Delegate.Combine(this.onNetworksRebuilt, listener);
	}

	public void RemoveNetworksRebuiltListener(Action<IList<UtilityNetwork>, ICollection<int>> listener)
	{
		this.onNetworksRebuilt = (Action<IList<UtilityNetwork>, ICollection<int>>)Delegate.Remove(this.onNetworksRebuilt, listener);
	}

	public IList<UtilityNetwork> GetNetworks()
	{
		return this.networks;
	}

	private Dictionary<int, object> items = new Dictionary<int, object>();

	private Dictionary<int, object> endpoints = new Dictionary<int, object>();

	private Dictionary<int, int> links = new Dictionary<int, int>();

	private List<UtilityNetwork> networks;

	private HashSet<int> visitedCells;

	private Action<IList<UtilityNetwork>, ICollection<int>> onNetworksRebuilt;

	private Queue<int> queued = new Queue<int>();

	protected UtilityNetworkGridNode[] visualGrid;

	private UtilityNetworkGridNode[] stashedVisualGrid;

	protected UtilityNetworkGridNode[] physicalGrid;

	protected HashSet<int> physicalNodes;

	protected HashSet<int> visualNodes;

	private bool dirty;

	private int tileLayer = -1;
}
