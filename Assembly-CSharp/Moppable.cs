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
		new WorkChore<Moppable>(Db.Get().ChoreTypes.Mop, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
		base.SetWorkTime(float.PositiveInfinity);
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForMop, null);
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_mop_dirtywater_kanim") };
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Moppable.OnSpawn", base.gameObject, new Extents(Grid.PosToCell(this), this.offsets), GameScenePartitioner.Instance.liquidChangedMask.mask, new Action<object>(this.OnLiquidChanged));
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
	}

	protected override void OnStopWork(Worker worker)
	{
		this.popfxHandle.Clear();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.popfxHandle.Clear();
	}

	private void OnPopFX(object data)
	{
		if (this.amountMopped > 0f)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, GameUtil.GetFormattedMass(-this.amountMopped, GameUtil.TimeSlice.None, true, "{0:0.#}"), this.transform, 1.5f, false);
			this.amountMopped = 0f;
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		int num = Grid.PosToCell(this);
		for (int i = 0; i < this.offsets.Length; i++)
		{
			int num2 = Grid.OffsetCell(num, this.offsets[i]);
			if (Grid.Element[num2].IsLiquid)
			{
				SimMessages.AddRemoveSubstance(num2, Grid.Element[num2].id, CellEventLogger.Instance.Mop, -20f * dt, Grid.Temperature[num2], -1);
				this.amountMopped += Mathf.Min(Grid.Cell[num2].mass, 20f * dt);
			}
		}
		return false;
	}

	private bool IsThereLiquid()
	{
		int num = Grid.PosToCell(this);
		bool flag = false;
		for (int i = 0; i < this.offsets.Length; i++)
		{
			int num2 = Grid.OffsetCell(num, this.offsets[i]);
			if (Grid.Element[num2].IsLiquid)
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
			this.destroyHandle.Clear();
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
		this.popfxHandle.Clear();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
	}

	public static void MopCell(int cell, Worker worker)
	{
		if (Grid.Element[cell].IsLiquid)
		{
			SimMessages.AddRemoveSubstance(cell, Grid.Element[cell].id, CellEventLogger.Instance.Mop, -Grid.Cell[cell].mass, Grid.Temperature[cell], -1);
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
