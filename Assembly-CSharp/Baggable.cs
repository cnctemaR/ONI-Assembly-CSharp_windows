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
		if (base.transform.parent != null && base.transform.parent.GetComponent<Trap>() != null)
		{
			base.GetComponent<KBatchedAnimController>().enabled = true;
		}
	}

	private void OnStorageChanged(object data)
	{
		if (!(data is Storage) && (data == null || !(bool)data))
		{
			GameObject prefab = Assets.GetPrefab(this.creatureTag);
			Vector3 vector = Grid.CellToPosCCC(Grid.PosToCell(base.gameObject.transform.GetPosition()), Grid.SceneLayer.Creatures);
			GameObject gameObject = Util.KInstantiate(prefab, vector, Quaternion.identity, Folder.Entities);
			gameObject.SetActive(true);
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			Storage storage = data as Storage;
			MinionIdentity minionIdentity = ((storage == null) ? null : storage.GetComponent<MinionIdentity>());
			if (minionIdentity != null)
			{
				KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
				component.enabled = false;
			}
		}
	}

	[SerializeField]
	public KAnimFile animOverride;

	[SerializeField]
	public Tag creatureTag;

	private MinionIdentity minion;
}
