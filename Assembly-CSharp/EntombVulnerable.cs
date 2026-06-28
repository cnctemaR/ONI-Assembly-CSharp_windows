using System;
using KSerialization;
using STRINGS;

public class EntombVulnerable : KMonoBehaviour, IWiltCause
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

	public string WiltStateString
	{
		get
		{
			return Db.Get().CreatureStatusItems.Entombed.resolveStringCallback(CREATURES.STATUSITEMS.ENTOMBED.LINE_ITEM, this);
		}
	}

	public WiltCondition.Condition[] Conditions
	{
		get
		{
			return new WiltCondition.Condition[] { WiltCondition.Condition.Entombed };
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("EntombVulnerable", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.CheckEntombed();
		if (this.isEntombed)
		{
			base.Trigger(-1089732772, true);
		}
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
		if (Grid.IsValidCell(num))
		{
			if (!this.IsCellSafe(num))
			{
				if (!this.isEntombed)
				{
					this.isEntombed = true;
					this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.Entombed, null);
					base.GetComponent<KPrefabID>().AddTag(GameTags.Entombed);
					base.Trigger(-1089732772, true);
				}
			}
			else if (this.isEntombed)
			{
				this.isEntombed = false;
				this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.Entombed, false);
				base.GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
				base.Trigger(-1089732772, false);
			}
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.occupyArea.TestArea(cell, null, (int testCell, object data) => Grid.IsValidCell(testCell) && !Grid.Solid[testCell]);
	}

	[MyCmpReq]
	private KSelectable selectable;

	private OccupyArea _occupyArea;

	[Serialize]
	private bool isEntombed = false;

	private GameScenePartitionerEntry partitionerEntry;
}
