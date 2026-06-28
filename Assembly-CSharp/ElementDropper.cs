using System;
using UnityEngine;

public class ElementDropper : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = this.storage.FindFirst(this.emitTag);
		if (gameObject == null)
		{
			return;
		}
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		if (component.Mass >= this.emitMass)
		{
			Pickupable pickupable = gameObject.GetComponent<Pickupable>();
			if (pickupable != null)
			{
				pickupable = pickupable.Take(this.emitMass);
				pickupable.transform.SetPosition(pickupable.transform.GetPosition() + this.emitOffset);
			}
			else
			{
				this.storage.Drop(gameObject);
				gameObject.transform.SetPosition(gameObject.transform.GetPosition() + this.emitOffset);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, pickupable.GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), pickupable.transform, 1.5f, false);
		}
	}

	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public float emitMass;

	[SerializeField]
	public Vector3 emitOffset = Vector3.zero;

	[MyCmpGet]
	private Storage storage;
}
