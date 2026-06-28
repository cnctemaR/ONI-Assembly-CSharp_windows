using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilityNetworkManager<NetworkType, ItemType> : IUtilityNetworkMgr where NetworkType : UtilityNetwork, new() where ItemType : MonoBehaviour
{
	public UtilityNetworkManager(int game_width, int game_height, int tile_layer)
	{
		this.tileLayer = tile_layer;
		this.networks = new List<UtilityNetwork>();
		this.Initialize(game_width, game_height);
	}

	public bool IsDirty
	{
		get
		{
			return this.dirty;
		}
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

	private UtilityNetworkGridNode[] GetGrid(bool is_physical_building)
	{
		return (!is_physical_building) ? this.visualGrid : this.physicalGrid;
	}

	private HashSet<int> GetNodes(bool is_physical_building)
	{
		return (!is_physical_building) ? this.visualNodes : this.physicalNodes;
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
		int num = Grid.CellAbove(cell);
		int num2 = Grid.CellBelow(cell);
		int num3 = Grid.CellLeft(cell);
		int num4 = Grid.CellRight(cell);
		if (Grid.IsValidCell(num) && (connections & UtilityConnections.Up) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array = grid;
			int num5 = num;
			array[num5].connections = array[num5].connections & ~UtilityConnections.Down;
		}
		if (Grid.IsValidCell(num2) && (connections & UtilityConnections.Down) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array2 = grid;
			int num6 = num2;
			array2[num6].connections = array2[num6].connections & ~UtilityConnections.Up;
		}
		if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Left) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array3 = grid;
			int num7 = num3;
			array3[num7].connections = array3[num7].connections & ~UtilityConnections.Right;
		}
		if (Grid.IsValidCell(num4) && (connections & UtilityConnections.Right) != (UtilityConnections)0)
		{
			UtilityNetworkGridNode[] array4 = grid;
			int num8 = num4;
			array4[num8].connections = array4[num8].connections & ~UtilityConnections.Left;
		}
		nodes.Remove(cell);
		if (is_physical_building)
		{
			this.dirty = true;
			this.ClearCell(cell, false);
		}
	}

	private UtilityConnections InverseDirection(UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Right;
		case UtilityConnections.Right:
			return UtilityConnections.Left;
		case UtilityConnections.Up:
			return UtilityConnections.Down;
		case UtilityConnections.Down:
			return UtilityConnections.Up;
		}
		throw new AccessViolationException();
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
		if (direction != (UtilityConnections)0 && (grid[dest_cell].connections & this.InverseDirection(direction)) == (UtilityConnections)0)
		{
			return;
		}
		GameObject gameObject = Grid.Objects[dest_cell, this.tileLayer];
		if (gameObject != null)
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
		this.dirty = true;
		if (item != null)
		{
			if (is_endpoint)
			{
				this.endpoints[cell] = item;
			}
			else
			{
				this.items[cell] = item;
			}
		}
	}

	private unsafe void Reconnect(int cell)
	{
		int* ptr = stackalloc int[checked(4 * 4)];
		*ptr = Grid.CellAbove(cell);
		ptr[1] = Grid.CellBelow(cell);
		ptr[2] = Grid.CellLeft(cell);
		ptr[3] = Grid.CellRight(cell);
		int* ptr2 = stackalloc int[checked(4 * 4)];
		*ptr2 = 4;
		ptr2[1] = 8;
		ptr2[2] = 1;
		ptr2[3] = 2;
		int* ptr3 = stackalloc int[checked(4 * 4)];
		*ptr3 = 8;
		ptr3[1] = 4;
		ptr3[2] = 2;
		ptr3[3] = 1;
		UtilityConnections connections = this.physicalGrid[cell].connections;
		UtilityConnections connections2 = this.visualGrid[cell].connections;
		for (int i = 0; i < 4; i++)
		{
			int num = ptr[i];
			UtilityConnections utilityConnections = (UtilityConnections)ptr2[i];
			UtilityConnections utilityConnections2 = (UtilityConnections)ptr3[i];
			if (Grid.IsValidCell(num))
			{
				if ((connections & utilityConnections) != (UtilityConnections)0)
				{
					if (this.physicalNodes.Contains(num))
					{
						UtilityNetworkGridNode[] array = this.physicalGrid;
						int num2 = num;
						array[num2].connections = array[num2].connections | utilityConnections2;
					}
					if (this.visualNodes.Contains(num))
					{
						UtilityNetworkGridNode[] array2 = this.visualGrid;
						int num3 = num;
						array2[num3].connections = array2[num3].connections | utilityConnections2;
					}
				}
				else if ((connections2 & utilityConnections) != (UtilityConnections)0 && (this.physicalNodes.Contains(num) || this.visualNodes.Contains(num)))
				{
					UtilityNetworkGridNode[] array3 = this.visualGrid;
					int num4 = num;
					array3[num4].connections = array3[num4].connections | utilityConnections2;
				}
			}
		}
	}

	public void RemoveFromNetworks(int cell, object item, bool is_endpoint)
	{
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
		int* ptr = stackalloc int[checked(4 * 4)];
		*ptr = Grid.CellAbove(cell);
		ptr[1] = Grid.CellBelow(cell);
		ptr[2] = Grid.CellLeft(cell);
		ptr[3] = Grid.CellRight(cell);
		int* ptr2 = stackalloc int[checked(4 * 4)];
		*ptr2 = -9;
		ptr2[1] = -5;
		ptr2[2] = -3;
		ptr2[3] = -2;
		for (int i = 0; i < 4; i++)
		{
			int num = ptr[i];
			int num2 = ptr2[i];
			int num3 = (int)(this.physicalGrid[num].connections & (UtilityConnections)num2);
			this.physicalGrid[num].connections = (UtilityConnections)num3;
		}
	}

	private void RebuildNetworks(int layer, bool is_physical)
	{
		UtilityNetworkGridNode[] grid = this.GetGrid(is_physical);
		HashSet<int> nodes = this.GetNodes(is_physical);
		this.visitedCells.Clear();
		this.queued.Clear();
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
					int num3 = Grid.CellLeft(num2);
					int num4 = Grid.CellRight(num2);
					int num5 = Grid.CellAbove(num2);
					int num6 = Grid.CellBelow(num2);
					utilityNetworkGridNode = grid[num2];
					object obj = null;
					object obj2 = null;
					if (is_physical)
					{
						if (this.items.TryGetValue(num2, out obj))
						{
							if (obj is IDisconnectable)
							{
								IDisconnectable disconnectable = obj as IDisconnectable;
								if (disconnectable.IsDisconnected())
								{
									continue;
								}
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
					if ((utilityNetworkGridNode.connections & UtilityConnections.Left) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num3, UtilityConnections.Left);
					}
					if ((utilityNetworkGridNode.connections & UtilityConnections.Right) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num4, UtilityConnections.Right);
					}
					if ((utilityNetworkGridNode.connections & UtilityConnections.Up) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num5, UtilityConnections.Up);
					}
					if ((utilityNetworkGridNode.connections & UtilityConnections.Down) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num6, UtilityConnections.Down);
					}
					int num7;
					if (this.links.TryGetValue(num2, out num7))
					{
						this.QueueCellForVisit(grid, num7, (UtilityConnections)0);
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
		UtilityNetworkGridNode[] grid = this.GetGrid(true);
		UtilityNetworkGridNode utilityNetworkGridNode = grid[cell];
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
		int num = Grid.CellAbove(cell);
		int num2 = Grid.CellBelow(cell);
		int num3 = Grid.CellLeft(cell);
		int num4 = Grid.CellRight(cell);
		if (nodes.Contains(num))
		{
			utilityConnections |= UtilityConnections.Up;
		}
		if (nodes.Contains(num2))
		{
			utilityConnections |= UtilityConnections.Down;
		}
		if (nodes.Contains(num3))
		{
			utilityConnections |= UtilityConnections.Left;
		}
		if (nodes.Contains(num4))
		{
			utilityConnections |= UtilityConnections.Right;
		}
		return utilityConnections;
	}

	public void SetConnections(UtilityConnections connections, int cell, bool is_physical_building)
	{
		HashSet<int> nodes = this.GetNodes(is_physical_building);
		nodes.Add(cell);
		this.visualGrid[cell].connections = connections;
		if (is_physical_building)
		{
			this.dirty = true;
			UtilityConnections utilityConnections = ((!is_physical_building) ? connections : (connections & this.GetNeighboursAsConnections(cell, nodes)));
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
		utilityConnections |= array[cell].connections;
		array = this.GetGrid(true);
		return utilityConnections | array[cell].connections;
	}

	public void AddConnection(UtilityConnections new_connection, int cell, bool is_physical_building)
	{
		if (is_physical_building)
		{
			this.dirty = true;
		}
		UtilityNetworkGridNode[] grid = this.GetGrid(is_physical_building);
		UtilityConnections connections = grid[cell].connections;
		grid[cell].connections = connections | new_connection;
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
		string text = string.Empty;
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
		if (text == string.Empty)
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

	private UtilityNetworkGridNode[] visualGrid;

	private UtilityNetworkGridNode[] stashedVisualGrid;

	private UtilityNetworkGridNode[] physicalGrid;

	private HashSet<int> physicalNodes;

	private HashSet<int> visualNodes;

	private bool dirty;

	private int tileLayer = -1;
}
