using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementSplitter : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = this.pickupable;
		pickupable.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable.OnTake, new Func<float, Pickupable>(this.OnTake));
		this.pickupable.CanAbsorb = delegate(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			PrimaryElement component = this.pickupable.GetComponent<PrimaryElement>();
			PrimaryElement component2 = go.GetComponent<PrimaryElement>();
			return component.ElementID == component2.ElementID && component.Units > 0f && component2.Units > 0f;
		};
	}

	private Pickupable OnTake(float amount)
	{
		Storage storage = this.pickupable.storage;
		Pickupable component = this.primaryElement.Element.substance.SpawnResource(this.transform.position, amount, this.primaryElement.Temperature, true, false).GetComponent<Pickupable>();
		if (amount >= this.pickupable.TotalAmount)
		{
			this.pickupable.GetComponent<PrimaryElement>().Mass = 0f;
			this.pickupable.gameObject.DeleteObject();
		}
		else
		{
			this.pickupable.TotalAmount = this.pickupable.TotalAmount - amount;
		}
		component.Trigger(1335436905, this.pickupable);
		if (storage != null)
		{
			storage.Trigger(-1697596308, base.gameObject);
		}
		return component;
	}

	[MyCmpReq]
	private Pickupable pickupable;

	[MyCmpReq]
	private PrimaryElement primaryElement;
}
