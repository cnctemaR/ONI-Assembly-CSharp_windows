using System;
using System.Collections.Generic;

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

	public void Add(FishOvercrowdingMonitor.Instance fish)
	{
		this.fishes.Add(fish);
	}

	public void Remove(FishOvercrowdingMonitor.Instance fish)
	{
		this.fishes.Remove(fish);
	}

	public void Sim1000ms(float dt)
	{
		int num = this.versionCounter++;
		int num2 = 1;
		this.cavityIdToCavityInfo.Clear();
		this.cellToFishCount.Clear();
		ListPool<FishOvercrowingManager.FishInfo, FishOvercrowingManager>.PooledList pooledList = ListPool<FishOvercrowingManager.FishInfo, FishOvercrowingManager>.Allocate();
		foreach (FishOvercrowdingMonitor.Instance instance in this.fishes)
		{
			int num3 = Grid.PosToCell(instance);
			if (Grid.IsValidCell(num3))
			{
				FishOvercrowingManager.FishInfo fishInfo = new FishOvercrowingManager.FishInfo
				{
					cell = num3,
					fish = instance
				};
				pooledList.Add(fishInfo);
				int num4 = 0;
				this.cellToFishCount.TryGetValue(num3, out num4);
				num4++;
				this.cellToFishCount[num3] = num4;
			}
		}
		foreach (FishOvercrowingManager.FishInfo fishInfo2 in pooledList)
		{
			ListPool<int, FishOvercrowingManager>.PooledList pooledList2 = ListPool<int, FishOvercrowingManager>.Allocate();
			pooledList2.Add(fishInfo2.cell);
			int i = 0;
			int num5 = num2++;
			while (i < pooledList2.Count)
			{
				int num6 = pooledList2[i++];
				if (Grid.IsValidCell(num6))
				{
					FishOvercrowingManager.Cell cell = this.cells[num6];
					if (cell.version != num)
					{
						if (Grid.IsLiquid(num6))
						{
							cell.cavityId = num5;
							cell.version = num;
							int num7 = 0;
							this.cellToFishCount.TryGetValue(num6, out num7);
							FishOvercrowingManager.CavityInfo cavityInfo = default(FishOvercrowingManager.CavityInfo);
							if (!this.cavityIdToCavityInfo.TryGetValue(num5, out cavityInfo))
							{
								cavityInfo = default(FishOvercrowingManager.CavityInfo);
							}
							cavityInfo.fishCount += num7;
							cavityInfo.cellCount++;
							this.cavityIdToCavityInfo[num5] = cavityInfo;
							pooledList2.Add(Grid.CellLeft(num6));
							pooledList2.Add(Grid.CellRight(num6));
							pooledList2.Add(Grid.CellAbove(num6));
							pooledList2.Add(Grid.CellBelow(num6));
							this.cells[num6] = cell;
						}
					}
				}
			}
			pooledList2.Recycle();
		}
		foreach (FishOvercrowingManager.FishInfo fishInfo3 in pooledList)
		{
			FishOvercrowingManager.Cell cell2 = this.cells[fishInfo3.cell];
			FishOvercrowingManager.CavityInfo cavityInfo2 = default(FishOvercrowingManager.CavityInfo);
			this.cavityIdToCavityInfo.TryGetValue(cell2.cavityId, out cavityInfo2);
			fishInfo3.fish.SetOvercrowdingInfo(cavityInfo2.cellCount, cavityInfo2.fishCount);
		}
		pooledList.Recycle();
	}

	public static FishOvercrowingManager Instance;

	private List<FishOvercrowdingMonitor.Instance> fishes = new List<FishOvercrowdingMonitor.Instance>();

	private Dictionary<int, FishOvercrowingManager.CavityInfo> cavityIdToCavityInfo = new Dictionary<int, FishOvercrowingManager.CavityInfo>();

	private Dictionary<int, int> cellToFishCount = new Dictionary<int, int>();

	private FishOvercrowingManager.Cell[] cells;

	private int versionCounter = 1;

	private struct Cell
	{
		public int version;

		public int cavityId;
	}

	private struct FishInfo
	{
		public int cell;

		public FishOvercrowdingMonitor.Instance fish;
	}

	private struct CavityInfo
	{
		public int fishCount;

		public int cellCount;
	}
}
