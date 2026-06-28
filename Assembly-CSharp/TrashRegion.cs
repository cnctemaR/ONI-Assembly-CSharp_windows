using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class TrashRegion : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		List<Tag> allTags = TagFilterScreen.GetAllTags();
		this.filterable.SetTags(allTags);
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		this.storage.preferPrimaryCell = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnFilterChanged(this.filterable.GetTags());
		base.GetComponent<Region>().OnRegionChanged += this.Refresh;
		this.Refresh();
	}

	private void OnFilterChanged(Tag[] tags)
	{
		this.ClearChores();
		this.SetStatus();
		bool flag = tags == null || tags.Length == 0;
		this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoStorageFilterSet, flag, this);
		this.Refresh();
	}

	private void Refresh()
	{
		HashSet<CellOffset> availableOffsets = this.GetAvailableOffsets();
		if (availableOffsets.Count == 0)
		{
			this.MarkAsFull();
		}
		else if (this.IsFull())
		{
			this.MarkAsAvailable();
		}
		CellOffset[] array = new CellOffset[availableOffsets.Count];
		availableOffsets.CopyTo(array);
		this.storage.SetOffsets(array);
		if (this.solidChangedEntry != null)
		{
			this.solidChangedEntry.Release();
		}
		if (array.Length > 0)
		{
			Extents extents = new Extents(Grid.PosToCell(this), array);
			extents.x--;
			extents.y--;
			extents.width += 2;
			extents.height += 2;
			this.solidChangedEntry = GameScenePartitioner.Instance.Add("Region.RegionChanged", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, delegate(object obj)
			{
				this.Refresh();
			});
		}
	}

	private HashSet<CellOffset> GetAvailableOffsets()
	{
		HashSet<CellOffset> hashSet = new HashSet<CellOffset>();
		HashSet<int> availableCells = this.GetAvailableCells();
		if (availableCells.Count > 0)
		{
			int num = Grid.PosToCell(this);
			foreach (int num2 in availableCells)
			{
				foreach (CellOffset cellOffset in OffsetGroups.Standard)
				{
					int num3 = Grid.OffsetCell(num2, cellOffset);
					CellOffset offset = Grid.GetOffset(num, num3);
					hashSet.Add(offset);
				}
			}
		}
		return hashSet;
	}

	private HashSet<int> GetAvailableCells()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int num in base.GetComponent<Region>().Cells)
		{
			if (TrashRegion.HasRoom(num))
			{
				hashSet.Add(num);
			}
		}
		return hashSet;
	}

	private static bool HasRoom(int cell)
	{
		if (Grid.Solid[cell])
		{
			return false;
		}
		int num = Grid.CellAbove(cell);
		return !(Grid.Objects[num, 3] != null);
	}

	public bool IsFull()
	{
		return this.fetchChores.Count == 0;
	}

	private void CreateFetchChore()
	{
		Tag[] tags = this.filterable.GetTags();
		Action<Chore> action = new Action<Chore>(this.OnFetchComplete);
		Action<Chore> action2 = new Action<Chore>(this.OnFetchStart);
		Action<Chore> action3 = new Action<Chore>(this.OnFetchEnd);
		FetchChore fetchChore = new FetchChore(this.storage, 2.1474836E+09f, tags, null, null, true, action, action2, action3, FetchOrder2.OperationalRequirement.Operational, 0);
		this.fetchChores.Add(fetchChore);
	}

	private void OnFetchStart(Chore chore)
	{
		this.CreateFetchChore();
	}

	private void OnFetchEnd(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (this.fetchChores.Contains(fetchChore))
		{
			this.fetchChores.Remove(fetchChore);
			fetchChore.Cancel("TrashRegion redistribution");
		}
	}

	private void OnFetchComplete(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		this.fetchChores.Remove(fetchChore);
		Pickupable fetchTarget = fetchChore.fetchTarget;
		if (fetchTarget != null)
		{
			this.storage.Drop(fetchTarget.gameObject);
			int num = Grid.PosToCell(fetchChore.fetcher.transform.position);
			int nearestCell = this.GetNearestCell(num);
			fetchTarget.transform.SetPosition(Grid.CellToPosCCC(nearestCell, Grid.SceneLayer.Move));
		}
	}

	private int GetNearestCell(int cell)
	{
		int num = Grid.InvalidCell;
		int num2 = int.MaxValue;
		int num3 = Grid.InvalidCell;
		int num4 = int.MaxValue;
		foreach (int num5 in this.GetAvailableCells())
		{
			int cellDistance = Grid.GetCellDistance(cell, num5);
			if (TrashRegion.HasRoom(num5) && cellDistance < num4)
			{
				num3 = num5;
				num4 = cellDistance;
			}
			if (cellDistance < num2)
			{
				num = num5;
				num2 = cellDistance;
			}
		}
		if (num3 != Grid.InvalidCell)
		{
			return num3;
		}
		return num;
	}

	private void MarkAsAvailable()
	{
		if (!this.IsFull())
		{
			return;
		}
		this.CreateFetchChore();
	}

	private void ClearChores()
	{
		for (int i = this.fetchChores.Count - 1; i >= 0; i--)
		{
			this.fetchChores[i].Cancel("Storage region full.");
		}
		this.fetchChores.Clear();
	}

	private void MarkAsFull()
	{
		if (this.IsFull())
		{
			return;
		}
		this.ClearChores();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.solidChangedEntry != null)
		{
			this.solidChangedEntry.Release();
		}
	}

	private void SetStatus()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.TreeFilterableTags, base.GetComponent<TreeFilterable>());
	}

	private List<FetchChore> fetchChores = new List<FetchChore>();

	[MyCmpReq]
	private Storage storage;

	[MyCmpAdd]
	private TreeFilterable filterable;

	[MyCmpGet]
	private KSelectable selectable;

	private ScenePartitionerEntry solidChangedEntry;
}
