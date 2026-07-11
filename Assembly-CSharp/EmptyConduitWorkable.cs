using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class EmptyConduitWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		base.SetWorkTime(float.PositiveInfinity);
		this.faceTargetWhenWorking = true;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		base.Subscribe(2127324410, new Action<object>(this.OnEmptyConduitCancelled));
		if (EmptyConduitWorkable.emptyLiquidConduitStatusItem == null)
		{
			EmptyConduitWorkable.emptyLiquidConduitStatusItem = new StatusItem("EmptyLiquidConduit", BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, 66);
			EmptyConduitWorkable.emptyGasConduitStatusItem = new StatusItem("EmptyGasConduit", BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, 130);
		}
		this.requiredRolePerk = RoleManager.rolePerks.CanDoPlumbing.id;
		this.shouldShowRolePerkStatusItem = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.elapsedTime != -1f)
		{
			this.MarkForEmptying();
		}
	}

	public void MarkForEmptying()
	{
		if (this.chore == null)
		{
			StatusItem statusItem = this.GetStatusItem();
			KSelectable component = base.GetComponent<KSelectable>();
			component.ToggleStatusItem(statusItem, true, null);
			this.CreateWorkChore();
		}
	}

	private void CancelEmptying()
	{
		this.CleanUpVisualization();
		if (this.chore != null)
		{
			this.chore.Cancel("Cancel");
			this.chore = null;
			this.shouldShowRolePerkStatusItem = false;
			this.UpdateStatusItem(null);
		}
	}

	private void CleanUpVisualization()
	{
		StatusItem statusItem = this.GetStatusItem();
		KSelectable component = base.GetComponent<KSelectable>();
		if (component != null)
		{
			component.ToggleStatusItem(statusItem, false, null);
		}
		this.elapsedTime = -1f;
		if (this.chore != null)
		{
			base.GetComponent<Prioritizable>().RemoveRef();
		}
	}

	protected override void OnCleanUp()
	{
		this.CancelEmptying();
		base.OnCleanUp();
	}

	private ConduitFlow GetFlowManager()
	{
		return (this.conduit.type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	private void OnEmptyConduitCancelled(object data)
	{
		this.CancelEmptying();
	}

	private StatusItem GetStatusItem()
	{
		ConduitType type = this.conduit.type;
		StatusItem statusItem;
		if (type != ConduitType.Gas)
		{
			if (type != ConduitType.Liquid)
			{
				throw new ArgumentException();
			}
			statusItem = EmptyConduitWorkable.emptyLiquidConduitStatusItem;
		}
		else
		{
			statusItem = EmptyConduitWorkable.emptyGasConduitStatusItem;
		}
		return statusItem;
	}

	private void CreateWorkChore()
	{
		base.GetComponent<Prioritizable>().AddRef();
		this.chore = new WorkChore<EmptyConduitWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		this.chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanDoPlumbing.id);
		this.elapsedTime = 0f;
		this.emptiedPipe = false;
		this.shouldShowRolePerkStatusItem = true;
		this.UpdateStatusItem(null);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.elapsedTime == -1f)
		{
			return true;
		}
		bool flag = false;
		this.elapsedTime += dt;
		if (!this.emptiedPipe)
		{
			if (this.elapsedTime > 4f)
			{
				this.EmptyPipeContents();
				this.emptiedPipe = true;
				this.elapsedTime = 0f;
			}
		}
		else if (this.elapsedTime > 2f)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (this.GetFlowManager().GetContents(num).mass > 0f)
			{
				this.elapsedTime = 0f;
				this.emptiedPipe = false;
			}
			else
			{
				this.CleanUpVisualization();
				this.chore = null;
				flag = true;
				this.shouldShowRolePerkStatusItem = false;
				this.UpdateStatusItem(null);
			}
		}
		return flag;
	}

	private void EmptyPipeContents()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		ConduitFlow.ConduitContents conduitContents = this.GetFlowManager().RemoveElement(num, float.PositiveInfinity);
		this.elapsedTime = 0f;
		if (conduitContents.mass > 0f && conduitContents.element != SimHashes.Vacuum)
		{
			ConduitType type = this.conduit.type;
			IChunkManager chunkManager;
			if (type != ConduitType.Gas)
			{
				if (type != ConduitType.Liquid)
				{
					throw new ArgumentException();
				}
				chunkManager = LiquidSourceManager.Instance;
			}
			else
			{
				chunkManager = GasSourceManager.Instance;
			}
			chunkManager.CreateChunk(conduitContents.element, conduitContents.mass, conduitContents.temperature, conduitContents.diseaseIdx, conduitContents.diseaseCount, Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
		}
	}

	public override float GetPercentComplete()
	{
		return Mathf.Clamp01(this.elapsedTime / 4f);
	}

	[MyCmpReq]
	private Conduit conduit;

	private static StatusItem emptyLiquidConduitStatusItem;

	private static StatusItem emptyGasConduitStatusItem;

	private Chore chore;

	private const float RECHECK_PIPE_INTERVAL = 2f;

	private const float TIME_TO_EMPTY_PIPE = 4f;

	private const float NO_EMPTY_SCHEDULED = -1f;

	[Serialize]
	private float elapsedTime = -1f;

	private bool emptiedPipe = true;
}
