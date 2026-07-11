using System;
using Klei;
using UnityEngine;

[SkipSaveFileSerialization]
public class EntitySplitter : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = base.GetComponent<Pickupable>();
		if (pickupable == null)
		{
			global::Debug.LogError(base.name + " does not have a pickupable component!", null);
		}
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
			return component != null && component2 != null && component.PrefabTag == component2.PrefabTag && pickupable.TotalAmount + other.TotalAmount <= this.maxStackSize;
		};
		base.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
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
		GameObject gameObject = null;
		if (pickupable.transform.parent != null)
		{
			gameObject = pickupable.transform.parent.gameObject;
		}
		GameObject gameObject2 = GameUtil.KInstantiate(prefab, pickupable.transform.GetPosition(), Grid.SceneLayer.Ore, gameObject, null, 0);
		Pickupable component = gameObject2.GetComponent<Pickupable>();
		if (component == null)
		{
			global::Debug.LogError("Edible::OnTake() No Pickupable component for " + gameObject2.name, gameObject2);
		}
		gameObject2.SetActive(true);
		component.TotalAmount = Mathf.Min(amount, pickupable.TotalAmount);
		pickupable.TotalAmount -= amount;
		component.Trigger(1335436905, pickupable);
		if (storage != null)
		{
			storage.Trigger(-1697596308, pickupable.gameObject);
			storage.Trigger(-778359855, null);
		}
		return component;
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			PrimaryElement primaryElement = pickupable.PrimaryElement;
			if (primaryElement != null)
			{
				float num = 0f;
				float mass = component.Mass;
				float mass2 = primaryElement.Mass;
				if (mass > 0f && mass2 > 0f)
				{
					num = SimUtil.CalculateFinalTemperature(mass, component.Temperature, mass2, primaryElement.Temperature);
				}
				else if (primaryElement.Mass > 0f)
				{
					num = primaryElement.Temperature;
				}
				component.SetMassTemperature(mass + mass2, num);
				if (CameraController.Instance != null)
				{
					string sound = GlobalAssets.GetSound("Ore_absorb", false);
					if (sound != null && CameraController.Instance.IsAudibleSound(pickupable.transform.GetPosition(), sound))
					{
						base.PlaySound3D(sound);
					}
				}
			}
		}
	}

	public float maxStackSize = float.MaxValue;
}
