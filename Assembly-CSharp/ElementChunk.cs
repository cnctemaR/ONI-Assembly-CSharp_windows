using System;
using Klei;
using UnityEngine;

[SkipSaveFileSerialization]
public class ElementChunk : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.OreSizeVisualizers.Add(base.gameObject);
		GameComps.ElementSplitters.Add(base.gameObject);
		base.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
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
}
