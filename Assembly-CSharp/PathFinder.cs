using System;
using System.Collections.Generic;

public class PathFinder
{
	public static void Initialize()
	{
		NavType[] array = new NavType[9];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (NavType)i;
		}
		PathFinder.PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, array);
		for (int j = 0; j < Grid.CellCount; j++)
		{
			if (Grid.Visible[j] > 0 || Grid.Spawnable[j] > 0)
			{
				ListPool<int, PathFinder>.PooledList pooledList = ListPool<int, PathFinder>.Allocate();
				GameUtil.FloodFillConditional(j, PathFinder.allowPathfindingFloodFillCb, pooledList, null);
				Grid.AllowPathfinding[j] = true;
				pooledList.Recycle();
			}
		}
		Grid.OnReveal = (Action<int>)Delegate.Combine(Grid.OnReveal, new Action<int>(PathFinder.OnReveal));
	}

	private static void OnReveal(int cell)
	{
	}

	public static void UpdatePath(NavGrid nav_grid, PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathFinderQuery query, ref PathFinder.Path path)
	{
		PathFinder.Run(nav_grid, abilities, potential_path, query, ref path);
	}

	public static bool ValidatePath(NavGrid nav_grid, PathFinderAbilities abilities, ref PathFinder.Path path)
	{
		if (!path.IsValid())
		{
			return false;
		}
		for (int i = 0; i < path.nodes.Count; i++)
		{
			PathFinder.Path.Node node = path.nodes[i];
			if (i < path.nodes.Count - 1)
			{
				PathFinder.Path.Node node2 = path.nodes[i + 1];
				int num = node.cell * nav_grid.maxLinksPerCell;
				bool flag = false;
				NavGrid.Link link = nav_grid.Links[num];
				while (link.link != PathFinder.InvalidHandle)
				{
					if (link.link == node2.cell && node2.navType == link.endNavType && node.navType == link.startNavType)
					{
						PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(node.cell, node.navType, PathFinder.PotentialPath.Flags.None);
						flag = abilities.TraversePath(ref potentialPath, node.cell, node.navType, 0, link.transitionId, 0);
						if (flag)
						{
							break;
						}
					}
					num++;
					link = nav_grid.Links[num];
				}
				if (!flag)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathFinderQuery query)
	{
		int invalidCell = PathFinder.InvalidCell;
		NavType navType = NavType.NumNavTypes;
		query.ClearResult();
		if (!Grid.IsValidCell(potential_path.cell))
		{
			return;
		}
		PathFinder.FindPaths(nav_grid, ref abilities, potential_path, PathFinder.PathGrid, query, ref PathFinder.QueryId, PathFinder.Temp.Potentials, ref invalidCell, ref navType);
		if (invalidCell != PathFinder.InvalidCell)
		{
			PathFinder.Cell cell = PathFinder.PathGrid.GetCell(invalidCell, navType, PathFinder.QueryId);
			query.SetResult(invalidCell, cell.cost, navType);
		}
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathFinderQuery query, ref PathFinder.Path path)
	{
		PathFinder.Run(nav_grid, abilities, potential_path, query);
		if (query.GetResultCell() != PathFinder.InvalidCell)
		{
			PathFinder.BuildResultPath(query.GetResultCell(), query.GetResultNavType(), PathFinder.PathGrid, PathFinder.QueryId, ref path);
		}
		else
		{
			path.Clear();
		}
	}

	private static void BuildResultPath(int path_cell, NavType path_nav_type, PathGrid path_grid, int query_id, ref PathFinder.Path path)
	{
		if (path_cell != PathFinder.InvalidCell)
		{
			PathFinder.Cell cell = path_grid.GetCell(path_cell, path_nav_type, query_id);
			path.Clear();
			path.cost = cell.cost;
			while (path_cell != PathFinder.InvalidCell)
			{
				path.AddNode(new PathFinder.Path.Node
				{
					cell = path_cell,
					navType = cell.navType,
					transitionId = cell.transitionId
				});
				path_cell = cell.parent;
				if (path_cell != PathFinder.InvalidCell)
				{
					cell = path_grid.GetCell(path_cell, cell.parentNavType, query_id);
				}
			}
			if (path.nodes != null)
			{
				for (int i = 0; i < path.nodes.Count / 2; i++)
				{
					PathFinder.Path.Node node = path.nodes[i];
					path.nodes[i] = path.nodes[path.nodes.Count - i - 1];
					path.nodes[path.nodes.Count - i - 1] = node;
				}
			}
		}
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathGrid path_grid, PathFinderQuery query, ref int query_id, PathFinder.PotentialList potentials, ref int result_cell, ref NavType result_nav_type)
	{
		potentials.Clear();
		query_id++;
		PathFinder.Cell cell = path_grid.GetCell(potential_path, query_id);
		PathFinder.AddPotential(potential_path, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, potentials, query_id, path_grid, ref cell);
		PathFinder.FindPaths(nav_grid, ref abilities, potentials, query_id, path_grid, query, ref result_cell, ref result_nav_type);
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialList potentials, int query_id, PathGrid path_grid, PathFinderQuery query, ref int result_cell, ref NavType result_nav_type)
	{
		int maxValue = int.MaxValue;
		while (potentials.Count > 0)
		{
			KeyValuePair<int, PathFinder.PotentialPath> keyValuePair = potentials.Next();
			if (PathFinder.FindPaths(nav_grid, ref abilities, keyValuePair.Value, keyValuePair.Key, potentials, query_id, path_grid, query, ref result_cell, ref result_nav_type, ref maxValue))
			{
				break;
			}
		}
	}

	private static bool FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialPath potential, int potential_cost, PathFinder.PotentialList potentials, int query_id, PathGrid path_grid, PathFinderQuery query, ref int result_cell, ref NavType result_nav_type, ref int result_cost)
	{
		PathFinder.Cell cell = path_grid.GetCell(potential, query_id);
		if (cell.cost != potential_cost)
		{
			return false;
		}
		int cost = cell.cost;
		NavType navType = cell.navType;
		bool flag = navType != NavType.Tube && query.IsMatch(potential.cell, cell.parent, cost) && cost < result_cost;
		if (flag)
		{
			result_cell = potential.cell;
			result_cost = cost;
			result_nav_type = navType;
		}
		if (!flag)
		{
			PathFinder.AddPotentials(potential, cell.cost, (int)cell.underwaterCost, ref abilities, query, nav_grid.maxLinksPerCell, nav_grid.Links, potentials, query_id, path_grid, cell.parent, cell.parentNavType);
		}
		return flag;
	}

	public static void AddPotential(PathFinder.PotentialPath potential_path, int parent_cell, NavType parent_nav_type, int cost, int underwater_cost, int transition_id, PathFinder.PotentialList potentials, int query_id, PathGrid path_grid, ref PathFinder.Cell cell_data)
	{
		cell_data.queryId = query_id;
		cell_data.cost = cost;
		cell_data.underwaterCost = (byte)Math.Min(underwater_cost, 255);
		cell_data.parent = parent_cell;
		cell_data.navType = potential_path.navType;
		cell_data.parentNavType = parent_nav_type;
		cell_data.transitionId = transition_id;
		potentials.Add(cost, potential_path);
		path_grid.SetCell(potential_path, ref cell_data);
	}

	public static bool IsSubmerged(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = Grid.CellAbove(cell);
		return (Grid.IsValidCell(num) && Grid.Element[num].IsLiquid) || (Grid.Element[cell].IsLiquid && Grid.IsValidCell(num) && Grid.Element[num].IsSolid);
	}

	public static void AddPotentials(PathFinder.PotentialPath potential, int cost, int underwater_cost, ref PathFinderAbilities abilities, PathFinderQuery query, int max_links_per_cell, NavGrid.Link[] links, PathFinder.PotentialList potentials, int query_id, PathGrid path_grid, int parent_cell, NavType parent_nav_type)
	{
		int num = potential.cell * max_links_per_cell;
		NavGrid.Link link = links[num];
		for (int num2 = link.link; num2 != PathFinder.InvalidHandle; num2 = link.link)
		{
			NavType startNavType = link.startNavType;
			if (startNavType == potential.navType && (parent_cell != num2 || parent_nav_type != link.startNavType) && path_grid.IsCellInRange(num2))
			{
				PathFinder.Cell cell = path_grid.GetCell(num2, link.endNavType, query_id);
				int num3 = cost + link.cost;
				bool flag = query_id != cell.queryId;
				bool flag2 = num3 < cell.cost;
				if (flag || flag2)
				{
					PathFinder.PotentialPath potentialPath = potential;
					potentialPath.cell = num2;
					potentialPath.navType = link.endNavType;
					int num4;
					if (PathFinder.IsSubmerged(num2))
					{
						num4 = underwater_cost + 1;
					}
					else
					{
						num4 = 0;
					}
					if (abilities.TraversePath(ref potentialPath, potential.cell, potential.navType, num3, link.transitionId, num4))
					{
						PathFinder.AddPotential(potentialPath, potential.cell, potential.navType, num3, num4, link.transitionId, potentials, query_id, path_grid, ref cell);
					}
				}
			}
			num++;
			link = links[num];
		}
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public static int QueryId;

	public static PathGrid PathGrid;

	private static readonly Func<int, bool> allowPathfindingFloodFillCb = delegate(int cell)
	{
		if (Grid.Solid[cell])
		{
			return false;
		}
		if (Grid.AllowPathfinding[cell])
		{
			return false;
		}
		Grid.AllowPathfinding[cell] = true;
		return true;
	};

	public struct Cell
	{
		public int queryId;

		public int cost;

		public int parent;

		public byte underwaterCost;

		public NavType navType;

		public NavType parentNavType;

		public int transitionId;
	}

	public struct PotentialPath
	{
		public PotentialPath(int cell, NavType nav_type, PathFinder.PotentialPath.Flags flags)
		{
			this.cell = cell;
			this.navType = nav_type;
			this.flags = flags;
		}

		public void SetFlags(PathFinder.PotentialPath.Flags new_flags)
		{
			this.flags |= new_flags;
		}

		public void ClearFlags(PathFinder.PotentialPath.Flags new_flags)
		{
			this.flags &= ~new_flags;
		}

		public bool HasFlag(PathFinder.PotentialPath.Flags flag)
		{
			return (byte)(this.flags & flag) != 0;
		}

		public int cell;

		public NavType navType;

		private PathFinder.PotentialPath.Flags flags;

		[Flags]
		public enum Flags : byte
		{
			None = 0,
			HasSuit = 1,
			UnlimitedSubmergedTravel = 2,
			PerformSuitChecks = 4
		}
	}

	public struct Path
	{
		public void AddNode(PathFinder.Path.Node node)
		{
			if (this.nodes == null)
			{
				this.nodes = new List<PathFinder.Path.Node>();
			}
			this.nodes.Add(node);
		}

		public bool IsValid()
		{
			return this.nodes != null && this.nodes.Count > 1;
		}

		public bool HasArrived()
		{
			return this.nodes != null && this.nodes.Count > 0;
		}

		public void Clear()
		{
			this.cost = 0;
			if (this.nodes != null)
			{
				this.nodes.Clear();
			}
		}

		public int cost;

		public List<PathFinder.Path.Node> nodes;

		public struct Node
		{
			public int cell;

			public NavType navType;

			public int transitionId;
		}
	}

	public class PotentialList
	{
		public KeyValuePair<int, PathFinder.PotentialPath> Next()
		{
			return this.queue.Dequeue();
		}

		public int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public void Add(int cost, PathFinder.PotentialPath path)
		{
			this.queue.Enqueue(cost, path);
		}

		public void Clear()
		{
			this.queue.Clear();
		}

		private PathFinder.PotentialList.HOTQueue<PathFinder.PotentialPath> queue = new PathFinder.PotentialList.HOTQueue<PathFinder.PotentialPath>();

		public class PriorityQueue<TValue>
		{
			public PriorityQueue()
			{
				this._baseHeap = new List<KeyValuePair<int, TValue>>();
			}

			public void Enqueue(int priority, TValue value)
			{
				this.Insert(priority, value);
			}

			public KeyValuePair<int, TValue> Dequeue()
			{
				KeyValuePair<int, TValue> keyValuePair = this._baseHeap[0];
				this.DeleteRoot();
				return keyValuePair;
			}

			public KeyValuePair<int, TValue> Peek()
			{
				if (this.Count > 0)
				{
					return this._baseHeap[0];
				}
				throw new InvalidOperationException("Priority queue is empty");
			}

			private void ExchangeElements(int pos1, int pos2)
			{
				KeyValuePair<int, TValue> keyValuePair = this._baseHeap[pos1];
				this._baseHeap[pos1] = this._baseHeap[pos2];
				this._baseHeap[pos2] = keyValuePair;
			}

			private void Insert(int priority, TValue value)
			{
				KeyValuePair<int, TValue> keyValuePair = new KeyValuePair<int, TValue>(priority, value);
				this._baseHeap.Add(keyValuePair);
				this.HeapifyFromEndToBeginning(this._baseHeap.Count - 1);
			}

			private int HeapifyFromEndToBeginning(int pos)
			{
				if (pos >= this._baseHeap.Count)
				{
					return -1;
				}
				while (pos > 0)
				{
					int num = (pos - 1) / 2;
					if (this._baseHeap[num].Key - this._baseHeap[pos].Key <= 0)
					{
						break;
					}
					this.ExchangeElements(num, pos);
					pos = num;
				}
				return pos;
			}

			private void DeleteRoot()
			{
				if (this._baseHeap.Count <= 1)
				{
					this._baseHeap.Clear();
					return;
				}
				this._baseHeap[0] = this._baseHeap[this._baseHeap.Count - 1];
				this._baseHeap.RemoveAt(this._baseHeap.Count - 1);
				this.HeapifyFromBeginningToEnd(0);
			}

			private void HeapifyFromBeginningToEnd(int pos)
			{
				int count = this._baseHeap.Count;
				if (pos >= count)
				{
					return;
				}
				for (;;)
				{
					int num = pos;
					int num2 = 2 * pos + 1;
					int num3 = 2 * pos + 2;
					if (num2 < count && this._baseHeap[num].Key - this._baseHeap[num2].Key > 0)
					{
						num = num2;
					}
					if (num3 < count && this._baseHeap[num].Key - this._baseHeap[num3].Key > 0)
					{
						num = num3;
					}
					if (num == pos)
					{
						break;
					}
					this.ExchangeElements(num, pos);
					pos = num;
				}
			}

			public void Clear()
			{
				this._baseHeap.Clear();
			}

			public int Count
			{
				get
				{
					return this._baseHeap.Count;
				}
			}

			private List<KeyValuePair<int, TValue>> _baseHeap;
		}

		private class HOTQueue<TValue>
		{
			public KeyValuePair<int, TValue> Dequeue()
			{
				if (this.hotQueue.Count == 0)
				{
					PathFinder.PotentialList.PriorityQueue<TValue> priorityQueue = this.hotQueue;
					this.hotQueue = this.coldQueue;
					this.coldQueue = priorityQueue;
					this.hotThreshold = this.coldThreshold;
				}
				this.count--;
				return this.hotQueue.Dequeue();
			}

			public void Enqueue(int priority, TValue value)
			{
				if (priority <= this.hotThreshold)
				{
					this.hotQueue.Enqueue(priority, value);
				}
				else
				{
					this.coldQueue.Enqueue(priority, value);
					this.coldThreshold = Math.Max(this.coldThreshold, priority);
				}
				this.count++;
			}

			public KeyValuePair<int, TValue> Peek()
			{
				if (this.hotQueue.Count == 0)
				{
					PathFinder.PotentialList.PriorityQueue<TValue> priorityQueue = this.hotQueue;
					this.hotQueue = this.coldQueue;
					this.coldQueue = priorityQueue;
					this.hotThreshold = this.coldThreshold;
				}
				return this.hotQueue.Peek();
			}

			public void Clear()
			{
				this.count = 0;
				this.hotThreshold = int.MinValue;
				this.hotQueue.Clear();
				this.coldThreshold = int.MinValue;
				this.coldQueue.Clear();
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}

			private PathFinder.PotentialList.PriorityQueue<TValue> hotQueue = new PathFinder.PotentialList.PriorityQueue<TValue>();

			private PathFinder.PotentialList.PriorityQueue<TValue> coldQueue = new PathFinder.PotentialList.PriorityQueue<TValue>();

			private int hotThreshold = int.MinValue;

			private int coldThreshold = int.MinValue;

			private int count;
		}
	}

	private class Temp
	{
		public static PathFinder.PotentialList Potentials = new PathFinder.PotentialList();
	}
}
