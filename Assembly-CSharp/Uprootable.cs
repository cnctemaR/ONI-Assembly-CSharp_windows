using System;
using KSerialization;
using STRINGS;

public class Uprootable : Workable
{
	protected Uprootable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.buttonLabel = UI.USERMENUACTIONS.UPROOT.NAME;
		this.buttonTooltip = UI.USERMENUACTIONS.UPROOT.TOOLTIP;
		this.cancelButtonLabel = UI.USERMENUACTIONS.CANCELUPROOT.NAME;
		this.cancelButtonTooltip = UI.USERMENUACTIONS.CANCELUPROOT.TOOLTIP;
		this.pendingStatusItem = Db.Get().MiscStatusItems.PendingUproot;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Uprooting;
	}

	public bool IsMarkedForUproot
	{
		get
		{
			return this.isMarkedForUproot;
		}
	}

	public Storage GetPlanterStorage
	{
		get
		{
			return this.planterStorage;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.pendingStatusItem = Db.Get().MiscStatusItems.PendingUproot;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Uprooting;
		this.Subscribe(1309017699, new Action<object>(this.OnPlanterStorage));
	}

	protected override void OnSpawn()
	{
		this.Subscribe(2127324410, new Action<object>(this.ForceCancelUproot));
		base.SetWorkTime(12.5f);
		this.Subscribe(2127324410, new Action<object>(this.OnCancel));
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.faceTargetWhenWorking = true;
		Components.Uprootables.Add(this);
		this.area = base.GetComponent<OccupyArea>();
		Prioritizable.AddRef(base.gameObject);
		if (this.isMarkedForUproot)
		{
			this.MarkForUproot();
		}
	}

	private void OnPlanterStorage(object data)
	{
		this.planterStorage = (Storage)data;
		Prioritizable component = base.GetComponent<Prioritizable>();
		if (component != null)
		{
			component.showIcon = this.planterStorage == null;
		}
	}

	public bool IsInPlanterBox()
	{
		return this.planterStorage != null;
	}

	public void Uproot()
	{
		this.isMarkedForUproot = false;
		this.chore = null;
		this.uprootComplete = true;
		this.Trigger(-216549700, this);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		this.userMenu.Refresh();
	}

	public void SetCanBeUprooted(bool state)
	{
		this.canBeUprooted = state;
		if (this.canBeUprooted)
		{
			this.SetUprootedComplete(false);
		}
		this.userMenu.Refresh();
	}

	public void SetUprootedComplete(bool state)
	{
		this.uprootComplete = state;
	}

	public void MarkForUproot()
	{
		if (!this.canBeUprooted)
		{
			return;
		}
		if (DebugHandler.InstantBuildMode)
		{
			this.Uproot();
		}
		else if (this.chore == null)
		{
			this.chore = new WorkChore<Uprootable>(Db.Get().ChoreTypes.Uproot, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true, int.MaxValue);
			base.GetComponent<KSelectable>().AddStatusItem(this.pendingStatusItem, this);
		}
		this.isMarkedForUproot = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.Uproot();
	}

	private void OnCancel(object data)
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancel uproot");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
		}
		this.isMarkedForUproot = false;
		this.userMenu.Refresh();
	}

	public bool HasChore()
	{
		return this.chore != null;
	}

	private void OnClickUproot()
	{
		this.MarkForUproot();
	}

	protected void OnClickCancelUproot()
	{
		this.OnCancel(null);
	}

	public virtual void ForceCancelUproot(object data = null)
	{
		this.OnCancel(null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.uprootComplete)
		{
			if (this.deselectOnUproot)
			{
				KSelectable component = base.GetComponent<KSelectable>();
				if (component != null && SelectTool.Instance.selected == component)
				{
					SelectTool.Instance.Select(null, false);
				}
			}
			return;
		}
		if (!this.canBeUprooted)
		{
			return;
		}
		if (this.chore != null)
		{
			UserMenu userMenu = this.userMenu;
			string text = this.cancelButtonTooltip;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_uproot", this.cancelButtonLabel, new global::System.Action(this.OnClickCancelUproot), global::Action.NumActions, null, null, null, text, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = this.buttonTooltip;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_uproot", this.buttonLabel, new global::System.Action(this.OnClickUproot), global::Action.NumActions, null, null, null, text, true), 1f);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Uprootables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
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
	protected bool isMarkedForUproot;

	protected bool uprootComplete;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[Serialize]
	protected bool canBeUprooted = true;

	public bool deselectOnUproot = true;

	protected Chore chore;

	private string buttonLabel;

	private string buttonTooltip;

	private string cancelButtonLabel;

	private string cancelButtonTooltip;

	private StatusItem pendingStatusItem;

	public OccupyArea area;

	private Storage planterStorage;
}
