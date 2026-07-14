using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Submergable")]
public class Submergable : KMonoBehaviour
{
	public bool IsSubmerged
	{
		get
		{
			return this.isSubmerged;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Submergable.OnSpawn", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnElementChanged));
		this.OnElementChanged(null);
		this.RefreshStatusItem();
	}

	protected virtual void OnElementChanged(object data)
	{
		bool flag = true;
		int num = Grid.PosToCell(base.gameObject);
		for (int i = 0; i < this.occupyArea.OccupiedCellsOffsets.Length; i++)
		{
			CellOffset cellOffset = this.occupyArea.OccupiedCellsOffsets[i];
			if (!Grid.IsLiquid(Grid.OffsetCell(num, cellOffset)))
			{
				flag = false;
				break;
			}
		}
		if (flag != this.isSubmerged)
		{
			this.isSubmerged = flag;
			this.OnSubmergedStateChanged();
			base.gameObject.Trigger(1983811727, null);
		}
	}

	protected virtual void OnSubmergedStateChanged()
	{
		this.RefreshStatusItem();
	}

	protected virtual void RefreshStatusItem()
	{
		if (this.GetStatusItem != null)
		{
			base.GetComponent<KSelectable>().ToggleStatusItem(this.GetStatusItem(), !this.isSubmerged, this);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	[MyCmpGet]
	private OccupyArea occupyArea;

	public Func<StatusItem> GetStatusItem;

	protected bool isSubmerged;

	private HandleVector<int>.Handle partitionerEntry;
}
