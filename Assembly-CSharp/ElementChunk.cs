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
		this.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Use);
		this.transform.SetPosition(position);
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
				if (component.Mass > 0f && primaryElement.Mass > 0f)
				{
					float num = SimUtil.CalculateFinalTemperature(component.Mass, component.Temperature, primaryElement.Mass, primaryElement.Temperature);
					component.Temperature = num;
				}
				else if (primaryElement.Mass > 0f)
				{
					component.Temperature = primaryElement.Temperature;
				}
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
