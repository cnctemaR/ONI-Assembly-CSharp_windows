using System;
using Klei;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ElementChunk")]
public class ElementChunk : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.OreSizeVisualizers.Add(base.gameObject);
		GameComps.ElementSplitters.Add(base.gameObject);
		base.Subscribe<ElementChunk>(-2064133523, ElementChunk.OnAbsorbDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		base.transform.SetPosition(position);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Element element = component.Element;
		KSelectable component2 = base.GetComponent<KSelectable>();
		Func<Element> func = () => element;
		component2.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, func);
		component2.AddStatusItem(Db.Get().MiscStatusItems.OreMass, base.gameObject);
		component2.AddStatusItem(Db.Get().MiscStatusItems.OreTemp, base.gameObject);
	}

	protected override void OnCleanUp()
	{
		GameComps.ElementSplitters.Remove(base.gameObject);
		GameComps.OreSizeVisualizers.Remove(base.gameObject);
		base.OnCleanUp();
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			PrimaryElement primaryElement = pickupable.PrimaryElement;
			if (primaryElement != null)
			{
				float mass = primaryElement.Mass;
				if (mass > 0f)
				{
					PrimaryElement component = base.GetComponent<PrimaryElement>();
					float mass2 = component.Mass;
					float num = ((mass2 > 0f) ? SimUtil.CalculateFinalTemperature(mass2, component.Temperature, mass, primaryElement.Temperature) : primaryElement.Temperature);
					component.SetMassTemperature(mass2 + mass, num);
				}
				if (CameraController.Instance != null)
				{
					string sound = GlobalAssets.GetSound("Ore_absorb", false);
					Vector3 position = pickupable.transform.GetPosition();
					position.z = 0f;
					if (sound != null && CameraController.Instance.IsAudibleSound(position, sound))
					{
						KFMOD.PlayOneShot(sound, position, 1f);
					}
				}
			}
		}
	}

	private static readonly EventSystem.IntraObjectHandler<ElementChunk> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<ElementChunk>(delegate(ElementChunk component, object data)
	{
		component.OnAbsorb(data);
	});
}
