using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FishOvercrowingManager")]
public class FishOvercrowingManager : KMonoBehaviour, ISim1000ms
{
	public static void DestroyInstance()
	{
		FishOvercrowingManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		FishOvercrowingManager.Instance = this;
		this.cells = new FishOvercrowingManager.Cell[Grid.CellCount];
	}

	public void Add(KPrefabID aquaticEntity)
	{
		this.allAquaticEntities.Add(aquaticEntity);
	}

	public void Remove(KPrefabID aquaticEntity)
	{
		this.allAquaticEntities.Remove(aquaticEntity);
	}

	public void Sim1000ms(float dt)
	{
		int num = this.versionCounter;
		this.versionCounter = num + 1;
		int num2 = num;
		for (int num3 = 0; num3 != this.ponds.Count; num3++)
		{
			FishOvercrowingManager.Pond pond = this.ponds[num3];
			pond.fishes.Clear();
			pond.eggs.Clear();
			pond.cellCount = 0;
			pond.occupancy.dirty = true;
		}
		int num4 = ((this.ponds.Count == 0) ? (-1) : 0);
		QueuePool<int, FishOvercrowingManager>.PooledQueue pooledQueue = QueuePool<int, FishOvercrowingManager>.Allocate();
		foreach (KPrefabID kprefabID in this.allAquaticEntities)
		{
			if (!kprefabID.IsNullOrDestroyed())
			{
				int num5 = Grid.PosToCell(kprefabID);
				if (Grid.IsValidCell(num5))
				{
					pooledQueue.Clear();
					pooledQueue.Enqueue(num5);
					FishOvercrowingManager.Cell cell = this.cells[num5];
					int num6;
					if (cell.Version == num2)
					{
						num6 = cell.PondIndex;
					}
					else if (num4 != -1 && num4 < this.ponds.Count)
					{
						num6 = num4;
						num4++;
						if (num4 == this.ponds.Count)
						{
							num4 = -1;
						}
					}
					else
					{
						FishOvercrowingManager.Pond pond2 = new FishOvercrowingManager.Pond
						{
							fishes = new List<KPrefabID>(),
							eggs = new List<KPrefabID>()
						};
						this.ponds.Add(pond2);
						num6 = this.ponds.Count - 1;
					}
					FishOvercrowingManager.Pond pond3 = this.ponds[num6];
					if (kprefabID.HasTag(GameTags.Egg))
					{
						pond3.eggs.Add(kprefabID);
					}
					else
					{
						pond3.fishes.Add(kprefabID);
					}
					int num7;
					while (pooledQueue.TryDequeue(out num7))
					{
						if (Grid.IsValidCell(num7) && this.cells[num7].Version != num2 && Grid.IsNavigatableLiquid(num7))
						{
							this.cells[num7] = new FishOvercrowingManager.Cell(num2, num6);
							pond3.cellCount++;
							pooledQueue.Enqueue(Grid.CellLeft(num7));
							pooledQueue.Enqueue(Grid.CellRight(num7));
							pooledQueue.Enqueue(Grid.CellAbove(num7));
							pooledQueue.Enqueue(Grid.CellBelow(num7));
						}
					}
				}
			}
		}
		pooledQueue.Recycle();
		if (num4 != -1)
		{
			int num8 = this.ponds.Count - num4;
			if (num8 > 0)
			{
				this.ponds.RemoveRange(num4, num8);
			}
		}
		this.allAquaticEntities.RemoveAll(new Predicate<KPrefabID>(Util.IsNullOrDestroyed));
	}

	public FishOvercrowingManager.Pond GetPond(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return null;
		}
		FishOvercrowingManager.Cell cell2 = this.cells[cell];
		if (cell2.Version != this.versionCounter - 1)
		{
			return null;
		}
		return this.ponds[cell2.PondIndex];
	}

	public int GetFishInPondCount(int cell, HashSet<Tag> accepted_tags)
	{
		int num = 0;
		FishOvercrowingManager.Pond pond = this.GetPond(cell);
		if (pond == null)
		{
			return 0;
		}
		foreach (KPrefabID kprefabID in pond.fishes)
		{
			if (!kprefabID.HasTag(GameTags.Creatures.Bagged) && !kprefabID.HasTag(GameTags.Trapped) && accepted_tags.Contains(kprefabID.PrefabTag))
			{
				num++;
			}
		}
		return num;
	}

	public static FishOvercrowingManager Instance;

	private readonly List<KPrefabID> allAquaticEntities = new List<KPrefabID>();

	private readonly List<FishOvercrowingManager.Pond> ponds = new List<FishOvercrowingManager.Pond>();

	private FishOvercrowingManager.Cell[] cells;

	private int versionCounter = 2;

	private readonly struct Cell
	{
		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public int PondIndex
		{
			get
			{
				return this.pondIndex;
			}
		}

		public Cell(int version, int pondIndex)
		{
			this.version = version;
			this.pondIndex = pondIndex;
		}

		private readonly int version;

		private readonly int pondIndex;
	}

	public class Pond
	{
		public int FishCount
		{
			get
			{
				return this.fishes.Count;
			}
		}

		public int EggCount
		{
			get
			{
				return this.eggs.Count;
			}
		}

		public List<KPrefabID> fishes;

		public List<KPrefabID> eggs;

		public int cellCount;

		public OvercrowdingMonitor.Occupancy occupancy = new OvercrowdingMonitor.Occupancy();
	}
}
