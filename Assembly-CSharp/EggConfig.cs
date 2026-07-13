using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public class EggConfig
{
	[Obsolete("Mod compatibility: Use CreateEgg with requiredDlcIds and forbiddenDlcIds")]
	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate)
	{
		return EggConfig.CreateEgg(id, name, desc, creature_id, anim, mass, egg_sort_order, base_incubation_rate, null, null);
	}

	[Obsolete("Mod compatibility: Use CreateEgg with requiredDlcIds and forbiddenDlcIds")]
	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate, string[] dlcIds)
	{
		string[] array;
		string[] array2;
		DlcManager.ConvertAvailableToRequireAndForbidden(dlcIds, out array, out array2);
		return EggConfig.CreateEgg(id, name, desc, creature_id, anim, mass, egg_sort_order, base_incubation_rate, array, array2);
	}

	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate, string[] requiredDlcIds, string[] forbiddenDlcIds)
	{
		return EggConfig.CreateEgg(id, name, desc, creature_id, anim, mass, egg_sort_order, base_incubation_rate, requiredDlcIds, forbiddenDlcIds, false);
	}

	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate, string[] requiredDlcIds, string[] forbiddenDlcIds, bool preventEggDrops)
	{
		return EggConfig.CreateEgg(id, name, desc, creature_id, anim, mass, egg_sort_order, base_incubation_rate, requiredDlcIds, forbiddenDlcIds, preventEggDrops, mass);
	}

	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate, string[] requiredDlcIds, string[] forbiddenDlcIds, bool preventEggDrops, float eggMassToDrop)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, true, Assets.GetAnim(anim), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true, 0, SimHashes.Creature, null);
		gameObject.AddOrGet<KBoxCollider2D>().offset = new Vector2f(0f, 0.36f);
		gameObject.AddOrGet<Pickupable>().sortOrder = SORTORDER.EGGS + egg_sort_order;
		gameObject.AddOrGet<Effects>();
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.AddTag(GameTags.Egg, false);
		kprefabID.AddTag(GameTags.IncubatableEgg, false);
		kprefabID.AddTag(GameTags.PedestalDisplayable, false);
		kprefabID.requiredDlcIds = requiredDlcIds;
		kprefabID.forbiddenDlcIds = forbiddenDlcIds;
		IncubationMonitor.Def def = gameObject.AddOrGetDef<IncubationMonitor.Def>();
		def.preventEggDrops = preventEggDrops;
		def.spawnedCreature = creature_id;
		def.baseIncubationRate = base_incubation_rate;
		gameObject.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
		global::UnityEngine.Object.Destroy(gameObject.GetComponent<EntitySplitter>());
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		if (!preventEggDrops)
		{
			EggCrackerConfig.RegisterEgg(id, name, desc, eggMassToDrop, requiredDlcIds, forbiddenDlcIds, EggConfig.CUSTOM_EGG_OUTPUTS.ContainsKey(creature_id) ? EggConfig.CUSTOM_EGG_OUTPUTS[creature_id].ToArray() : null);
		}
		return gameObject;
	}

	public static Dictionary<Tag, List<global::Tuple<Tag, float>>> CUSTOM_EGG_OUTPUTS = new Dictionary<Tag, List<global::Tuple<Tag, float>>>();
}
