using System;
using KSerialization;
using STRINGS;

public class Harvestable : Workable
{
	protected Harvestable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
	}

	protected override void OnSpawn()
	{
		this.Subscribe(2127324410, new EventSystem.EventHandler(this.ForceCancelHarvest));
		base.SetWorkTime(10f);
		this.Subscribe(2127324410, new EventSystem.EventHandler(this.OnCancel));
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.faceTargetWhenWorking = true;
		Components.Harvestables.Add(this);
		this.area = base.GetComponent<OccupyArea>();
		if (this.isMarkedForHarvest)
		{
			this.MarkForHarvest();
		}
	}

	public void Harvest()
	{
		this.isMarkedForHarvest = false;
		this.chore = null;
		this.Trigger(1272413801, this);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.Operating);
		if (this.deselectOnHarvest && SelectTool.Instance.selected == this.selectable)
		{
			SelectTool.Instance.Select(null, false);
		}
		this.userMenu.Refresh();
	}

	public void SetCanBeHarvested(bool state)
	{
		this.canBeHarvested = state;
		if (this.canBeHarvested)
		{
			this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, null);
		}
		else
		{
			this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest);
		}
		this.userMenu.Refresh();
	}

	public virtual void MarkForHarvest()
	{
		if (!this.canBeHarvested)
		{
			return;
		}
		if (this.chore == null)
		{
			this.chore = new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
			this.selectable.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
		}
		this.isMarkedForHarvest = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.Harvest();
	}

	protected virtual void OnCancel(object data)
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancel harvest");
			this.chore = null;
			this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
		}
		this.isMarkedForHarvest = false;
	}

	public bool HasChore()
	{
		return this.chore != null;
	}

	protected virtual void OnClickHarvest()
	{
		this.MarkForHarvest();
	}

	protected void OnClickCancelHarvest()
	{
		this.OnCancel(null);
	}

	public virtual void ForceCancelHarvest(object data = null)
	{
		this.OnCancel(null);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
		this.userMenu.Refresh();
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		if (this.chore != null)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.CANCELHARVEST.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_harvest", UI.USERMENUACTIONS.CANCELHARVEST.NAME, new global::System.Action(this.OnClickCancelHarvest), global::Action.NumActions, null, null, null, null, text));
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			Func<bool> func = () => this.canBeHarvested;
			string text = ((!this.canBeHarvested) ? UI.USERMENUACTIONS.HARVEST.TOOLTIP_DISABLED : UI.USERMENUACTIONS.HARVEST.TOOLTIP);
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_harvest", UI.USERMENUACTIONS.HARVEST.NAME, new global::System.Action(this.OnClickHarvest), global::Action.NumActions, null, func, null, null, text));
		}
	}

	protected override void OnCleanUp()
	{
		Components.Harvestables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.Trigger(-1358696400, worker);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.Trigger(-942831938, worker);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "harvest", EffectPrefabs.Instance.HarvestEffect);
		return anim;
	}

	[MyCmpAdd]
	protected UserMenu userMenu;

	[Serialize]
	protected bool isMarkedForHarvest;

	[Serialize]
	protected bool canBeHarvested;

	public bool deselectOnHarvest = true;

	protected Chore chore;

	public OccupyArea area;
}
