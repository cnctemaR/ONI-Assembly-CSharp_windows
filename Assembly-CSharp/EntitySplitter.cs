using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class EntitySplitter : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = base.GetComponent<Pickupable>();
		Pickupable pickupable2 = pickupable;
		pickupable2.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable2.OnTake, new Func<float, Pickupable>((float amount) => EntitySplitter.Split(pickupable, amount, null)));
		pickupable.CanAbsorb = delegate(Pickupable other)
		{
			if (other == null)
			{
				return false;
			}
			KPrefabID component = pickupable.GetComponent<KPrefabID>();
			KPrefabID component2 = other.GetComponent<KPrefabID>();
			Edible component3 = this.GetComponent<Edible>();
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

	public static Pickupable Split(Pickupable pickupable, float amount, GameObject prefab = null)
	{
		if (amount >= pickupable.TotalAmount && prefab == null)
		{
			return pickupable;
		}
		Storage storage = pickupable.storage;
		if (prefab == null)
		{
			prefab = Assets.GetPrefab(pickupable.GetComponent<KPrefabID>().PrefabTag);
		}
		GameObject gameObject = GameUtil.KInstantiate(prefab, pickupable.transform.position, Grid.SceneLayer.Use, pickupable.transform.parent.gameObject, null, 0);
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component == null)
		{
			global::Debug.LogError("Edible::OnTake() No Pickupable component for " + gameObject.name, gameObject);
		}
		gameObject.SetActive(true);
		component.TotalAmount = Mathf.Min(amount, pickupable.TotalAmount);
		pickupable.TotalAmount -= amount;
		component.Trigger(1335436905, pickupable);
		if (storage != null)
		{
			storage.Trigger(-1697596308, pickupable.gameObject);
		}
		return component;
	}
}
