using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DietManager")]
public class DietManager : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		DietManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.diets = DietManager.CollectDiets(null);
		DietManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (Tag tag in WorldInventory.Instance.GetDiscovered())
		{
			this.Discover(tag);
		}
		foreach (KeyValuePair<Tag, Diet> keyValuePair in this.diets)
		{
			Diet.Info[] infos = keyValuePair.Value.infos;
			for (int i = 0; i < infos.Length; i++)
			{
				foreach (Tag tag2 in infos[i].consumedTags)
				{
					if (Assets.GetPrefab(tag2) == null)
					{
						global::Debug.LogError("Could not find prefab: " + tag2);
					}
				}
			}
		}
		WorldInventory.Instance.OnDiscover += this.OnWorldInventoryDiscover;
	}

	private void Discover(Tag tag)
	{
		foreach (KeyValuePair<Tag, Diet> keyValuePair in this.diets)
		{
			if (keyValuePair.Value.GetDietInfo(tag) != null)
			{
				WorldInventory.Instance.Discover(tag, keyValuePair.Key);
			}
		}
	}

	private void OnWorldInventoryDiscover(Tag category_tag, Tag tag)
	{
		this.Discover(tag);
	}

	public static Dictionary<Tag, Diet> CollectDiets(Tag[] target_species)
	{
		Dictionary<Tag, Diet> dictionary = new Dictionary<Tag, Diet>();
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			CreatureCalorieMonitor.Def def = kprefabID.GetDef<CreatureCalorieMonitor.Def>();
			if (def != null && (target_species == null || Array.IndexOf<Tag>(target_species, kprefabID.GetComponent<CreatureBrain>().species) >= 0))
			{
				dictionary[kprefabID.PrefabTag] = def.diet;
			}
		}
		return dictionary;
	}

	private Dictionary<Tag, Diet> diets;

	public static DietManager Instance;
}
