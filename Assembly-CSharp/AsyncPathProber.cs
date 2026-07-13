using System;
using System.Collections.Generic;
using System.Threading;

public static class AsyncPathProber
{
	public static AsyncPathProber.Manager Instance { get; private set; }

	public static void CreateInstance(int count)
	{
		DebugUtil.Assert(AsyncPathProber.Instance == null);
		AsyncPathProber.Instance = new AsyncPathProber.Manager();
		AsyncPathProber.Instance.Start(count);
	}

	public static void DestroyInstance()
	{
		AsyncPathProber.Instance.Shutdown();
		AsyncPathProber.Instance = null;
	}

	public class PathProberResources
	{
		public PathProberResources()
		{
			this.path_grids = new Dictionary<ulong, PathGrid>();
			this.potentials = new PathFinder.PotentialList();
			this.scratch = new PathFinder.PotentialScratchPad(Pathfinding.Instance.MaxLinksPerCell());
			this.found_cells = new List<int>();
		}

		public Dictionary<ulong, PathGrid> path_grids;

		public PathFinder.PotentialList potentials;

		public PathFinder.PotentialScratchPad scratch;

		public List<int> found_cells;
	}

	private static class AsyncPathProbeWorker
	{
		public static void main(object _)
		{
			AsyncPathProber.PathProberResources pathProberResources = new AsyncPathProber.PathProberResources();
			while (!AsyncPathProber.Instance.Halting())
			{
				Navigator.AsyncPathGridUpdaterEntry asyncPathGridUpdaterEntry;
				if (AsyncPathProber.Instance.NextTask(out asyncPathGridUpdaterEntry))
				{
					ulong allocatedClassification = asyncPathGridUpdaterEntry.navigator.PathGrid.AllocatedClassification;
					PathGrid pathGrid = null;
					if (!pathProberResources.path_grids.TryGetValue(allocatedClassification, out pathGrid))
					{
						pathGrid = new PathGrid(asyncPathGridUpdaterEntry.navigator.PathGrid);
						pathProberResources.path_grids[allocatedClassification] = pathGrid;
					}
					else
					{
						pathGrid.CloneNavTypes(asyncPathGridUpdaterEntry.navigator.PathGrid);
					}
					ushort serialNo = AsyncPathProber.Instance.SerialNo;
					if (pathGrid.SerialNo > AsyncPathProber.Instance.SerialNo)
					{
						pathGrid.ResetProberCells();
					}
					PathProber.Run(asyncPathGridUpdaterEntry.originCell, asyncPathGridUpdaterEntry.abilities, asyncPathGridUpdaterEntry.navigator.NavGrid, asyncPathGridUpdaterEntry.startingNavType, pathGrid, AsyncPathProber.Instance.SerialNo, pathProberResources.scratch, pathProberResources.potentials, asyncPathGridUpdaterEntry.startingFlags, pathProberResources.found_cells);
					if (asyncPathGridUpdaterEntry.navigator.reportOccupation)
					{
						pathProberResources.found_cells.Sort();
					}
					Navigator.AsyncPathGridUpdaterEntry asyncPathGridUpdaterEntry2 = asyncPathGridUpdaterEntry;
					lock (asyncPathGridUpdaterEntry2)
					{
						if (!asyncPathGridUpdaterEntry.toRemove)
						{
							pathProberResources.path_grids[allocatedClassification] = asyncPathGridUpdaterEntry.navigator.PathGrid;
							asyncPathGridUpdaterEntry.navigator.PathGrid = pathGrid;
							asyncPathGridUpdaterEntry.framesSinceLastUpdate = 0;
							if (asyncPathGridUpdaterEntry.navigator.reportOccupation)
							{
								List<int> occupiedCells = asyncPathGridUpdaterEntry.navigator.occupiedCells;
								int i = 0;
								int j = 0;
								while (i < occupiedCells.Count)
								{
									if (j >= pathProberResources.found_cells.Count)
									{
										break;
									}
									if (occupiedCells[i] < pathProberResources.found_cells[j])
									{
										MinionGroupProber.Get().Vacate(occupiedCells[i]);
										i++;
									}
									else if (pathProberResources.found_cells[j] < occupiedCells[i])
									{
										MinionGroupProber.Get().Occupy(pathProberResources.found_cells[j]);
										j++;
									}
									else
									{
										i++;
										j++;
									}
								}
								while (j < pathProberResources.found_cells.Count)
								{
									MinionGroupProber.Get().Occupy(pathProberResources.found_cells[j]);
									j++;
								}
								while (i < occupiedCells.Count)
								{
									MinionGroupProber.Get().Vacate(occupiedCells[i]);
									i++;
								}
								asyncPathGridUpdaterEntry.navigator.occupiedCells = pathProberResources.found_cells;
								pathProberResources.found_cells = occupiedCells;
							}
						}
					}
					pathProberResources.found_cells.Clear();
					AsyncPathProber.Instance.TaskComplete(asyncPathGridUpdaterEntry);
				}
				else
				{
					Thread.Sleep(1);
				}
			}
		}
	}

