using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class EggConfig
{
	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, true, Assets.GetAnim(anim), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true, 0, SimHashes.Creature, null);
		gameObject.AddOrGet<KBoxCollider2D>().offset = new Vector2f(0f, 0.36f);
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		pickupable.sortOrder = SORTORDER.EGGS + egg_sort_order;
		gameObject.AddOrGet<Effects>();
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.AddTag(GameTags.Egg);
		kprefabID.AddTag(GameTags.IncubatableEgg);
		IncubationMonitor.Def def = gameObject.AddOrGetDef<IncubationMonitor.Def>();
		def.spawnedCreature = creature_id;
		def.baseIncubationRate = base_incubation_rate;
		OvercrowdingMonitor.Def def2 = gameObject.AddOrGetDef<OvercrowdingMonitor.Def>();
		def2.spaceRequiredPerCreature = 0;
		global::UnityEngine.Object.Destroy(gameObject.GetComponent<EntitySplitter>());
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		string text = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RESULT_DESCRIPTION, name);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(id, 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("RawEgg", 0.5f * mass),
			new ComplexRecipe.RecipeElement("EggShell", 0.5f * mass)
		};
		string text2 = ComplexRecipeManager.MakeObsoleteRecipeID(id, "RawEgg");
		string text3 = ComplexRecipeManager.MakeRecipeID(id, array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text3, array, array2);
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, name, text);
		complexRecipe.fabricators = new List<Tag> { "EggCracker" };
		complexRecipe.time = 5f;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text2, text3);
		return gameObject;
	}
}
