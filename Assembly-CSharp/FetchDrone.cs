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
			this.choreConsumer.SetPermittedByUser(array[i], false);
		}
		foreach (Storage storage in base.GetComponents<Storage>())
		{
			if (storage.storageID != GameTags.ChargedPortableBattery)
			{
				this.pickupableStorage = storage;
				break;
			}
		}
		this.SetupPickupable();
		this.pickupableStorage.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
	}

	public void SetupPickupable()
	{
		this.animController = base.GetComponent<KBatchedAnimController>();
		GameObject gameObject = Util.NewGameObject(base.gameObject, "pickupableSymbol");
		gameObject.SetActive(false);
		bool flag;
		Vector3 vector = this.animController.GetSymbolTransform(FetchDrone.HASH_SNAPTO_THING, out flag).GetColumn(3);
		vector.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
		gameObject.transform.SetPosition(vector);
		this.pickupableKBAC = gameObject.AddComponent<KBatchedAnimController>();
		this.pickupableKBAC.AnimFiles = new KAnimFile[] { Assets.GetAnim("algae_kanim") };
		this.pickupableKBAC.sceneLayer = Grid.SceneLayer.BuildingUse;
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = FetchDrone.HASH_SNAPTO_THING;
		kbatchedAnimTracker.offset = Vector3.zero;
	}

	private void OnStorageChanged(object data)
	{
		this.ShowPickupSymbol(!this.pickupableStorage.IsEmpty());
	}

	private void ShowPickupSymbol(bool show)
	{
		this.pickupableKBAC.gameObject.SetActive(show);
		if (show)
		{
			Pickupable component = this.pickupableStorage.items[0].GetComponent<Pickupable>();
			if (component != null)
			{
				KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
				if (component.GetComponent<MinionIdentity>())
				{
					this.AddAnimTracker(component2.gameObject);
				}
				else
				{
					this.pickupableKBAC.SwapAnims(component2.AnimFiles);
					this.pickupableKBAC.Play(component2.currentAnim, KAnim.PlayMode.Loop, 1f, 0f);
				}
			}
		}
		this.animController.SetSymbolVisiblity(FetchDrone.BOTTOM, !show);
		this.animController.SetSymbolVisiblity(FetchDrone.BOTTOM_CARRY, show);
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
			KBatchedAnimTracker kbatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.useTargetPoint = false;
			kbatchedAnimTracker.fadeOut = false;
			kbatchedAnimTracker.symbol = new HashedString("snapTo_chest");
			kbatchedAnimTracker.forceAlwaysVisible = true;
		}
	}

	private static string HASH_SNAPTO_THING = "snapTo_thing";

	private static string BOTTOM = "bottom";

	private static string BOTTOM_CARRY = "bottom_carry";

	private KBatchedAnimController pickupableKBAC;

	private KBatchedAnimController animController;

	private Storage pickupableStorage;

	[MyCmpAdd]
	private ChoreConsumer choreConsumer;
}