	public class Manager
	{
		public ushort SerialNo
		{
			get
			{
				return this.pathgridSerialNo;
			}
		}

		public void Register(Navigator.AsyncPathGridUpdaterEntry nav)
		{
			DebugUtil.Assert(Game.IsOnMainThread());
			this.pendingAdds.Add(nav);
		}

		public void Unregister(Navigator.AsyncPathGridUpdaterEntry nav)
		{
			lock (nav)
			{
				nav.toRemove = true;
			}
		}

		public void Start(int agentCount)
		{
			this.agents = new Thread[agentCount];
			this.halting = false;
			for (int i = 0; i < agentCount; i++)
			{
				this.agents[i] = new Thread(new ParameterizedThreadStart(AsyncPathProber.AsyncPathProbeWorker.main));
				this.agents[i].Start();
			}
		}

		public void Shutdown()
		{
			lock (this)
			{
				this.halting = true;
			}
			Thread[] array = this.agents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Join();
			}
		}

		public bool NextTask(out Navigator.AsyncPathGridUpdaterEntry entry)
		{
			lock (this)
			{
				if (!this.halting && this.nextWorkItem < this.workQueue.Count)
				{
					entry = this.workQueue[this.nextWorkItem];
					this.nextWorkItem++;
					if (!entry.toRemove)
					{
						this.inFlight.Add(entry);
					}
					return !entry.toRemove;
				}
			}
			entry = null;
			return false;
		}

		public void TaskComplete(Navigator.AsyncPathGridUpdaterEntry entry)
		{
			lock (this)
			{
				this.inFlight.Remove(entry);
			}
		}

		public bool Halting()
		{
			return this.halting;
		}

		public void TickFrame()
		{
			lock (this)
			{
				this.pathgridSerialNo += 1;
				if (this.pathgridSerialNo == 0)
				{
					this.pathgridSerialNo += 1;
				}
				for (int i = this.navigators.Count - 1; i >= 0; i--)
				{
					if (this.navigators[i].toRemove)
					{
						this.navigators[i] = this.navigators[this.navigators.Count - 1];
						this.navigators.RemoveAt(this.navigators.Count - 1);
					}
					else
					{
						this.navigators[i].abilities.Refresh();
						this.navigators[i].originCell = this.navigators[i].navigator.cachedCell;
						this.navigators[i].startingNavType = this.navigators[i].navigator.CurrentNavType;
						this.navigators[i].startingFlags = this.navigators[i].navigator.flags;
						this.navigators[i].framesSinceLastUpdate++;
					}
				}
				foreach (Navigator.AsyncPathGridUpdaterEntry asyncPathGridUpdaterEntry in this.pendingAdds)
				{
					if (!asyncPathGridUpdaterEntry.toRemove)
					{
						asyncPathGridUpdaterEntry.originCell = Grid.PosToCell(asyncPathGridUpdaterEntry.navigator);
						asyncPathGridUpdaterEntry.startingNavType = asyncPathGridUpdaterEntry.navigator.CurrentNavType;
						asyncPathGridUpdaterEntry.abilities = asyncPathGridUpdaterEntry.navigator.GetCurrentAbilities();
						asyncPathGridUpdaterEntry.framesSinceLastUpdate = 10000;
						this.navigators.Add(asyncPathGridUpdaterEntry);
					}
				}
				this.pendingAdds.Clear();
				this.workQueue.Clear();
				foreach (Navigator.AsyncPathGridUpdaterEntry asyncPathGridUpdaterEntry2 in this.navigators)
				{
					if (!this.inFlight.Contains(asyncPathGridUpdaterEntry2))
					{
						this.workQueue.Add(asyncPathGridUpdaterEntry2);
					}
				}
				this.workQueue.Sort(this.workQueuePrioritizer);
				this.nextWorkItem = 0;
			}
		}

		private List<Navigator.AsyncPathGridUpdaterEntry> navigators = new List<Navigator.AsyncPathGridUpdaterEntry>();

		private List<Navigator.AsyncPathGridUpdaterEntry> pendingAdds = new List<Navigator.AsyncPathGridUpdaterEntry>();

		private List<Navigator.AsyncPathGridUpdaterEntry> workQueue = new List<Navigator.AsyncPathGridUpdaterEntry>();

		private HashSet<Navigator.AsyncPathGridUpdaterEntry> inFlight = new HashSet<Navigator.AsyncPathGridUpdaterEntry>();

		private Comparison<Navigator.AsyncPathGridUpdaterEntry> workQueuePrioritizer = (Navigator.AsyncPathGridUpdaterEntry a, Navigator.AsyncPathGridUpdaterEntry b) => b.framesSinceLastUpdate - a.framesSinceLastUpdate;

		private Thread[] agents;

		private bool halting;

		private int nextWorkItem;

		private ushort pathgridSerialNo;
	}
}
