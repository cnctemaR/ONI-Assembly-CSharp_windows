using System;
using System.Collections.Generic;

public class DietManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.diets = DietManager.CollectDiets();
		DietManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.diets = DietManager.CollectDiets();
		foreach (Tag tag in WorldInventory.Instance.GetDiscovered())
		{
			this.OnWorldInventoryDiscover(tag);
		}
		WorldInventory.Instance.OnDiscover += this.OnWorldInventoryDiscover;
	}

	private void OnWorldInventoryDiscover(Tag t)
	{
		TagBits tagBits = new TagBits(t);
		foreach (KeyValuePair<Tag, Diet> keyValuePair in this.diets)
		{
			if (keyValuePair.Value.GetDietInfo(tagBits) != null)
			{
				WorldInventory.Instance.Discover(t, keyValuePair.Key);
			}
		}
	}

	public static Dictionary<Tag, Diet> CollectDiets()
	{
		Dictionary<Tag, Diet> dictionary = new Dictionary<Tag, Diet>();
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			CreatureCalorieMonitor.Def def = kprefabID.GetDef<CreatureCalorieMonitor.Def>();
			if (def != null)
			{
				dictionary[kprefabID.PrefabTag] = def.diet;
			}
		}
		return dictionary;
	}

	private Dictionary<Tag, Diet> diets;

	public static DietManager Instance;
}
