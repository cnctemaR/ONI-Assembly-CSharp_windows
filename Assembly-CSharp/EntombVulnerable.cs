using System;
using KSerialization;

public class EntombVulnerable : KMonoBehaviour
{
	private OccupyArea occupyArea
	{
		get
		{
			if (this._occupyArea == null)
			{
				this._occupyArea = base.GetComponent<OccupyArea>();
			}
			return this._occupyArea;
		}
	}

	public bool GetEntombed
	{
		get
		{
			return this.isEntombed;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("EntombVulnerable", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedMask.mask, new Action<object>(this.OnSolidChanged));
		this.CheckEntombed();
	}

	protected override void OnCleanUp()
	{
		this.partitionerEntry.Release();
		base.OnCleanUp();
	}

	private void OnSolidChanged(object data)
	{
		this.CheckEntombed();
	}

	private void CheckEntombed()
	{
		int num = Grid.PosToCell(base.gameObject.transform.position);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (!this.IsCellSafe(num))
		{
			if (!this.isEntombed)
			{
				this.isEntombed = true;
				this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.Entombed, null);
				this.Trigger(-1089732772, true);
			}
		}
		else if (this.isEntombed)
		{
			this.isEntombed = false;
			this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.Entombed, false);
			this.Trigger(-1089732772, false);
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.occupyArea.TestArea(cell, (int testCell) => Grid.IsValidCell(testCell) && !Grid.Solid[testCell]);
	}

	[MyCmpReq]
	private KSelectable selectable;

	private OccupyArea _occupyArea;

	[Serialize]
	private bool isEntombed;

	private GameScenePartitionerEntry partitionerEntry;
}
