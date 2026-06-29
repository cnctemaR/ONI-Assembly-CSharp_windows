using System;
using Klei.AI;
using UnityEngine;

public class EggConfig
{
	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, int egg_sort_order)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, false, Assets.GetAnim(anim), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true, SimHashes.Creature, null);
		gameObject.AddOrGet<KBoxCollider2D>().offset = new Vector2f(0f, 0.36f);
		IncubatableEgg incubatableEgg = gameObject.AddOrGet<IncubatableEgg>();
		incubatableEgg.sortOrder = egg_sort_order;
		gameObject.AddOrGet<Effects>();
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.AddPrefabTag(GameTags.Egg);
		IncubationMonitor.Def def = gameObject.AddOrGetDef<IncubationMonitor.Def>();
		def.spawnedCreature = creature_id;
		def.baseIncubation = 0.055555556f;
		OvercrowdingMonitor.Def def2 = gameObject.AddOrGetDef<OvercrowdingMonitor.Def>();
		def2.spaceRequiredPerCreature = 0;
		EdiblesManager.FoodInfo foodInfo = new EdiblesManager.FoodInfo(id, 0f, 0, 0f, 0f, 0f);
		EntityTemplates.ExtendEntityToFood(gameObject, foodInfo, false);
		global::UnityEngine.Object.Destroy(gameObject.GetComponent<EntitySplitter>());
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		return gameObject;
	}
}
