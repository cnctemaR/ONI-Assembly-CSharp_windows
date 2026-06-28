using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EntitySplitter : KMonoBehaviour, ISaveLoadableJson
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
			KPrefabID component = base.GetComponent<KPrefabID>();
			KPrefabID component2 = go.GetComponent<KPrefabID>();
			return component != null && component2 != null && component.PrefabTag == component2.PrefabTag;
		};
	}

	private Pickupable OnTake(float amount)
	{
		Storage storage = base.GetComponent<Pickupable>().storage;
		GameObject prefab = Assets.GetPrefab(base.GetComponent<KPrefabID>().PrefabTag);
		GameObject gameObject = GameUtil.KInstantiate(prefab, this.transform.position, Grid.SceneLayer.Use, this.transform.parent.gameObject, null, 0);
		Debug.Assert(gameObject != null, "WTH, the GO is null, shouldn't happen on instantiate");
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component == null)
		{
			Debug.LogError("Edible::OnTake() No Pickupable component for " + gameObject.name, gameObject);
		}
		gameObject.SetActive(true);
		component.TotalAmount = Mathf.Min(amount, this.pickupable.TotalAmount);
		if (amount >= this.pickupable.TotalAmount)
		{
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
}
