using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedOre
{
	public static void LoadGeneratedOre(List<Type> types)
	{
		Type typeFromHandle = typeof(IOreConfig);
		HashSet<SimHashes> hashSet = new HashSet<SimHashes>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				IOreConfig oreConfig = Activator.CreateInstance(type) as IOreConfig;
				SimHashes elementID = oreConfig.ElementID;
				if (elementID != SimHashes.Void)
				{
					hashSet.Add(elementID);
				}
				Assets.AddPrefab(oreConfig.CreatePrefab().GetComponent<KPrefabID>());
			}
		}
		foreach (Element element in ElementLoader.elements)
		{
			if (element != null && !hashSet.Contains(element.id))
			{
				if (element.substance != null && element.substance.anim != null)
				{
					GameObject gameObject = null;
					if (element.IsSolid)
					{
						gameObject = EntityTemplates.CreateSolidOreEntity(element.id, null);
					}
					else if (element.IsLiquid)
					{
						gameObject = EntityTemplates.CreateLiquidOreEntity(element.id, null);
					}
					else if (element.IsGas)
					{
						gameObject = EntityTemplates.CreateGasOreEntity(element.id, null);
					}
					if (gameObject != null)
					{
						Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
					}
				}
				else
				{
					global::Debug.LogError("Missing substance or anim for element [" + element.name + "]");
				}
			}
		}
	}

	public static SubstanceChunk CreateChunk(Element element, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		if (temperature <= 0f)
		{
			DebugUtil.LogWarningArgs(new object[] { "GeneratedOre.CreateChunk tried to create a chunk with a temperature <= 0" });
		}
		GameObject prefab = Assets.GetPrefab(element.tag);
		if (prefab == null)
		{
			global::Debug.LogError("Could not find prefab for element " + element.id.ToString());
		}
		SubstanceChunk component = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0).GetComponent<SubstanceChunk>();
		component.transform.SetPosition(position);
		component.gameObject.SetActive(true);
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		component2.Mass = mass;
		component2.Temperature = temperature;
		component2.AddDisease(diseaseIdx, diseaseCount, "GeneratedOre.CreateChunk");
		component.GetComponent<KPrefabID>().InitializeTags(false);
		return component;
	}
}
