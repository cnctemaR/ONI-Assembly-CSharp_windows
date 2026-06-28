using System;
using KSerialization;
using UnityEngine;

public class EntombVulnerable : KMonoBehaviour
{
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
		this.handle = GameScheduler.Instance.SchedulePeriodic("CheckEntombed", 0.5f, new Action<object>(this.CheckEntombed), null, null, 0f);
		this.CheckEntombed(null);
	}

	public void Configure(bool twoTilesTall)
	{
		this.twoTilesTall = twoTilesTall;
	}

	public void CheckEntombed(object param)
	{
		int num = Grid.PosToCell(base.gameObject.transform.position);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (!this.IsCellSafe(num))
		{
			if (!this.isEntombed)
			{
				this.isEntombed = true;
				if (!component.HasStatusItem(Db.Get().CreatureStatusItems.Entombed))
				{
					component.AddStatusItem(Db.Get().CreatureStatusItems.Entombed, null);
				}
				this.Trigger(-1089732772, true);
			}
		}
		else if (this.isEntombed)
		{
			this.isEntombed = false;
			if (!component.HasStatusItem(Db.Get().CreatureStatusItems.Entombed))
			{
				component.RemoveStatusItem(Db.Get().CreatureStatusItems.Entombed);
			}
			this.Trigger(-1089732772, false);
		}
	}

	public bool IsCellSafe(int cell)
	{
		return !Grid.Solid[cell] && (!this.twoTilesTall || !Grid.Solid[Grid.CellAbove(cell)]);
	}

	protected override void OnCleanUp()
	{
		this.handle.Clear();
		base.OnCleanUp();
	}

	[Serialize]
	private bool isEntombed;

	[HideInInspector]
	public bool twoTilesTall;

	private SchedulerHandle handle;
}
