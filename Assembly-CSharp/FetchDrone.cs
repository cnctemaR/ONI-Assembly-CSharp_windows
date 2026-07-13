using System;
using UnityEngine;

public class FetchDrone : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		ChoreGroup[] array = new ChoreGroup[]
		{
			Db.Get().ChoreGroups.Build,
			Db.Get().ChoreGroups.Basekeeping,
			Db.Get().ChoreGroups.Cook,
			Db.Get().ChoreGroups.Art,
			Db.Get().ChoreGroups.Dig,
			Db.Get().ChoreGroups.Research,
			Db.Get().ChoreGroups.Farming,
			Db.Get().ChoreGroups.Ranching,
			Db.Get().ChoreGroups.MachineOperating,
			Db.Get().ChoreGroups.MedicalAid,
			Db.Get().ChoreGroups.Combat,
			Db.Get().ChoreGroups.LifeSupport,
			Db.Get().ChoreGroups.Recreation,
			Db.Get().ChoreGroups.Toggle,
			Db.Get().ChoreGroups.Rocketry
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				this.choreConsumer.SetPermittedByUser(array[i], false);
			}
		}
		foreach (Storage storage in base.GetComponents<Storage>())
		{
			if (storage.storageID != GameTags.ChargedPortableBattery)
			{
				this.pickupableStorage = storage;
				break;
			}
		}
		this.animController = base.GetComponent<KBatchedAnimController>();
		this.pickupableStorage.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		base.Subscribe(-1582839653, new Action<object>(this.OnTagsChanged));
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(-1697596308);
		base.Unsubscribe(-1582839653);
		base.OnCleanUp();
	}

	private void OnTagsChanged(object data)
	{
		TagChangedEventData tagChangedEventData = (TagChangedEventData)data;
		if (tagChangedEventData.added && tagChangedEventData.tag == GameTags.Creatures.Die)
		{
			Brain component = base.GetComponent<Brain>();
			if (component != null && !component.IsRunning())
			{
				component.Resume("death");
			}
		}
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		this.RemoveTracker(gameObject);
		this.ShowPickupSymbol(gameObject);
	}

	private void ShowPickupSymbol(GameObject pickupable)
	{
		bool flag = this.pickupableStorage.items.Contains(pickupable);
		if (flag)
		{
			this.AddAnimTracker(pickupable);
		}
		this.animController.SetSymbolVisiblity(FetchDrone.BOTTOM, !flag);
		this.animController.SetSymbolVisiblity(FetchDrone.BOTTOM_CARRY, flag);
	}

	private void AddAnimTracker(GameObject go)
	{
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (component == null)
		{
			return;
		}
		if (component.AnimFiles != null && component.AnimFiles.Length != 0 && component.AnimFiles[0] != null && component.GetComponent<Pickupable>().trackOnPickup)
		{
			KBatchedAnimTracker kbatchedAnimTracker = go.GetComponent<KBatchedAnimTracker>();
			if (kbatchedAnimTracker != null && kbatchedAnimTracker.controller == this.animController)
			{
				return;
			}
			kbatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.useTargetPoint = false;
			kbatchedAnimTracker.fadeOut = false;
			kbatchedAnimTracker.symbol = ((go.GetComponent<Brain>() != null) ? new HashedString("snapTo_pivot") : new HashedString("snapTo_thing"));
			kbatchedAnimTracker.forceAlwaysVisible = true;
		}
	}

	private void RemoveTracker(GameObject go)
	{
		KBatchedAnimTracker kbatchedAnimTracker = ((go != null) ? go.GetComponent<KBatchedAnimTracker>() : null);
		if (kbatchedAnimTracker != null && kbatchedAnimTracker.controller == this.animController)
		{
			global::UnityEngine.Object.Destroy(kbatchedAnimTracker);
		}
	}

	private static string BOTTOM = "bottom";

	private static string BOTTOM_CARRY = "bottom_carry";

	private KBatchedAnimController animController;

	private Storage pickupableStorage;

	[MyCmpAdd]
	private ChoreConsumer choreConsumer;
}
