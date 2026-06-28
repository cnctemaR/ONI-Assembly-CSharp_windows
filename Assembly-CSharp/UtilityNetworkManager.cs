using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilityNetworkManager<NetworkType, ItemType> : IUtilityNetworkMgr where NetworkType : UtilityNetwork, new() where ItemType : MonoBehaviour
{
	public UtilityNetworkManager(int game_width, int game_height, int tile_layer, int item_layer, Vent.Transfer transfer_type, bool allow_joins)
	{
		this.tileLayer = tile_layer;
		this.transferType = transfer_type;
		this.networks = new List<NetworkType>();
		this.Initialize(game_width, game_height);
	}

	public ConduitFlow ConduitFlowManager { get; set; }

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
		this.physicalGrid = new UtilityNetworkManager<NetworkType, ItemType>.Node[game_width * game_height];
		this.visualGrid = new UtilityNetworkManager<NetworkType, ItemType>.Node[game_width * game_height];
		this.stashedVisualGrid = new UtilityNetworkManager<NetworkType, ItemType>.Node[game_width * game_height];
		this.physicalNodes = new HashSet<int>();
		this.visualNodes = new HashSet<int>();
		this.visitedCells = new HashSet<int>();
		for (int i = 0; i < this.visualGrid.Length; i++)
		{
			this.visualGrid[i] = new UtilityNetworkManager<NetworkType, ItemType>.Node
			{
				networkIdx = -1,
				connections = (UtilityConnections)0
			};
			this.physicalGrid[i] = new UtilityNetworkManager<NetworkType, ItemType>.Node
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
				NetworkType networkType = this.networks[i];
				networkType.Reset();
			}
			this.networks.Clear();
			this.RebuildNetworks(this.tileLayer, false);
			this.RebuildNetworks(this.tileLayer, true);
		}
	}

	private UtilityNetworkManager<NetworkType, ItemType>.Node[] GetGrid(bool is_physical_building)
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
		UtilityNetworkManager<NetworkType, ItemType>.Node[] grid = this.GetGrid(is_physical_building);
		HashSet<int> nodes = this.GetNodes(is_physical_building);
		this.dirty = true;
		UtilityConnections connections = grid[cell].connections;
		grid[cell].connections = (UtilityConnections)0;
		grid[cell].networkIdx = -1;
		int num = Grid.CellAbove(cell);
		int num2 = Grid.CellBelow(cell);
		int num3 = Grid.CellLeft(cell);
		int num4 = Grid.CellRight(cell);
		if (Grid.IsValidCell(num) && (connections & UtilityConnections.Up) != (UtilityConnections)0)
		{
			UtilityNetworkManager<NetworkType, ItemType>.Node[] array = grid;
			int num5 = num;
			array[num5].connections = array[num5].connections & ~UtilityConnections.Down;
		}
		if (Grid.IsValidCell(num2) && (connections & UtilityConnections.Down) != (UtilityConnections)0)
		{
			UtilityNetworkManager<NetworkType, ItemType>.Node[] array2 = grid;
			int num6 = num2;
			array2[num6].connections = array2[num6].connections & ~UtilityConnections.Up;
		}
		if (Grid.IsValidCell(num3) && (connections & UtilityConnections.Left) != (UtilityConnections)0)
		{
			UtilityNetworkManager<NetworkType, ItemType>.Node[] array3 = grid;
			int num7 = num3;
			array3[num7].connections = array3[num7].connections & ~UtilityConnections.Right;
		}
		if (Grid.IsValidCell(num4) && (connections & UtilityConnections.Right) != (UtilityConnections)0)
		{
			UtilityNetworkManager<NetworkType, ItemType>.Node[] array4 = grid;
			int num8 = num4;
			array4[num8].connections = array4[num8].connections & ~UtilityConnections.Left;
		}
		nodes.Remove(cell);
		if (is_physical_building)
		{
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

	private void QueueCellForVisit(UtilityNetworkManager<NetworkType, ItemType>.Node[] grid, int dest_cell, UtilityConnections direction)
	{
		if (!Grid.IsValidCell(dest_cell))
		{
			return;
		}
		if (direction != (UtilityConnections)0 && (grid[dest_cell].connections & this.InverseDirection(direction)) == (UtilityConnections)0)
		{
			return;
		}
		if (this.visitedCells.Contains(dest_cell))
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

	public void AddToNetworks(int cell, FlowUtilityNetwork.IItem item)
	{
		this.dirty = true;
		if (item != null)
		{
			if (item.EndpointType == Vent.Endpoint.Conduit)
			{
				this.conduits[cell] = item;
			}
			else
			{
				this.endpoints[cell] = item;
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
						UtilityNetworkManager<NetworkType, ItemType>.Node[] array = this.physicalGrid;
						int num2 = num;
						array[num2].connections = array[num2].connections | utilityConnections2;
					}
					if (this.visualNodes.Contains(num))
					{
						UtilityNetworkManager<NetworkType, ItemType>.Node[] array2 = this.visualGrid;
						int num3 = num;
						array2[num3].connections = array2[num3].connections | utilityConnections2;
					}
				}
				else if ((connections2 & utilityConnections) != (UtilityConnections)0 && (this.physicalNodes.Contains(num) || this.visualNodes.Contains(num)))
				{
					UtilityNetworkManager<NetworkType, ItemType>.Node[] array3 = this.visualGrid;
					int num4 = num;
					array3[num4].connections = array3[num4].connections | utilityConnections2;
				}
			}
		}
	}

	public void RemoveFromNetworks(int cell, FlowUtilityNetwork.IItem vent)
	{
		this.dirty = true;
		if (vent != null)
		{
			if (vent.EndpointType == Vent.Endpoint.Conduit)
			{
				if (this.conduits.ContainsKey(cell))
				{
					this.conduits.Remove(cell);
					this.physicalGrid[cell].connections = (UtilityConnections)0;
					this.physicalGrid[cell].networkIdx = -1;
					this.ConduitFlowManager.DeactivateCell(cell);
					this.Disconnect(cell);
				}
			}
			else
			{
				this.endpoints.Remove(cell);
			}
		}
		else if (vent == null)
		{
			this.physicalGrid[cell].connections = (UtilityConnections)0;
			this.physicalGrid[cell].networkIdx = -1;
			this.Disconnect(cell);
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
		UtilityNetworkManager<NetworkType, ItemType>.Node[] grid = this.GetGrid(is_physical);
		HashSet<int> nodes = this.GetNodes(is_physical);
		this.visitedCells.Clear();
		this.queued.Clear();
		foreach (int num in nodes)
		{
			UtilityNetworkManager<NetworkType, ItemType>.Node node = grid[num];
			if (!this.visitedCells.Contains(num))
			{
				this.queued.Enqueue(num);
				this.visitedCells.Add(num);
				NetworkType networkType = new NetworkType();
				networkType.id = this.networks.Count;
				networkType.transferType = this.transferType;
				this.networks.Add(networkType);
				while (this.queued.Count > 0)
				{
					int num2 = this.queued.Dequeue();
					int num3 = Grid.CellLeft(num2);
					int num4 = Grid.CellRight(num2);
					int num5 = Grid.CellAbove(num2);
					int num6 = Grid.CellBelow(num2);
					node = grid[num2];
					if (is_physical)
					{
						if (this.conduits.ContainsKey(num2))
						{
							FlowUtilityNetwork.IItem item = this.conduits[num2];
							if (item != null)
							{
								networkType.AddItem(num2, item);
							}
						}
						if (this.endpoints.ContainsKey(num2))
						{
							FlowUtilityNetwork.IItem item2 = this.endpoints[num2];
							if (item2 != null)
							{
								networkType.AddItem(num2, item2);
							}
						}
					}
					grid[num2].networkIdx = networkType.id;
					if ((node.connections & UtilityConnections.Left) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num3, UtilityConnections.Left);
					}
					if ((node.connections & UtilityConnections.Right) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num4, UtilityConnections.Right);
					}
					if ((node.connections & UtilityConnections.Up) != (UtilityConnections)0)
					{
						this.QueueCellForVisit(grid, num5, UtilityConnections.Up);
					}
					if ((node.connections & UtilityConnections.Down) != (UtilityConnections)0)
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
		if (is_physical && this.ConduitFlowManager != null)
		{
			this.ConduitFlowManager.RebuildConnections(nodes);
			foreach (NetworkType networkType2 in this.networks)
			{
				this.ConduitFlowManager.ScanNetworkSources(networkType2 as FlowUtilityNetwork);
			}
			this.ConduitFlowManager.RefreshPaths();
		}
	}

	public UtilityNetwork GetNetworkByID(int id)
	{
		return this.networks[id];
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

	public UtilityNetwork GetNetworkForOrientation(int cell, Orientation orientation)
	{
		cell = Grid.GetCellInDirection(cell, orientation);
		if (!Grid.IsValidCell(cell))
		{
			return null;
		}
		UtilityNetworkManager<NetworkType, ItemType>.Node[] grid = this.GetGrid(true);
		UtilityNetworkManager<NetworkType, ItemType>.Node node = grid[cell];
		UtilityNetwork utilityNetwork = null;
		if (node.networkIdx != -1 && node.networkIdx < this.networks.Count)
		{
			utilityNetwork = this.networks[node.networkIdx];
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
		this.dirty = true;
		HashSet<int> nodes = this.GetNodes(is_physical_building);
		nodes.Add(cell);
		this.visualGrid[cell].connections = connections;
		if (is_physical_building)
		{
			UtilityConnections utilityConnections = ((!is_physical_building) ? connections : (connections & this.GetNeighboursAsConnections(cell, nodes)));
			this.physicalGrid[cell].connections = utilityConnections;
		}
		this.Reconnect(cell);
	}

	public UtilityConnections GetConnections(int cell, bool is_physical_building)
	{
		UtilityNetworkManager<NetworkType, ItemType>.Node[] array = this.GetGrid(is_physical_building);
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
		UtilityNetworkManager<NetworkType, ItemType>.Node[] array = this.GetGrid(false);
		utilityConnections |= array[cell].connections;
		array = this.GetGrid(true);
		return utilityConnections | array[cell].connections;
	}

	public void AddConnection(UtilityConnections new_connection, int cell, bool is_physical_building)
	{
		this.dirty = true;
		UtilityNetworkManager<NetworkType, ItemType>.Node[] grid = this.GetGrid(is_physical_building);
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

	public FlowUtilityNetwork.IItem GetEndpoint(int cell)
	{
		FlowUtilityNetwork.IItem item = null;
		this.endpoints.TryGetValue(cell, out item);
		return item;
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

	private Dictionary<int, FlowUtilityNetwork.IItem> conduits = new Dictionary<int, FlowUtilityNetwork.IItem>();

	private Dictionary<int, FlowUtilityNetwork.IItem> endpoints = new Dictionary<int, FlowUtilityNetwork.IItem>();

	private Dictionary<int, int> links = new Dictionary<int, int>();

	public List<NetworkType> networks;

	private HashSet<int> visitedCells;

	private Queue<int> queued = new Queue<int>();

	private UtilityNetworkManager<NetworkType, ItemType>.Node[] visualGrid;

	private UtilityNetworkManager<NetworkType, ItemType>.Node[] stashedVisualGrid;

	private UtilityNetworkManager<NetworkType, ItemType>.Node[] physicalGrid;

	private HashSet<int> physicalNodes;

	private HashSet<int> visualNodes;

	private bool dirty;

	private int tileLayer = -1;

	private Vent.Transfer transferType;

	public struct Node : IEquatable<UtilityNetworkManager<NetworkType, ItemType>.Node>
	{
		public bool Equals(UtilityNetworkManager<NetworkType, ItemType>.Node other)
		{
			return this.connections == other.connections && this.networkIdx == other.networkIdx;
		}

		public override bool Equals(object obj)
		{
			return ((UtilityNetworkManager<NetworkType, ItemType>.Node)obj).Equals(this);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(UtilityNetworkManager<NetworkType, ItemType>.Node x, UtilityNetworkManager<NetworkType, ItemType>.Node y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(UtilityNetworkManager<NetworkType, ItemType>.Node x, UtilityNetworkManager<NetworkType, ItemType>.Node y)
		{
			return !x.Equals(y);
		}

		public UtilityConnections connections;

		public int networkIdx;
	}
}
