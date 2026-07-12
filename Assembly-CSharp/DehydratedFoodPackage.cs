using System;
using System.Collections.Generic;
using System.Linq;
using FoodRehydrator;
using KSerialization;
using UnityEngine;

internal class DehydratedFoodPackage : Workable
{
	public override BuildingFacade GetBuildingFacade()
	{
		DebugUtil.DevAssert(this.rehydrator == null || this.rehydrator == base.gameObject.GetComponent<Pickupable>().storage.gameObject, "invalid rehydrator reference", null);
		if (!(this.rehydrator != null))
		{
			return null;
		}
		return this.rehydrator.GetComponent<BuildingFacade>();
	}

	public override KAnimControllerBase GetAnimController()
	{
		DebugUtil.DevAssert(this.rehydrator == null || this.rehydrator == base.gameObject.GetComponent<Pickupable>().storage.gameObject, "invalid rehydrator reference", null);
		if (!(this.rehydrator != null))
		{
			return null;
		}
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
	}

	public void RemovedFromRehydrator()
	{
		this.rehydrator = null;
		this.storage.allowItemRemoval = false;
		foreach (GameObject gameObject in this.storage.items)
		{
			gameObject.AddTag(GameTags.StoredPrivate);
		}
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
			gameObject.GetComponent<Edible>().Calories = 1000000f;
			this.storage.Store(gameObject, false, false, true, false);
			if (base.GetComponent<Pickupable>().storage != null && base.GetComponent<Pickupable>().storage.GetComponent<AccessabilityManager>() != null)
			{
				this.StoredInRehydrator(base.GetComponent<Pickupable>().storage.gameObject);
			}
		}
		base.Subscribe(-1697596308, new Action<object>(this.StorageChangeHandler));
		this.DehydrateItem(this.storage.items.ElementAtOrDefault<GameObject>(0));
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
		if (this.storage.items.Count != 1)
		{
			DebugUtil.DevAssert(false, "OnCompleteWork invalid contents of package", null);
			return;
		}
		GameObject gameObject = this.storage.items[0];
		this.storage.Transfer(worker.GetComponent<Storage>(), false, false);
		DebugUtil.DevAssert(this.rehydrator == base.GetComponent<Pickupable>().storage.gameObject, "OnCompleteWork rehydrator mismatch", null);
		DehydratedManager component = this.rehydrator.GetComponent<DehydratedManager>();
		this.rehydrator.GetComponent<AccessabilityManager>().SetActiveWorkable(null);
		component.ConsumeResourcesForRehydration(base.gameObject, gameObject);
		this.rehydrator = null;
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

	private void StorageChangeHandler(object obj)
	{
		GameObject gameObject = (GameObject)obj;
		DebugUtil.DevAssert(!this.storage.items.Contains(gameObject), "Attempting to add item to a dehydrated food package which is not allowed", null);
		this.RehydrateItem(gameObject);
	}

	public void DehydrateItem(GameObject item)
	{
		DebugUtil.DevAssert(item != null, "Attempting to dehydrate contents of an empty packet", null);
		if (this.storage.items.Count != 1 || item == null)
		{
			DebugUtil.DevAssert(false, "DehydrateItem called, incorrect content", null);
			return;
		}
		item.AddTag(GameTags.Dehydrated);
		Pickupable component = item.GetComponent<Pickupable>();
		this.SwapPickupablesBehaviors(component);
		component.allowedChoreTypes = new List<ChoreType> { Db.Get().ChoreTypes.Eat };
		component.targetWorkable = this;
	}

	public void RehydrateItem(GameObject item)
	{
		if (this.storage.items.Count != 0)
		{
			DebugUtil.DevAssert(false, "RehydrateItem called, incorrect storage content", null);
			return;
		}
		item.RemoveTag(GameTags.Dehydrated);
		item.AddTag(GameTags.Rehydrated);
		item.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.RehydratedFood, null);
		Pickupable component = item.GetComponent<Pickupable>();
		this.SwapPickupablesBehaviors(component);
		component.allowedChoreTypes = null;
		component.targetWorkable = component;
	}

	private void Swap<Type>(ref Type a, ref Type b)
	{
		Type type = a;
		a = b;
		b = type;
	}

	private void SwapPickupablesBehaviors(Pickupable pickup)
	{
		this.Swap<bool>(ref pickup.absorbable, ref this.containedObjectBehaviors.pickup_absorbable);
		this.Swap<Func<Pickupable, float, Pickupable>>(ref pickup.OnTake, ref this.containedObjectBehaviors.pickupable_ontake);
		this.Swap<Func<Pickupable, bool>>(ref pickup.CanAbsorb, ref this.containedObjectBehaviors.pickupable_canabsorb);
		CellOffset[] offsets = pickup.GetOffsets();
		pickup.SetOffsets(this.containedObjectBehaviors.pickupable_offset);
		this.Swap<CellOffset[]>(ref offsets, ref this.containedObjectBehaviors.pickupable_offset);
	}

	[Serialize]
	public Tag FoodTag;

	[MyCmpReq]
	private Storage storage;

	private GameObject rehydrator;

	private DehydratedFoodPackage.OverriddenPickupableProperties containedObjectBehaviors = new DehydratedFoodPackage.OverriddenPickupableProperties();

	private class OverriddenPickupableProperties
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
