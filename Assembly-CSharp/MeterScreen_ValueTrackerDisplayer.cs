using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MeterScreen_ValueTrackerDisplayer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.Tooltip.OnToolTip = new Func<string>(this.OnTooltip);
		base.OnSpawn();
	}

	public void Refresh()
	{
		this.RefreshWorldMinionIdentities();
		this.InternalRefresh();
	}

	protected abstract void InternalRefresh();

	protected abstract string OnTooltip();

	public virtual void OnClick(BaseEventData base_ev_data)
	{
	}

	private void RefreshWorldMinionIdentities()
	{
		this.worldLiveMinionIdentities = new List<MinionIdentity>(from x in Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId, false)
			where !x.IsNullOrDestroyed()
			select x);
	}

	protected virtual List<MinionIdentity> GetWorldMinionIdentities()
	{
		if (this.worldLiveMinionIdentities == null)
		{
			this.RefreshWorldMinionIdentities();
		}
		if (this.minionListCustomSortOperation != null)
		{
			this.worldLiveMinionIdentities = this.minionListCustomSortOperation(this.worldLiveMinionIdentities);
		}
		return this.worldLiveMinionIdentities;
	}

	protected virtual List<MinionIdentity> GetAllMinionsFromAllWorlds()
	{
		List<MinionIdentity> list = new List<MinionIdentity>(Components.LiveMinionIdentities.Items.Where<MinionIdentity>((MinionIdentity x) => !x.IsNullOrDestroyed()));
		if (this.minionListCustomSortOperation != null)
		{
			this.worldLiveMinionIdentities = this.minionListCustomSortOperation(list);
		}
		return list;
	}

	public LocText Label;

	public ToolTip Tooltip;

	public GameObject diagnosticGraph;

	public TextStyleSetting ToolTipStyle_Header;

	public TextStyleSetting ToolTipStyle_Property;

	protected Func<List<MinionIdentity>, List<MinionIdentity>> minionListCustomSortOperation;

	private List<MinionIdentity> worldLiveMinionIdentities;
}
