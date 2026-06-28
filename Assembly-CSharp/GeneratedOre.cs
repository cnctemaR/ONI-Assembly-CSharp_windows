using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GeneratedOre
{
	public static void LoadGeneratedOre()
	{
		Type typeFromHandle = typeof(IOreConfig);
		Assembly assembly = Assembly.GetAssembly(typeof(GeneratedOre));
		Type[] types = assembly.GetTypes();
		HashSet<SimHashes> hashSet = new HashSet<SimHashes>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				object obj = Activator.CreateInstance(type);
				IOreConfig oreConfig = obj as IOreConfig;
				SimHashes elementID = oreConfig.ElementID;
				if (elementID != SimHashes.Void)
				{
					hashSet.Add(elementID);
				}
				GameObject gameObject = oreConfig.CreatePrefab();
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				Assets.AddPrefab(component);
			}
		}
		List<Element> elements = ElementLoader.elements;
		foreach (Element element in elements)
		{
			if (element != null && !hashSet.Contains(element.id))
			{
				if (element.substance != null && element.substance.anim != null)
				{
					GameObject gameObject2 = null;
					if (element.IsSolid)
					{
						gameObject2 = EntityTemplates.CreateSolidOreEntity(element.id, null);
					}
					else if (element.IsLiquid)
					{
						gameObject2 = EntityTemplates.CreateLiquidOreEntity(element.id, null);
					}
					else if (element.IsGas)
					{
						gameObject2 = EntityTemplates.CreateGasOreEntity(element.id, null);
					}
					if (gameObject2 != null)
					{
						KPrefabID component2 = gameObject2.GetComponent<KPrefabID>();
						Assets.AddPrefab(component2);
					}
				}
				else
				{
					Output.LogWarning(new object[] { "Missing substance or anim for element [" + element.name + "]" });
				}
			}
		}
	}

	public static void ConfigureAnims(GameObject prefab, string anim_file)
	{
		KAnimFile anim = Assets.GetAnim(anim_file);
		KBatchedAnimController kbatchedAnimController = prefab.AddOrGet<KBatchedAnimController>();
		if (anim != null)
		{
			kbatchedAnimController.SetAnims(new KAnimFile[] { anim }, true);
		}
	}

	public static SubstanceChunk CreateChunk(Element element, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		if (temperature <= 0f)
		{
			Output.LogWarning(new object[] { "GeneratedOre.CreateChunk tried to create a chunk with a temperature <= 0" });
		}
		SubstanceChunk component = GameUtil.KInstantiate(Assets.GetPrefab(element.tag), Grid.SceneLayer.Use, Folder.Ore, null, 0).GetComponent<SubstanceChunk>();
		component.transform.SetPosition(position);
		component.gameObject.SetActive(true);
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		component2.Mass = mass;
		component2.Temperature = temperature;
		component2.AddDisease(diseaseIdx, diseaseCount, "GeneratedOre.CreateChunk");
		KPrefabID component3 = component.GetComponent<KPrefabID>();
		component3.InitializeTags();
		return component;
	}
}
