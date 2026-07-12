using System;
using System.Collections.Generic;
using FoodRehydrator;
using Klei;
using KSerialization;
using UnityEngine;

internal class DehydratedFoodPackage : Workable
{
	public override BuildingFacade GetBuildingFacade()
	{
		DebugUtil.DevAssert(this.rehydrator == base.gameObject.GetComponent<Pickupable>().storage.gameObject, "invalid rehydrator reference", null);
		return this.rehydrator.GetComponent<BuildingFacade>();
	}

	public override KAnimControllerBase GetAnimController()
	{
		DebugUtil.DevAssert(this.rehydrator == base.gameObject.GetComponent<Pickupable>().storage.gameObject, "invalid rehydrator reference", null);
		return this.rehydrator.GetComponent<KAnimControllerBase>();
	}

	public void StoredInRehydrator(GameObject rehydrator)
	{
		this.rehydrator = rehydrator;
		this.storage.allowItemRemoval = true;
		foreach (GameObject gameObject in this.storage.items)
		{
			gameObject.RemoveTag(GameTags.StoredPrivate);
		}
		this.SwapContentsPickupBehavior(true);
	}

	public void RemovedFromRehydrator()
	{
		this.rehydrator = null;
		this.storage.allowItemRemoval = false;
		foreach (GameObject gameObject in this.storage.items)
		{
			gameObject.AddTag(GameTags.StoredPrivate);
		}
		this.SwapContentsPickupBehavior(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.storage.items.Count < 1)
		{
			this.storage.ConsumeAllIgnoringDisease(this.FoodTag);
			int num = Grid.PosToCell(this);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.FoodTag), Grid.CellToPosCBC(num, Grid.SceneLayer.Creatures), Grid.SceneLayer.Creatures, null, 0);
			gameObject.SetActive(true);
			gameObject.AddTag(GameTags.Dehydrated);
			gameObject.GetComponent<Edible>().Calories = 1000000f;
			this.storage.Store(gameObject, false, false, true, false);
			if (base.GetComponent<Pickupable>().storage != null && base.GetComponent<Pickupable>().storage.GetComponent<AccessabilityManager>() != null)
			{
				this.StoredInRehydrator(base.GetComponent<Pickupable>().storage.gameObject);
			}
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (this.rehydrator != null)
		{
			DehydratedManager component = this.rehydrator.GetComponent<DehydratedManager>();
			if (component != null)
			{
				component.SetFabricatedFoodSymbol(this.FoodTag);
			}
			this.rehydrator.GetComponent<AccessabilityManager>().SetActiveWorkable(this);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.SwapContentsPickupBehavior(false);
		GameObject gameObject = ((this.storage.items.Count > 0) ? this.storage.items[0] : null);
		this.storage.Transfer(worker.GetComponent<Storage>(), false, false);
		DehydratedManager component = base.GetComponent<Pickupable>().storage.GetComponent<DehydratedManager>();
		this.rehydrator.GetComponent<AccessabilityManager>().SetActiveWorkable(null);
		SimUtil.DiseaseInfo diseaseInfo = component.ConsumeResourcesFromRehydratingPackaged(base.gameObject);
		gameObject.GetComponent<PrimaryElement>().AddDisease(diseaseInfo.idx, diseaseInfo.count, "rehydrating");
		Pickupable.PickupableStartWorkInfo pickupableStartWorkInfo = (Pickupable.PickupableStartWorkInfo)worker.startWorkInfo;
		if (pickupableStartWorkInfo != null && pickupableStartWorkInfo.setResultCb != null && gameObject != null)
		{
			pickupableStartWorkInfo.setResultCb(gameObject);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (this.rehydrator != null)
		{
			this.rehydrator.GetComponent<AccessabilityManager>().SetActiveWorkable(null);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void Swap<Type>(ref Type a, ref Type b)
	{
		Type type = a;
		a = b;
		b = type;
	}

	private void SwapContentsPickupBehavior(bool inPackage)
	{
		DebugUtil.Assert(this.storage.items.Count <= 1, "Packets are required to either be empty or contain only 1 item!");
		if (this.storage.items.Count == 1)
		{
			GameObject gameObject = this.storage.items[0];
			Pickupable component = gameObject.GetComponent<Pickupable>();
			this.Swap<bool>(ref component.absorbable, ref this.containedObjectBehaviors.pickup_absorbable);
			this.Swap<Func<Pickupable, float, Pickupable>>(ref component.OnTake, ref this.containedObjectBehaviors.pickupable_ontake);
			this.Swap<Func<Pickupable, bool>>(ref component.CanAbsorb, ref this.containedObjectBehaviors.pickupable_canabsorb);
			component.targetWorkable = (inPackage ? this : component);
			CellOffset[] offsets = component.GetOffsets();
			component.SetOffsets(this.containedObjectBehaviors.pickupable_offset);
			this.Swap<CellOffset[]>(ref offsets, ref this.containedObjectBehaviors.pickupable_offset);
			if (!inPackage)
			{
				component.allowedChoreTypes = null;
				gameObject.gameObject.RemoveTag(GameTags.Dehydrated);
				gameObject.gameObject.AddTag(GameTags.Rehydrated);
				gameObject.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.RehydratedFood, null);
				return;
			}
			component.allowedChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Eat };
		}
	}

	[Serialize]
	public Tag FoodTag;

	[MyCmpReq]
	private Storage storage;

	private GameObject rehydrator;

	private DehydratedFoodPackage.ContainedObjectBehaviorOverrides containedObjectBehaviors = new DehydratedFoodPackage.ContainedObjectBehaviorOverrides();

	protected class ContainedObjectBehaviorOverrides
	{
		public bool pickup_absorbable;

		public Func<Pickupable, float, Pickupable> pickupable_ontake = delegate(Pickupable p, float _)
		{
			p.storage.Remove(p.gameObject, true);
			return p;
		};

		public Func<Pickupable, bool> pickupable_canabsorb = (Pickupable _) => false;

		public CellOffset[] pickupable_offset = new CellOffset[]
		{
			default(CellOffset),
			new CellOffset(0, -1)
		};
	}
}
