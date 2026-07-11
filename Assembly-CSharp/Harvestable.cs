using System;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Harvestable : Workable
{
	protected Harvestable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.onEnableOverlayDelegate = new Action<object>(this.OnEnableOverlay);
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
		if ((HashedString)data == OverlayModes.Harvest.ID)
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
		if (this.HarvestWhenReadyOverlayIcon != null)
		{
			return;
		}
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
			this.HarvestWhenReadyOverlayIcon.transform.SetPosition(vector);
			this.RefreshOverlayIcon(null);
		}
	}

	private void RefreshOverlayIcon(object data = null)
	{
		if (this.HarvestWhenReadyOverlayIcon != null)
		{
			if (Grid.IsVisible(Grid.PosToCell(base.gameObject)) || (CameraController.Instance != null && CameraController.Instance.FreeCameraEnabled))
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
				this.SetHarvestWhenReady(this.defaultHarvestStateWhenPlanted);
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
		this.multitoolContext = "harvest";
		this.multitoolHitEffectTag = "fx_harvest_splash";
		base.Subscribe<Harvestable>(1309017699, Harvestable.SetInPlanterBoxTrueDelegate);
	}

	protected override void OnSpawn()
	{
		base.Subscribe<Harvestable>(2127324410, Harvestable.ForceCancelHarvestDelegate);
		base.SetWorkTime(10f);
		base.Subscribe<Harvestable>(2127324410, Harvestable.OnCancelDelegate);
		base.Subscribe<Harvestable>(493375141, Harvestable.OnRefreshUserMenuDelegate);
		this.faceTargetWhenWorking = true;
		Components.Harvestables.Add(this);
		this.attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.area = base.GetComponent<OccupyArea>();
		if (this.isMarkedForHarvest)
		{
			this.MarkForHarvest();
		}
		Game.Instance.Subscribe(1248612973, this.onEnableOverlayDelegate);
		Game.Instance.Subscribe(1798162660, this.onEnableOverlayDelegate);
		Game.Instance.Subscribe(2015652040, new Action<object>(this.OnDisableOverlay));
	}

	public void Harvest()
	{
		this.isMarkedForHarvest = false;
		this.chore = null;
		base.Trigger(1272413801, this);
		KSelectable component = base.GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
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
				KSelectable component = base.GetComponent<KSelectable>();
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		this.RefreshOverlayIcon(null);
	}

	public void SetCanBeHarvested(bool state)
	{
		this.canBeHarvested = state;
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.canBeHarvested)
		{
			component.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, null);
			if (this.harvestWhenReady)
			{
				this.MarkForHarvest();
			}
			else if (this.isInPlanterBox)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		else
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, false);
			component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public virtual void MarkForHarvest()
	{
		if (!this.canBeHarvested)
		{
			return;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.chore == null)
		{
			this.chore = new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, this, null, null, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			component.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
		}
		this.isMarkedForHarvest = true;
		component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
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
			KSelectable component = base.GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
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
		KSelectable component = base.GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.harvestWhenReady)
		{
			string text = "action_harvest";
			string text2 = UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.NAME;
			global::System.Action action = delegate
			{
				this.OnClickCancelHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.GAMEOBJECTEFFECTS.PLANT_DO_NOT_HARVEST, base.transform, 1.5f, false);
			};
			string text3 = UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_harvest";
			string text2 = UI.USERMENUACTIONS.HARVEST_WHEN_READY.NAME;
			global::System.Action action = delegate
			{
				this.OnClickHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.GAMEOBJECTEFFECTS.PLANT_MARK_FOR_HARVEST, base.transform, 1.5f, false);
			};
			string text = UI.USERMENUACTIONS.HARVEST_WHEN_READY.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.DestroyOverlayIcon();
		Game.Instance.Unsubscribe(1248612973, this.onEnableOverlayDelegate);
		Game.Instance.Unsubscribe(2015652040, new Action<object>(this.OnDisableOverlay));
		Game.Instance.Unsubscribe(1798162660, this.onEnableOverlayDelegate);
		Components.Harvestables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KSelectable component = base.GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
	}

	[Serialize]
	protected bool isMarkedForHarvest;

	[Serialize]
	protected bool canBeHarvested;

	[Serialize]
	protected bool harvestWhenReady;

	public bool defaultHarvestStateWhenPlanted = true;

	public RectTransform HarvestWhenReadyOverlayIcon;

	[Serialize]
	private bool isInPlanterBox;

	protected Chore chore;

	public OccupyArea area;

	private Action<object> onEnableOverlayDelegate;

	private static readonly EventSystem.IntraObjectHandler<Harvestable> ForceCancelHarvestDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.ForceCancelHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> SetInPlanterBoxTrueDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.SetInPlanterBox(true);
	});
}
