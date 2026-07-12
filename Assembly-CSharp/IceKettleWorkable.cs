using System;
using System.Collections.Generic;
using UnityEngine;

public class IceKettleWorkable : Workable
{
	public MeterController meter { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_arrow", "meter_scale" });
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_icemelter_kettle_kanim") };
		this.synchronizeAnims = true;
		base.SetOffsets(new CellOffset[] { this.workCellOffset });
		base.SetWorkTime(5f);
		this.resetProgressOnStop = true;
		this.showProgressBar = false;
		this.storage.onDestroyItemsDropped = new Action<List<GameObject>>(this.RestoreStoredItemsInteractions);
		this.handler = base.Subscribe(-1697596308, new Action<object>(this.OnStroageChanged));
	}

	protected override void OnSpawn()
	{
		this.AdjustStoredItemsPositionsAndWorkable();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		Pickupable.PickupableStartWorkInfo pickupableStartWorkInfo = (Pickupable.PickupableStartWorkInfo)worker.startWorkInfo;
		this.meter.gameObject.SetActive(true);
		PrimaryElement component = pickupableStartWorkInfo.originalPickupable.GetComponent<PrimaryElement>();
		this.meter.SetSymbolTint(new KAnimHashedString("meter_fill"), component.Element.substance.colour);
		this.meter.SetSymbolTint(new KAnimHashedString("water1"), component.Element.substance.colour);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		float num = (this.workTime - base.WorkTimeRemaining) / this.workTime;
		this.meter.SetPositionPercent(Mathf.Clamp01(num));
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = worker.GetComponent<Storage>();
		Pickupable.PickupableStartWorkInfo pickupableStartWorkInfo = (Pickupable.PickupableStartWorkInfo)worker.startWorkInfo;
		if (pickupableStartWorkInfo.amount > 0f)
		{
			this.storage.TransferMass(component, pickupableStartWorkInfo.originalPickupable.KPrefabID.PrefabID(), pickupableStartWorkInfo.amount, false, false, false);
		}
		base.OnCompleteWork(worker);
		foreach (GameObject gameObject in component.items)
		{
			if (gameObject.HasTag(GameTags.Liquid))
			{
				Pickupable component2 = gameObject.GetComponent<Pickupable>();
				this.RestorePickupableInteractions(component2);
			}
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.meter.gameObject.SetActive(false);
	}

	private void OnStroageChanged(object obj)
	{
		this.AdjustStoredItemsPositionsAndWorkable();
	}

	private void AdjustStoredItemsPositionsAndWorkable()
	{
		int num = Grid.PosToCell(this);
		Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(num, new CellOffset(0, 0)), Grid.SceneLayer.Ore);
		foreach (GameObject gameObject in this.storage.items)
		{
			Pickupable component = gameObject.GetComponent<Pickupable>();
			component.transform.SetPosition(vector);
			component.UpdateCachedCell(num);
			this.OverridePickupableInteractions(component);
		}
	}

	private void OverridePickupableInteractions(Pickupable pickupable)
	{
		pickupable.AddTag(GameTags.LiquidSource);
		pickupable.targetWorkable = this;
		pickupable.SetOffsets(new CellOffset[] { this.workCellOffset });
	}

	private void RestorePickupableInteractions(Pickupable pickupable)
	{
		pickupable.RemoveTag(GameTags.LiquidSource);
		pickupable.targetWorkable = pickupable;
		pickupable.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	private void RestoreStoredItemsInteractions(List<GameObject> specificItems = null)
	{
		specificItems = ((specificItems == null) ? this.storage.items : specificItems);
		foreach (GameObject gameObject in specificItems)
		{
			Pickupable component = gameObject.GetComponent<Pickupable>();
			this.RestorePickupableInteractions(component);
		}
	}

	protected override void OnCleanUp()
	{
		if (base.worker != null)
		{
			ChoreDriver component = base.worker.GetComponent<ChoreDriver>();
			base.worker.StopWork();
			component.StopChore();
		}
		this.RestoreStoredItemsInteractions(null);
		base.Unsubscribe(this.handler);
		base.OnCleanUp();
	}

	public Storage storage;

	private int handler;

	public CellOffset workCellOffset = new CellOffset(0, 0);
}
