using System;
using STRINGS;
using UnityEngine;

public class Moppable : Workable
{
	private Moppable()
	{
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
		this.attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
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
		new WorkChore<Moppable>(Db.Get().ChoreTypes.Mop, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true);
		base.SetWorkTime(float.PositiveInfinity);
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForMop, null);
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_mop_dirtywater_kanim") };
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Moppable.OnSpawn", base.gameObject, new Extents(Grid.PosToCell(this), new CellOffset[]
		{
			new CellOffset(0, 0)
		}), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
		this.Refresh();
		this.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = UI.USERMENUACTIONS.CANCELMOP.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("icon_cancel", UI.USERMENUACTIONS.CANCELMOP.NAME, new global::System.Action(this.OnCancel), global::Action.NumActions, null, null, null, text, true), 1f);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(false);
		base.gameObject.Trigger(2127324410, null);
	}

	protected override void OnStartWork(Worker worker)
	{
		this.popfxHandle = GameScheduler.Instance.SchedulePeriodic("MoppablePopFX", 1f, new Action<object>(this.OnPopFX), null, null, 0f, null);
		this.Refresh();
		this.MopTick();
	}

	protected override void OnStopWork(Worker worker)
	{
		this.popfxHandle.ClearScheduler();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.popfxHandle.ClearScheduler();
	}

	private void OnPopFX(object data)
	{
		if (this.amountMopped > 0f)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, GameUtil.GetFormattedMass(-this.amountMopped, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), this.transform, 1.5f, false);
			this.amountMopped = 0f;
		}
	}

	private void SimUpdate(float dt)
	{
		if (base.worker != null)
		{
			this.Refresh();
			this.MopTick();
		}
	}

	private void OnCellMopped(object data)
	{
		if (this == null)
		{
			return;
		}
		Sim.MassConsumptionCallback massConsumptionCallback = (Sim.MassConsumptionCallback)data;
		if (massConsumptionCallback.mass > 0f)
		{
			this.amountMopped += massConsumptionCallback.mass;
			int num = Grid.PosToCell(this);
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(ElementLoader.elements[(int)massConsumptionCallback.removedElemIdx], massConsumptionCallback.mass, massConsumptionCallback.temperature, massConsumptionCallback.diseaseIdx, massConsumptionCallback.diseaseCount, Grid.CellToPosCCC(num, Grid.SceneLayer.Use));
			substanceChunk.transform.Translate((global::UnityEngine.Random.value - 0.5f) * 0.5f, 0f, 0f);
		}
	}

	public static void MopCell(int cell, float amount, Action<object> cb)
	{
		if (Grid.Element[cell].IsLiquid)
		{
			int num = -1;
			if (cb != null)
			{
				num = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(cb)).index;
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
				Moppable.MopCell(num2, this.amountMoppedPerTick, new Action<object>(this.OnCellMopped));
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
			if (Grid.Element[num2].IsLiquid && Grid.Cell[num2].mass <= MopTool.maxMopAmt)
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
		this.popfxHandle.ClearScheduler();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
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
			if (flag)
			{
				material.color = Game.Instance.uiColours.Dig.validLocation;
				this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, false);
			}
			else
			{
				this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, this);
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

	[MyCmpAdd]
	private UserMenu userMenu;

	public float amountMoppedPerTick = 1000f;

	private GameScenePartitionerEntry partitionerEntry;

	private SchedulerHandle destroyHandle;

	private float amountMopped;

	private SchedulerHandle popfxHandle;

	private MeshRenderer childRenderer;

	private CellOffset[] offsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};
}
