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
			if (element != null && element.IsSolid && !hashSet.Contains(element.id))
			{
				if (element.substance != null && element.substance.anim != null)
				{
					GameObject prefab = element.substance.GetPrefab();
					KPrefabID component2 = prefab.GetComponent<KPrefabID>();
					Assets.AddPrefab(component2);
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
}
