using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Harvestable : Workable
{
	protected Harvestable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public Worker completed_by { get; protected set; }

	public bool HarvestWhenReady
	{
		get
		{
			return this.harvestWhenReady;
		}
	}

	public bool CanBeHavested
	{
		get
		{
			return this.canBeHarvested;
		}
	}

	private void OnEnableOverlay(object data)
	{
		if ((SimViewMode)data == SimViewMode.HarvestWhenReady)
		{
			this.CreateOverlayIcon();
		}
		else
		{
			this.DestroyOverlayIcon();
		}
	}

	private void DestroyOverlayIcon()
	{
		if (this.HarvestWhenReadyOverlayIcon != null)
		{
			global::UnityEngine.Object.Destroy(this.HarvestWhenReadyOverlayIcon.gameObject);
			this.HarvestWhenReadyOverlayIcon = null;
		}
	}

	private void CreateOverlayIcon()
	{
		if (!(this.HarvestWhenReadyOverlayIcon != null))
		{
			if (base.GetComponent<Harvestable>() != null && base.GetComponent<AttackableBase>() == null)
			{
				this.HarvestWhenReadyOverlayIcon = Util.KInstantiate(Assets.UIPrefabs.HarvestWhenReadyOverlayIcon, GameScreenManager.Instance.worldSpaceCanvas, null).GetComponent<RectTransform>();
				OccupyArea component = base.GetComponent<OccupyArea>();
				Extents extents = component.GetExtents();
				KPrefabID component2 = base.GetComponent<KPrefabID>();
				Vector3 vector;
				if (component2.HasTag(GameTags.Hanging))
				{
					vector = new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)(extents.y + extents.height));
				}
				else
				{
					vector = new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)extents.y);
				}
				this.HarvestWhenReadyOverlayIcon.transform.position = vector;
				this.RefreshOverlayIcon(null);
			}
		}
	}

	private void RefreshOverlayIcon(object data = null)
	{
		if (this.HarvestWhenReadyOverlayIcon != null)
		{
			if (Grid.Visible[Grid.PosToCell(base.gameObject)] > 0 || DebugHandler.FreeCameraMode)
			{
				if (!this.HarvestWhenReadyOverlayIcon.gameObject.activeSelf)
				{
					this.HarvestWhenReadyOverlayIcon.gameObject.SetActive(true);
				}
			}
			else if (this.HarvestWhenReadyOverlayIcon.gameObject.activeSelf)
			{
				this.HarvestWhenReadyOverlayIcon.gameObject.SetActive(false);
			}
			HierarchyReferences component = this.HarvestWhenReadyOverlayIcon.GetComponent<HierarchyReferences>();
			if (this.harvestWhenReady)
			{
				component.GetReference("On").gameObject.SetActive(true);
				component.GetReference("Off").gameObject.SetActive(false);
			}
			else
			{
				component.GetReference("On").gameObject.SetActive(false);
				component.GetReference("Off").gameObject.SetActive(true);
			}
		}
	}

	private void OnDisableOverlay(object data)
	{
		this.DestroyOverlayIcon();
	}

	public void SetInPlanterBox(bool state)
	{
		if (state)
		{
			if (!this.isInPlanterBox)
			{
				this.isInPlanterBox = true;
				this.SetHarvestWhenReady(true);
			}
		}
		else
		{
			this.isInPlanterBox = false;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
		base.Subscribe(1309017699, delegate(object o)
		{
			this.SetInPlanterBox(true);
		});
	}

	protected override void OnSpawn()
	{
		base.Subscribe(2127324410, new Action<object>(this.ForceCancelHarvest));
		base.SetWorkTime(10f);
		base.Subscribe(2127324410, new Action<object>(this.OnCancel));
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.faceTargetWhenWorking = true;
		Components.Harvestables.Add(this);
		this.area = base.GetComponent<OccupyArea>();
		if (this.isMarkedForHarvest)
		{
			this.MarkForHarvest();
		}
		Game.Instance.Subscribe(1248612973, new Action<object>(this.OnEnableOverlay));
		Game.Instance.Subscribe(1798162660, new Action<object>(this.OnEnableOverlay));
		Game.Instance.Subscribe(2015652040, new Action<object>(this.OnDisableOverlay));
		this.iconRefreshHandle = UIScheduler.Instance.SchedulePeriodic("RefreshHarvestIcon", 1f, new Action<object>(this.RefreshOverlayIcon), null, null);
	}

	public void Harvest()
	{
		this.isMarkedForHarvest = false;
		this.chore = null;
		base.Trigger(1272413801, this);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		this.userMenu.Refresh();
	}

	public void SetHarvestWhenReady(bool state)
	{
		this.harvestWhenReady = state;
		if (this.harvestWhenReady && this.canBeHarvested && !this.isMarkedForHarvest)
		{
			this.MarkForHarvest();
		}
		if (this.isMarkedForHarvest && !this.harvestWhenReady)
		{
			this.OnCancel(null);
			if (this.canBeHarvested && this.isInPlanterBox)
			{
				this.selectable.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		this.RefreshOverlayIcon(null);
	}

	public void SetCanBeHarvested(bool state)
	{
		this.canBeHarvested = state;
		if (this.canBeHarvested)
		{
			this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, null);
			if (this.harvestWhenReady)
			{
				this.MarkForHarvest();
			}
			else if (this.isInPlanterBox)
			{
				this.selectable.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		else
		{
			this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, false);
			this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
		}
		this.userMenu.Refresh();
	}

	public virtual void MarkForHarvest()
	{
		if (this.canBeHarvested)
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, true, default(Tag), null, true, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
				this.selectable.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
			}
			this.isMarkedForHarvest = true;
			this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.completed_by = worker;
		this.Harvest();
	}

	protected virtual void OnCancel(object data)
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Cancel harvest");
			this.chore = null;
			this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
			this.SetHarvestWhenReady(false);
		}
		this.isMarkedForHarvest = false;
	}

	public bool HasChore()
	{
		return this.chore != null;
	}

	protected virtual void OnClickHarvestWhenReady()
	{
		this.SetHarvestWhenReady(true);
	}

	protected virtual void OnClickCancelHarvestWhenReady()
	{
		this.SetHarvestWhenReady(false);
	}

	public virtual void ForceCancelHarvest(object data = null)
	{
		this.OnCancel(null);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		this.userMenu.Refresh();
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		if (this.harvestWhenReady)
		{
			UserMenu userMenu = this.userMenu;
			string text = "action_harvest";
			string text2 = UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.NAME;
			global::System.Action action = delegate
			{
				this.OnClickCancelHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.GAMEOBJECTEFFECTS.PLANT_DO_NOT_HARVEST, base.transform, 1.5f, false);
			};
			string text3 = UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text3 = "action_harvest";
			string text2 = UI.USERMENUACTIONS.HARVEST_WHEN_READY.NAME;
			global::System.Action action = delegate
			{
				this.OnClickHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.GAMEOBJECTEFFECTS.PLANT_MARK_FOR_HARVEST, base.transform, 1.5f, false);
			};
			string text = UI.USERMENUACTIONS.HARVEST_WHEN_READY.TOOLTIP;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 1f);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.DestroyOverlayIcon();
		Game.Instance.Unsubscribe(1248612973, new Action<object>(this.OnEnableOverlay));
		Game.Instance.Unsubscribe(2015652040, new Action<object>(this.OnDisableOverlay));
		Game.Instance.Unsubscribe(1798162660, new Action<object>(this.OnEnableOverlay));
		this.iconRefreshHandle.ClearScheduler();
		Components.Harvestables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
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
	protected bool canBeHarvested = false;

	[Serialize]
	protected bool harvestWhenReady = false;

	public RectTransform HarvestWhenReadyOverlayIcon;

	[Serialize]
	private bool isInPlanterBox = false;

	protected Chore chore;

	public OccupyArea area;

	private SchedulerHandle iconRefreshHandle;
}
