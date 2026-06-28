using System;
using UnityEngine;

public class Baggable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Pickupable component = base.GetComponent<Pickupable>();
		component.workAnims = new HashedString[]
		{
			new HashedString("capture"),
			new HashedString("pickup")
		};
		component.overrideAnims = new KAnimFile[] { this.animOverride };
		component.trackOnPickup = false;
		component.useGunforPickup = false;
		component.SetOffsets(Grid.DefaultOffset);
		if (this.animOverride != null)
		{
			base.Subscribe(856640610, new Action<object>(this.OnStorageChanged));
		}
	}

	private void OnStorageChanged(object data)
	{
		Storage storage = (Storage)data;
		if (storage == null)
		{
			GameObject prefab = Assets.GetPrefab(this.creatureTag);
			Vector3 vector = Grid.CellToPosCCC(Grid.PosToCell(base.gameObject.transform.position), Grid.SceneLayer.Creatures);
			GameObject gameObject = Util.KInstantiate(prefab, vector, Quaternion.identity, Folder.Entities);
			gameObject.SetActive(true);
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			MinionIdentity component = storage.GetComponent<MinionIdentity>();
			if (component != null)
			{
				KBatchedAnimController component2 = base.GetComponent<KBatchedAnimController>();
				component2.enabled = false;
			}
		}
	}

	[SerializeField]
	public KAnimFile animOverride;

	[SerializeField]
	public Tag creatureTag;

	private MinionIdentity minion;
}
