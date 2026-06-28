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
				GameObject gameObject = GeneratedOre.CreateBasePrefab(elementID);
				oreConfig.ConfigurePrefab(gameObject);
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				Assets.AddPrefab(component);
			}
		}
		List<Element> elements = ElementLoader.elements;
		foreach (Element element in elements)
		{
			if (element != null && element.IsSolid && !hashSet.Contains(element.id) && element.substance != null)
			{
				KPrefabID prefab = element.substance.GetPrefab();
				KPrefabID component2 = prefab.GetComponent<KPrefabID>();
				Assets.AddPrefab(component2);
			}
		}
	}

	private static GameObject CreateBasePrefab(SimHashes elementID)
	{
		GameObject genericResource = EntityPrefabs.Instance.GenericResource;
		genericResource.SetActive(false);
		GameObject gameObject = GameUtil.KInstantiate(genericResource, Vector3.zero, Grid.SceneLayer.Use, Folder.EntityPrefabs, null, 0);
		if (elementID != SimHashes.Void)
		{
			string text = elementID.ToString();
			gameObject.name = text;
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			component.PrefabTag = TagManager.Create(text, null);
			component.UpdateSaveLoadTag();
			component.InitializeTags();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.ElementID = elementID;
			component2.UpdateElementTags = true;
			component2.UpdateTags();
			KSelectable component3 = gameObject.GetComponent<KSelectable>();
			component3.SetName(Strings.Get("STRINGS.ELEMENTS." + text.ToUpper() + ".NAME"));
		}
		return gameObject;
	}

	public static void ConfigureAnims(GameObject prefab, string anim_file)
	{
		KAnimFile anim = Assets.GetAnim(anim_file);
		KBatchedAnimController kbatchedAnimController = prefab.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.SetAnims(new KAnimFile[] { anim }, true);
	}

	public static KBatchedAnimController AddEffect(GameObject prefab, string fx_anim_file, string anim, KAnim.PlayMode mode = KAnim.PlayMode.Loop)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "fx";
		gameObject.transform.parent = prefab.transform;
		gameObject.transform.localPosition = Vector3.zero;
		KPrefabID kprefabID = gameObject.AddComponent<KPrefabID>();
		kprefabID.PrefabTag = TagManager.Create(prefab.name + "FX", null);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.initialAnim = anim;
		kbatchedAnimController.initialMode = mode;
		GeneratedOre.ConfigureAnims(gameObject, fx_anim_file);
		return kbatchedAnimController;
	}
}
