using System;
using System.Collections.Generic;
using UnityEngine;

public class BionicMinionStorageExtension : KMonoBehaviour, StoredMinionIdentity.IStoredMinionExtension
{
	public void AddStoredMinionGameObjectRequirements(GameObject storedMinionGameObject)
	{
		Storage[] components = storedMinionGameObject.GetComponents<Storage>();
		using (List<Tag>.Enumerator enumerator = BionicMinionStorageExtension.StoragesTypesToTransfer.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Tag inventoryType = enumerator.Current;
				if (components == null || !(components.FindFirst<Storage>((Storage s) => s.storageID == inventoryType) != null))
				{
					Storage storage = storedMinionGameObject.AddComponent<Storage>();
					storage.allowItemRemoval = false;
					storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
					storage.storageID = inventoryType;
				}
			}
		}
	}

	void StoredMinionIdentity.IStoredMinionExtension.PullFrom(StoredMinionIdentity source)
	{
		Storage[] components = source.GetComponents<Storage>();
		Storage[] components2 = base.GetComponents<Storage>();
		foreach (Storage storage in components)
		{
			bool flag = false;
			foreach (Storage storage2 in components2)
			{
				if (storage2.storageID == storage.storageID)
				{
					storage.Transfer(storage2, false, true);
					flag = true;
					break;
				}
			}
			DebugUtil.DevAssert(flag, "Missmatched storages on BionicMinionStorageExtension", null);
		}
	}

	void StoredMinionIdentity.IStoredMinionExtension.PushTo(StoredMinionIdentity destination)
	{
		GameObject gameObject = destination.gameObject;
		this.AddStoredMinionGameObjectRequirements(gameObject);
		Storage[] components = base.GetComponents<Storage>();
		Storage[] components2 = gameObject.GetComponents<Storage>();
		foreach (Tag tag in BionicMinionStorageExtension.StoragesTypesToTransfer)
		{
			Storage storage = null;
			Storage storage2 = null;
			foreach (Storage storage3 in components)
			{
				if (storage3.storageID == tag)
				{
					storage = storage3;
					break;
				}
			}
			foreach (Storage storage4 in components2)
			{
				if (storage4.storageID == tag)
				{
					storage2 = storage4;
					break;
				}
			}
			storage.Transfer(storage2, true, true);
		}
	}

	private static readonly List<Tag> StoragesTypesToTransfer = new List<Tag>
	{
		GameTags.StoragesIds.BionicBatteryStorage,
		GameTags.StoragesIds.BionicUpgradeStorage,
		GameTags.StoragesIds.BionicOxygenTankStorage
	};
}
