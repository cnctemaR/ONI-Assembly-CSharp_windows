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
			this.Subscribe(1228788923, new Action<object>(this.OnStorageChanged));
		}
	}

	private void OnStorageChanged(object data)
	{
		Pickupable component = base.GetComponent<Pickupable>();
		if (component.storage.GetComponent<MinionIdentity>() != null)
		{
			KBatchedAnimController component2 = base.GetComponent<KBatchedAnimController>();
			component2.Play("carry", KAnim.PlayMode.Once, 1f, 0f);
			KBatchedAnimController component3 = component.storage.GetComponent<KBatchedAnimController>();
			this.currentOwner = component3;
		}
		else if (this.currentOwner != null)
		{
			this.currentOwner = null;
		}
	}

	[SerializeField]
	public KAnimFile animOverride;

	private KBatchedAnimController currentOwner;
}
