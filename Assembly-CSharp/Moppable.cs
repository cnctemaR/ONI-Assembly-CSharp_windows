using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class Moppable : Workable, ISim1000ms, ISim200ms
{
	private Moppable()
	{
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.childRenderer = base.GetComponentInChildren<MeshRenderer>();
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!this.IsThereLiquid())
		{
			base.gameObject.DeleteObject();
			return;
		}
		Grid.Objects[Grid.PosToCell(base.gameObject), 8] = base.gameObject;
		new WorkChore<Moppable>(Db.Get().ChoreTypes.Mop, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		base.SetWorkTime(float.PositiveInfinity);
		KSelectable component = base.GetComponent<KSelectable>();
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForMop, null);
		base.Subscribe<Moppable>(493375141, Moppable.OnRefreshUserMenuDelegate);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_mop_dirtywater_kanim") };
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Moppable.OnSpawn", base.gameObject, new Extents(Grid.PosToCell(this), new CellOffset[]
		{
			new CellOffset(0, 0)
		}), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
		this.Refresh();
		base.Subscribe<Moppable>(-1432940121, Moppable.OnReachableChangedDelegate);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		SimAndRenderScheduler.instance.Remove(this);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string text = "icon_cancel";
		string text2 = UI.USERMENUACTIONS.CANCELMOP.NAME;
		global::System.Action action = new global::System.Action(this.OnCancel);
		string text3 = UI.USERMENUACTIONS.CANCELMOP.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(false);
		base.gameObject.Trigger(2127324410, null);
	}

	protected override void OnStartWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Add(this, false);
		this.Refresh();
		this.MopTick();
	}

	protected override void OnStopWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public void Sim1000ms(float dt)
	{
		if (this.amountMopped > 0f)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, GameUtil.GetFormattedMass(-this.amountMopped, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), base.transform, 1.5f, false);
			this.amountMopped = 0f;
		}
	}

	public void Sim200ms(float dt)
	{
		if (base.worker != null)
		{
			this.Refresh();
			this.MopTick();
		}
	}

	private void OnCellMopped(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		if (this == null)
		{
			return;
		}
		if (mass_cb_info.mass > 0f)
		{
			this.amountMopped += mass_cb_info.mass;
			int num = Grid.PosToCell(this);
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(ElementLoader.elements[(int)mass_cb_info.elemIdx], mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount, Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
			substanceChunk.transform.SetPosition(substanceChunk.transform.GetPosition() + new Vector3((global::UnityEngine.Random.value - 0.5f) * 0.5f, 0f, 0f));
		}
	}

	public static void MopCell(int cell, float amount, Action<Sim.MassConsumedCallback, object> cb)
	{
		if (Grid.Element[cell].IsLiquid)
		{
			int num = -1;
			if (cb != null)
			{
				num = Game.Instance.massConsumedCallbackManager.Add(cb, null, "Moppable").index;
			}
			SimMessages.ConsumeMass(cell, Grid.Element[cell].id, amount, 1, num);
		}
	}

	private void MopTick()
	{
		int num = Grid.PosToCell(this);
		for (int i = 0; i < this.offsets.Length; i++)
		{
			int num2 = Grid.OffsetCell(num, this.offsets[i]);
			if (Grid.Element[num2].IsLiquid)
			{
				Moppable.MopCell(num2, this.amountMoppedPerTick, new Action<Sim.MassConsumedCallback, object>(this.OnCellMopped));
			}
		}
	}

	private bool IsThereLiquid()
	{
		int num = Grid.PosToCell(this);
		bool flag = false;
		for (int i = 0; i < this.offsets.Length; i++)
		{
			int num2 = Grid.OffsetCell(num, this.offsets[i]);
			if (Grid.Element[num2].IsLiquid && Grid.Mass[num2] <= MopTool.maxMopAmt)
			{
				flag = true;
			}
		}
		return flag;
	}

	private void Refresh()
	{
		if (!this.IsThereLiquid())
		{
			if (!this.destroyHandle.IsValid)
			{
				this.destroyHandle = GameScheduler.Instance.Schedule("DestroyMoppable", 1f, delegate(object moppable)
				{
					this.TryDestroy();
				}, this, null);
			}
		}
		else if (this.destroyHandle.IsValid)
		{
			this.destroyHandle.ClearScheduler();
		}
	}

	private void OnLiquidChanged(object data)
	{
		this.Refresh();
	}

	private void TryDestroy()
	{
		if (this != null)
		{
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	private void OnReachableChanged(object data)
	{
		if (this.childRenderer != null)
		{
			Material material = this.childRenderer.material;
			bool flag = (bool)data;
			if (material.color == Game.Instance.uiColours.Dig.invalidLocation)
			{
				return;
			}
			KSelectable component = base.GetComponent<KSelectable>();
			if (flag)
			{
				material.color = Game.Instance.uiColours.Dig.validLocation;
				component.RemoveStatusItem(Db.Get().BuildingStatusItems.MopUnreachable, false);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MopUnreachable, this);
				GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate(object obj)
				{
					Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion);
				}, null, null);
				material.color = Game.Instance.uiColours.Dig.unreachable;
			}
		}
	}

	[MyCmpReq]
	private KSelectable Selectable;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	public float amountMoppedPerTick = 1000f;

	private HandleVector<int>.Handle partitionerEntry;

	private SchedulerHandle destroyHandle;

	private float amountMopped;

	private MeshRenderer childRenderer;

	private CellOffset[] offsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	private static readonly EventSystem.IntraObjectHandler<Moppable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Moppable>(delegate(Moppable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Moppable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Moppable>(delegate(Moppable component, object data)
	{
		component.OnReachableChanged(data);
	});
}
