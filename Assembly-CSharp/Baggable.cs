using System;
using UnityEngine;

public class Baggable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.animOverride != null)
		{
			this.Subscribe(856640610, new Action<object>(this.OnStorageChanged));
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
		else if (storage.GetComponent<MinionIdentity>() != null)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play("carry", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	[SerializeField]
	public KAnimFile animOverride;

	[SerializeField]
	public Tag creatureTag;
}
