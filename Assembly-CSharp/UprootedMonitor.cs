using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UprootedMonitor")]
public class UprootedMonitor : KMonoBehaviour
{
	public bool IsUprooted
	{
		get
		{
			return this.uprooted || base.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);
		}
	}

	protected override void OnPrefabInit()
	{
		base.Subscribe<UprootedMonitor>(-216549700, UprootedMonitor.OnUprootedDelegate);
		this.position = Grid.PosToCell(base.gameObject);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RegisterMonitoredCellsPartitionerEntries();
	}

	public void SetNewMonitorCells(CellOffset[] cellsOffsets)
	{
		this.UnregisterMonitoredCellsPartitionerEntries();
		this.monitorCells = cellsOffsets;
		this.RegisterMonitoredCellsPartitionerEntries();
	}

	private void UnregisterMonitoredCellsPartitionerEntries()
	{
		foreach (HandleVector<int>.Handle handle in this.partitionerEntries)
		{
			GameScenePartitioner.Instance.Free(ref handle);
		}
		this.partitionerEntries.Clear();
	}

	private void RegisterMonitoredCellsPartitionerEntries()
	{
		foreach (CellOffset cellOffset in this.monitorCells)
		{
			int num = Grid.OffsetCell(this.position, cellOffset);
			if (Grid.IsValidCell(this.position) && Grid.IsValidCell(num))
			{
				this.partitionerEntries.Add(GameScenePartitioner.Instance.Add("UprootedMonitor.OnSpawn", base.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnGroundChanged)));
			}
		}
		this.OnGroundChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.UnregisterMonitoredCellsPartitionerEntries();
		base.OnCleanUp();
	}

	public bool CheckTileGrowable()
	{
		return !this.canBeUprooted || (!this.uprooted && this.IsSuitableFoundation(this.position));
	}

	public bool IsSuitableFoundation(int cell)
	{
		bool flag = true;
		foreach (CellOffset cellOffset in this.monitorCells)
		{
			if (!Grid.IsCellOffsetValid(cell, cellOffset))
			{
				return false;
			}
			int num = Grid.OffsetCell(cell, cellOffset);
			if (this.customFoundationCheckFn != null)
			{
				flag = this.customFoundationCheckFn(num);
			}
			else
			{
				flag = Grid.Solid[num];
			}
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!this.CheckTileGrowable())
		{
			this.uprooted = true;
		}
		if (this.uprooted)
		{
			base.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			base.Trigger(-216549700, null);
		}
	}

	private int position;

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted;

	public CellOffset[] monitorCells = new CellOffset[]
	{
		new CellOffset(0, -1)
	};

	public Func<int, bool> customFoundationCheckFn;

	private List<HandleVector<int>.Handle> partitionerEntries = new List<HandleVector<int>.Handle>();

	private static readonly EventSystem.IntraObjectHandler<UprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<UprootedMonitor>(delegate(UprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			component.uprooted = true;
			component.Trigger(-216549700, null);
		}
	});
}
