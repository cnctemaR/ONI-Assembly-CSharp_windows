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
		this.grid = new FishOvercrowingManager.Cell[Grid.CellCount];
	}

	public void Add(KPrefabID aquaticEntity)
	{
		this.allAquaticEntities.Add(aquaticEntity);
	}

	public void Remove(KPrefabID aquaticEntity)
	{
		if (aquaticEntity.IsNullOrDestroyed())
		{
			return;
		}
		for (int i = this.allAquaticEntities.Count - 1; i >= 0; i--)
		{
			KPrefabID kprefabID = this.allAquaticEntities[i];
			if (!kprefabID.IsNullOrDestroyed() && kprefabID.InstanceID == aquaticEntity.InstanceID)
			{
				this.allAquaticEntities.RemoveAt(i);
				return;
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		int num = this.nextGeneration;
		this.nextGeneration = num + 1;
		int num2 = num;
		if (num2 == 0)
		{
			Array.Fill<FishOvercrowingManager.Cell>(this.grid, new FishOvercrowingManager.Cell
			{
				generation = 0,
				pondIndex = -1
			});
			num = this.nextGeneration;
			this.nextGeneration = num + 1;
			num2 = num;
		}
		for (int num3 = 0; num3 != this.ponds.Count; num3++)
		{
			FishOvercrowingManager.Pond pond = this.ponds[num3];
			pond.fishes.Clear();
			pond.eggs.Clear();
			pond.cellCount = 0;
			pond.occupancy.dirty = true;
		}
		int num4 = ((this.ponds.Count == 0) ? (-1) : 0);
		foreach (KPrefabID kprefabID in this.allAquaticEntities)
		{
			if (!kprefabID.IsNullOrDestroyed())
			{
				int num5 = Grid.PosToCell(kprefabID);
				if (Grid.IsValidCell(num5))
				{
					FishOvercrowingManager.Cell cell = this.grid[num5];
					bool flag = cell.generation != num2 || cell.pondIndex == -1;
					int num6;
					if (!flag)
					{
						num6 = cell.pondIndex;
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
					if (flag)
					{
						FloodFill.DepthTraverse<FloodFill.PredicateCondition, FishOvercrowingManager.VisitTracker, FloodFill.NoMaxDepth, FishOvercrowingManager.Visitor>(num5, new FloodFill.PredicateCondition(FishOvercrowingManager.isLiquidCell), new FishOvercrowingManager.VisitTracker(this.grid, num2), default(FloodFill.NoMaxDepth), new FishOvercrowingManager.Visitor(this.grid, this.ponds, num6));
					}
				}
			}
		}
		if (num4 != -1)
		{
			int num7 = this.ponds.Count - num4;
			if (num7 > 0)
			{
				this.ponds.RemoveRange(num4, num7);
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
		FishOvercrowingManager.Cell cell2 = this.grid[cell];
		if (cell2.generation != this.nextGeneration - 1 || cell2.pondIndex == -1)
		{
			return null;
		}
		return this.ponds[cell2.pondIndex];
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

	private FishOvercrowingManager.Cell[] grid;

	private int nextGeneration = 2;

	private static readonly Func<int, FloodFill.BoundaryCheckResult> isLiquidCell = delegate(int cell)
	{
		if (!Grid.IsNavigatableLiquidUnsafe(cell))
		{
			return FloodFill.BoundaryCheckResult.Halt;
		}
		return FloodFill.BoundaryCheckResult.Continue;
	};

	private struct Cell
	{
		public int generation;

		public int pondIndex;
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

	private readonly struct VisitTracker : FloodFill.IVisitTracker
	{
		public VisitTracker(FishOvercrowingManager.Cell[] grid, int generation)
		{
			this.grid = grid;
			this.generation = generation;
		}

		public bool Add(int cellIndex)
		{
			if (!this.Contains(cellIndex))
			{
				this.grid[cellIndex].generation = this.generation;
				return true;
			}
			return false;
		}

		public bool Contains(int cellIndex)
		{
			return this.grid[cellIndex].generation == this.generation;
		}

		private readonly FishOvercrowingManager.Cell[] grid;

		private readonly int generation;
	}

	private readonly struct Visitor : FloodFill.IVisitor
	{
		public Visitor(FishOvercrowingManager.Cell[] grid, List<FishOvercrowingManager.Pond> ponds, int pondIndex)
		{
			this.grid = grid;
			this.ponds = ponds;
			this.pondIndex = pondIndex;
		}

		public bool EarlyOut
		{
			get
			{
				return false;
			}
		}

		public void VisitCell(int cell)
		{
			this.ponds[this.pondIndex].cellCount++;
			this.grid[cell].pondIndex = this.pondIndex;
		}

		public void VisitBoundary(int cell)
		{
			this.grid[cell].pondIndex = -1;
		}

		private readonly FishOvercrowingManager.Cell[] grid;

		private readonly List<FishOvercrowingManager.Pond> ponds;

		private readonly int pondIndex;
	}
}
