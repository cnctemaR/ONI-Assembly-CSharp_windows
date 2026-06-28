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
		Pickupable pickupable2 = pickupable;
		pickupable2.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable2.OnTake, new Func<float, Pickupable>((float amount) => EntitySplitter.Split(pickupable, amount, null)));
		pickupable.CanAbsorb = delegate(Pickupable other)
		{
			bool flag;
			if (other == null)
			{
				flag = false;
			}
			else
			{
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
				flag = component != null && component2 != null && component.PrefabTag == component2.PrefabTag;
			}
			return flag;
		};
		base.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
	}

	public static Pickupable Split(Pickupable pickupable, float amount, GameObject prefab = null)
	{
		Pickupable pickupable2;
		if (amount >= pickupable.TotalAmount && prefab == null)
		{
			pickupable2 = pickupable;
		}
		else
		{
			Storage storage = pickupable.storage;
			if (prefab == null)
			{
				prefab = Assets.GetPrefab(pickupable.GetComponent<KPrefabID>().PrefabTag);
			}
			GameObject gameObject = GameUtil.KInstantiate(prefab, pickupable.transform.position, Grid.SceneLayer.Ore, pickupable.transform.parent.gameObject, null, 0);
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
			pickupable2 = component;
		}
		return pickupable2;
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
				global::UnityEngine.Debug.Assert(component.Temperature > 0f || component.Mass == 0f, "OnAbsorb resulted in a temperature of 0", base.gameObject);
				if (CameraController.Instance != null)
				{
					string sound = GlobalAssets.GetSound("Ore_absorb", false);
					if (sound != null && CameraController.Instance.IsAudibleSound(pickupable.transform.position, sound))
					{
						base.PlaySound3D(sound);
					}
				}
			}
		}
	}
}
