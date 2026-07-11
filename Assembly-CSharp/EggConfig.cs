using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class EggConfig
{
	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, true, Assets.GetAnim(anim), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true, SimHashes.Creature, null);
		gameObject.AddOrGet<KBoxCollider2D>().offset = new Vector2f(0f, 0.36f);
		IncubatableEgg incubatableEgg = gameObject.AddOrGet<IncubatableEgg>();
		incubatableEgg.sortOrder = egg_sort_order;
		gameObject.AddOrGet<Effects>();
		KPrefabID kprefabID = gameObject.AddOrGet<KPrefabID>();
		kprefabID.AddPrefabTag(GameTags.Egg);
		IncubationMonitor.Def def = gameObject.AddOrGetDef<IncubationMonitor.Def>();
		def.spawnedCreature = creature_id;
		def.baseIncubationRate = base_incubation_rate;
		OvercrowdingMonitor.Def def2 = gameObject.AddOrGetDef<OvercrowdingMonitor.Def>();
		def2.spaceRequiredPerCreature = 0;
		global::UnityEngine.Object.Destroy(gameObject.GetComponent<EntitySplitter>());
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		string text = string.Format(BUILDINGS.PREFABS.EGGCRACKER.RESULT_DESCRIPTION, name);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(id, 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("RawEgg", 0.5f * mass),
			new ComplexRecipe.RecipeElement("EggShell", 0.5f * mass)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(id, "RawEgg"), array, array2);
		complexRecipe.description = string.Format(BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, name, text);
		complexRecipe.fabricators = new List<Tag> { "EggCracker" };
		complexRecipe.time = 5f;
		return gameObject;
	}
}
