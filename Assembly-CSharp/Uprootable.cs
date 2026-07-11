using System;
using KSerialization;
using STRINGS;
using TUNING;

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
		this.attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.multitoolContext = "harvest";
		this.multitoolHitEffectTag = "fx_harvest_splash";
		base.Subscribe<Uprootable>(1309017699, Uprootable.OnPlanterStorageDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Uprootable>(2127324410, Uprootable.ForceCancelUprootDelegate);
		base.SetWorkTime(12.5f);
		base.Subscribe<Uprootable>(2127324410, Uprootable.OnCancelDelegate);
		base.Subscribe<Uprootable>(493375141, Uprootable.OnRefreshUserMenuDelegate);
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
		base.Trigger(-216549700, this);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void SetCanBeUprooted(bool state)
	{
		this.canBeUprooted = state;
		if (this.canBeUprooted)
		{
			this.SetUprootedComplete(false);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
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
			this.chore = new WorkChore<Uprootable>(Db.Get().ChoreTypes.Uproot, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
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
		Game.Instance.userMenu.Refresh(base.gameObject);
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
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.chore != null)
		{
			string text = "action_uproot";
			string text2 = this.cancelButtonLabel;
			global::System.Action action = new global::System.Action(this.OnClickCancelUproot);
			string text3 = this.cancelButtonTooltip;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_uproot";
			string text2 = this.buttonLabel;
			global::System.Action action = new global::System.Action(this.OnClickUproot);
			string text = this.buttonTooltip;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
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

	[Serialize]
	protected bool isMarkedForUproot;

	protected bool uprootComplete;

	[MyCmpReq]
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

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnPlanterStorageDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnPlanterStorage(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> ForceCancelUprootDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.ForceCancelUproot(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
