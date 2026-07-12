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

	public void SetStatusItem(StatusItem si)
	{
		bool flag = this.showStatusItemOnEntombed;
		this.SetShowStatusItemOnEntombed(false);
		this.EntombedStatusItem = si;
		this.SetShowStatusItemOnEntombed(flag);
	}

	public void SetShowStatusItemOnEntombed(bool val)
	{
		this.showStatusItemOnEntombed = val;
		if (this.isEntombed && this.EntombedStatusItem != null)
		{
			if (this.showStatusItemOnEntombed)
			{
				this.selectable.AddStatusItem(this.EntombedStatusItem, null);
				return;
			}
			this.selectable.RemoveStatusItem(this.EntombedStatusItem, false);
		}
	}

	public string WiltStateString
	{
		get
		{
			return Db.Get().CreatureStatusItems.Entombed.resolveStringCallback(CREATURES.STATUSITEMS.ENTOMBED.LINE_ITEM, base.gameObject);
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
		if (this.EntombedStatusItem == null)
		{
			this.EntombedStatusItem = this.DefaultEntombedStatusItem;
		}
		this.partitionerEntry = GameScenePartitioner.Instance.Add("EntombVulnerable", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.CheckEntombed();
		if (this.isEntombed)
		{
			base.GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
			base.Trigger(-1089732772, true);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	private void OnSolidChanged(object data)
	{
		this.CheckEntombed();
	}

	private void CheckEntombed()
	{
		int num = Grid.PosToCell(base.gameObject.transform.GetPosition());
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (!this.IsCellSafe(num))
		{
			if (!this.isEntombed)
			{
				this.isEntombed = true;
				if (this.showStatusItemOnEntombed)
				{
					this.selectable.AddStatusItem(this.EntombedStatusItem, base.gameObject);
				}
				base.GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
				base.Trigger(-1089732772, true);
			}
		}
		else if (this.isEntombed)
		{
			this.isEntombed = false;
			this.selectable.RemoveStatusItem(this.EntombedStatusItem, false);
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
			base.Trigger(-1089732772, false);
		}
		if (this.operational != null)
		{
			this.operational.SetFlag(EntombVulnerable.notEntombedFlag, !this.isEntombed);
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.occupyArea.TestArea(cell, null, EntombVulnerable.IsCellSafeCBDelegate);
	}

	private static bool IsCellSafeCB(int cell, object data)
	{
		return Grid.IsValidCell(cell) && !Grid.Solid[cell];
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Operational operational;

	private OccupyArea _occupyArea;

	[Serialize]
	private bool isEntombed;

	private StatusItem DefaultEntombedStatusItem = Db.Get().CreatureStatusItems.Entombed;

	[NonSerialized]
	private StatusItem EntombedStatusItem;

	private bool showStatusItemOnEntombed = true;

	public static readonly Operational.Flag notEntombedFlag = new Operational.Flag("not_entombed", Operational.Flag.Type.Functional);

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly Func<int, object, bool> IsCellSafeCBDelegate = (int cell, object data) => EntombVulnerable.IsCellSafeCB(cell, data);
}
