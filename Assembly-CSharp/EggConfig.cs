using System;
using UnityEngine;

public class EggConfig
{
	public static GameObject CreateEgg(string id, Tag creature_id, string anim)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, Strings.Get("STRINGS.CREATURES.SPECIES." + creature_id.Name.ToUpper() + ".EGG_NAME"), Strings.Get("STRINGS.CREATURES.SPECIES." + creature_id.Name.ToUpper() + ".DESC"), 1f, false, Assets.GetAnim(anim), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true, SimHashes.Creature, null);
		gameObject.AddOrGet<KBoxCollider2D>().offset = new Vector2f(0f, 0.36f);
		gameObject.AddOrGet<IncubatableEgg>();
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.AddPrefabTag(GameTags.Egg);
		IncubationMonitor.Def def = gameObject.AddOrGetDef<IncubationMonitor.Def>();
		def.spawnedCreature = creature_id;
		def.baseIncubation = 0.055555556f;
		EdiblesManager.FoodInfo foodInfo = new EdiblesManager.FoodInfo(id, 0f, 0, 255.15f, 277.15f, 9600f);
		EntityTemplates.ExtendEntityToFood(gameObject, foodInfo, true);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		return gameObject;
	}
}
