using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using UnityEngine.Pool;

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

	public const int kMaxProbersPerFrame = 4;

	public struct WorkResult
	{
		public Navigator navigator;

		public PathGrid pathGrid;

		public List<int> reachableCells;

		public List<int> newlyReachableCells;

		public List<int> noLongerReachableCells;
	}

	public struct WorkOrder
	{
		public void Cleanup()
		{
			this.abilities.RecycleClone();
		}

		public void Execute(PathFinder.PotentialList potentials, PathFinder.PotentialScratchPad scratch, ref AsyncPathProber.WorkResult result)
		{
			if (result.pathGrid.SerialNo >= this.serialNo)
			{
				result.pathGrid.ResetProberCells();
			}
			PathProber.Run(this.originCell, this.abilities, this.navGrid, this.startingNavType, result.pathGrid, this.serialNo, scratch, potentials, this.startingFlags, result.reachableCells);
			if (this.computeReachables)
			{
				result.reachableCells.Sort();
				int i = 0;
				int j = 0;
				while (i < this.navigator.occupiedCells.Count)
				{
					if (j >= result.reachableCells.Count)
					{
						break;
					}
					if (this.navigator.occupiedCells[i] < result.reachableCells[j])
					{
						result.noLongerReachableCells.Add(this.navigator.occupiedCells[i]);
						i++;
					}
					else if (result.reachableCells[j] < this.navigator.occupiedCells[i])
					{
						result.newlyReachableCells.Add(result.reachableCells[j]);
						j++;
					}
					else
					{
						i++;
						j++;
					}
				}
				while (i < this.navigator.occupiedCells.Count)
				{
					result.noLongerReachableCells.Add(this.navigator.occupiedCells[i]);
					i++;
				}
				while (j < result.reachableCells.Count)
				{
					result.newlyReachableCells.Add(result.reachableCells[j]);
					j++;
				}
			}
			this.Cleanup();
		}

		public Navigator navigator;

		public NavGrid navGrid;

		public ulong gridClassification;

		public PathFinderAbilities abilities;

		public int originCell;

		public NavType startingNavType;

		public PathFinder.PotentialPath.Flags startingFlags;

		public ushort serialNo;

		public bool computeReachables;
	}

	private static class AsyncPathProbeWorker
	{
		public static void main(object _)
		{
			PathFinder.PotentialList potentialList = new PathFinder.PotentialList();
			PathFinder.PotentialScratchPad potentialScratchPad = new PathFinder.PotentialScratchPad(Pathfinding.Instance.MaxLinksPerCell());
			try
			{
				while (!AsyncPathProber.Instance.Halting())
				{
					AsyncPathProber.WorkOrder workOrder;
					AsyncPathProber.WorkResult workResult;
					if (AsyncPathProber.Instance.NextTask(out workOrder, out workResult))
					{
						workOrder.Execute(potentialList, potentialScratchPad, ref workResult);
						AsyncPathProber.Instance.WorkCompleted(workResult);
					}
					else
					{
						Thread.Sleep(1);
					}
				}
			}
			catch (Exception ex)
			{
				AsyncPathProber.Instance.SetException(ExceptionDispatchInfo.Capture(ex));
			}
		}
	}

	public class Manager
	{
		public bool Halting()
		{
			return this.halting;
		}

		public Manager()
		{
			this.navigatorOrderer = (Navigator lhs, Navigator rhs) => this.navigators.GetValueOrDefault(rhs, 0).CompareTo(this.navigators.GetValueOrDefault(lhs, 0));
		}

		public void SetException(ExceptionDispatchInfo ex)
		{
			lock (this)
			{
				this.agentException = ex;
			}
		}

		public void Register(Navigator nav)
		{
			lock (this)
			{
				if (this.navigators.ContainsKey(nav))
				{
					Debug.LogWarning("Double registration of navigator to AsyncManager: " + nav.ToString());
				}
				if (!this.gridPool.ContainsKey(nav.PathGrid.AllocatedClassification))
				{
					bool flag2 = false;
					try
					{
						Monitor.Enter(this, ref flag2);
						int width = nav.PathGrid.widthInCells;
						int height = nav.PathGrid.heightInCells;
						bool applyOffset = nav.PathGrid.applyOffset;
						NavType[] navTypes = new NavType[nav.PathGrid.ValidNavTypes.Length];
						nav.PathGrid.ValidNavTypes.CopyTo(navTypes, 0);
						this.gridPool[nav.PathGrid.AllocatedClassification] = new ObjectPool<PathGrid>(() => new PathGrid(width, height, applyOffset, navTypes), null, null, null, false, 4 + this.agents.Length, 4 + this.agents.Length);
					}
					finally
					{
						if (flag2)
						{
							Monitor.Exit(this);
						}
					}
				}
				this.navigators[nav] = 10000;
			}
		}

		public void Unregister(Navigator nav)
		{
			lock (this)
			{
				if (!this.navigators.Remove(nav))
				{
					Debug.LogWarning("Unregister of unknown navigator from AsyncManager: " + nav.ToString());
				}
			}
		}

		public void WorkCompleted(AsyncPathProber.WorkResult result)
		{
			lock (this)
			{
				this.finishedWork.Add(result);
			}
		}

		public bool NextTask(out AsyncPathProber.WorkOrder order, out AsyncPathProber.WorkResult result)
		{
			lock (this)
			{
				if (this.workQueue.Count > 0)
				{
					order = this.workQueue[this.workQueue.Count - 1];
					this.workQueue.RemoveAt(this.workQueue.Count - 1);
					if (this.navigators.ContainsKey(order.navigator))
					{
						result = new AsyncPathProber.WorkResult
						{
							navigator = order.navigator,
							pathGrid = this.gridPool[order.gridClassification].Get(),
							newlyReachableCells = this.indexListPool.Get(),
							noLongerReachableCells = this.indexListPool.Get(),
							reachableCells = this.indexListPool.Get()
						};
						this.navigators[order.navigator] = -1;
						return true;
					}
				}
			}
			order = default(AsyncPathProber.WorkOrder);
			result = default(AsyncPathProber.WorkResult);
			return false;
		}

		private AsyncPathProber.WorkOrder makeWorkOrder(Navigator nav)
		{
			PathFinderAbilities currentAbilities = nav.GetCurrentAbilities();
			return new AsyncPathProber.WorkOrder
			{
				navigator = nav,
				navGrid = nav.NavGrid,
				gridClassification = nav.PathGrid.AllocatedClassification,
				abilities = currentAbilities.Clone(),
				originCell = nav.cachedCell,
				startingNavType = nav.CurrentNavType,
				startingFlags = nav.flags,
				serialNo = this.activeSerialNo,
				computeReachables = nav.reportOccupation
			};
		}

		public void TickFrame()
		{
			lock (this)
			{
				if (this.agentException != null)
				{
					this.agentException.Throw();
					this.agentException = null;
				}
				this.activeSerialNo += 1;
				if (this.activeSerialNo == 0)
				{
					this.activeSerialNo += 1;
				}
				for (int i = 0; i < this.finishedWork.Count; i++)
				{
					AsyncPathProber.WorkResult workResult = this.finishedWork[i];
					PathGrid pathGrid = workResult.pathGrid;
					if (this.navigators.ContainsKey(workResult.navigator))
					{
						pathGrid = workResult.navigator.TakeResult(ref workResult);
						this.navigators[workResult.navigator] = 0;
					}
					if (pathGrid != null)
					{
						this.gridPool[pathGrid.AllocatedClassification].Release(pathGrid);
					}
					this.indexListPool.Release(workResult.reachableCells);
					this.indexListPool.Release(workResult.newlyReachableCells);
					this.indexListPool.Release(workResult.noLongerReachableCells);
				}
				this.finishedWork.Clear();
				foreach (KeyValuePair<Navigator, int> keyValuePair in this.navigators)
				{
					this.navigatorOrdering.Add(keyValuePair.Key);
				}
				for (int j = this.navigatorOrdering.Count - 1; j >= 0; j--)
				{
					Navigator navigator = this.navigatorOrdering[j];
					int num = this.navigators[navigator];
					if (num == -1)
					{
						this.navigatorOrdering.RemoveAtSwap<Navigator>(j);
					}
					else
					{
						this.navigators[navigator] = num + 1;
					}
				}
				this.navigatorOrdering.Sort(this.navigatorOrderer);
				for (int k = 0; k < this.workQueue.Count; k++)
				{
					this.workQueue[k].Cleanup();
				}
				this.workQueue.Clear();
				int num2 = 0;
				while (num2 < this.navigatorOrdering.Count && this.workQueue.Count < 4)
				{
					AsyncPathProber.WorkOrder workOrder = this.makeWorkOrder(this.navigatorOrdering[num2]);
					if (Grid.IsValidCell(workOrder.originCell))
					{
						this.workQueue.Add(workOrder);
					}
					else
					{
						workOrder.Cleanup();
					}
					num2++;
				}
				this.navigatorOrdering.Clear();
			}
		}

		public void ApplyNavigationFailedPenalty(Navigator nav)
		{
			AsyncPathProber.Manager.NavFailures++;
			lock (this)
			{
				int num;
				if (this.navigators.TryGetValue(nav, out num) && num >= 0)
				{
					this.navigators[nav] = num + 10;
				}
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

		private const int kNovelProberPenalty = 10000;

		private const int kFailedNavigationPenalty = 10;

		private const int kNavigatorInFlightValue = -1;

		private Dictionary<Navigator, int> navigators = new Dictionary<Navigator, int>();

		private List<Navigator> navigatorOrdering = new List<Navigator>();

		private Comparison<Navigator> navigatorOrderer;

		private ushort activeSerialNo;

		private Thread[] agents;

		private bool halting;

		private ExceptionDispatchInfo agentException;

		private List<AsyncPathProber.WorkOrder> workQueue = new List<AsyncPathProber.WorkOrder>();

		private List<AsyncPathProber.WorkResult> finishedWork = new List<AsyncPathProber.WorkResult>();

		private ConcurrentDictionary<ulong, ObjectPool<PathGrid>> gridPool = new ConcurrentDictionary<ulong, ObjectPool<PathGrid>>();

		private ObjectPool<List<int>> indexListPool = new ObjectPool<List<int>>(() => new List<int>(Grid.CellCount / 8), null, delegate(List<int> list)
		{
			list.Clear();
		}, null, false, 12, 10000);

		private static int NavFailures;
	}
}
