using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class EntitySplitter : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = this.pickupable;
		pickupable.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable.OnTake, new Func<float, Pickupable>(this.OnTake));
		this.pickupable.CanAbsorb = delegate(Pickupable other)
		{
			if (other == null)
			{
				return false;
			}
			KPrefabID component = base.GetComponent<KPrefabID>();
			KPrefabID component2 = other.GetComponent<KPrefabID>();
			Edible component3 = base.GetComponent<Edible>();
			if (component3 != null)
			{
				Edible component4 = other.GetComponent<Edible>();
				if (component4 != null && component3.Units + component4.Units > 10f)
				{
					return false;
				}
			}
			return component != null && component2 != null && component.PrefabTag == component2.PrefabTag;
		};
	}

	private Pickupable OnTake(float amount)
	{
		if (amount >= this.pickupable.TotalAmount)
		{
			return this.pickupable;
		}
		Storage storage = base.GetComponent<Pickupable>().storage;
		GameObject prefab = Assets.GetPrefab(base.GetComponent<KPrefabID>().PrefabTag);
		GameObject gameObject = GameUtil.KInstantiate(prefab, this.transform.position, Grid.SceneLayer.Use, this.transform.parent.gameObject, null, 0);
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component == null)
		{
			global::Debug.LogError("Edible::OnTake() No Pickupable component for " + gameObject.name, gameObject);
		}
		gameObject.SetActive(true);
		component.TotalAmount = Mathf.Min(amount, this.pickupable.TotalAmount);
		this.pickupable.TotalAmount = this.pickupable.TotalAmount - amount;
		component.Trigger(1335436905, this.pickupable);
		if (storage != null)
		{
			storage.Trigger(-1697596308, base.gameObject);
		}
		return component;
	}

	[MyCmpReq]
	private Pickupable pickupable;
}
