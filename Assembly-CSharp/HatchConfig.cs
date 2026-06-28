using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class HatchConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = global::STRINGS.CREATURES.SPECIES.HATCH.NAME;
		string text2 = "Hatch";
		string text3 = text;
		string text4 = global::STRINGS.CREATURES.SPECIES.HATCH.DESC;
		float num = 400f;
		KAnimFile anim = Assets.GetAnim("hatch_kanim");
		string text5 = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text2, text3, text4, num, anim, text5, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		Trait trait = Db.Get().CreateTrait("HatchBaseTrait", text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchConfig.CALORIE_BATTERY, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, text, false, false, true));
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, "HatchBaseTrait", "HatchNavGrid", NavType.Floor, 32, 2f, "Meat", 2, true, false, 30f, 283f, 294f, 243f, 343f);
		this.SetupDiet(gameObject);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "egg_hatch_kanim", HatchConfig.CALORIES_BURNED_PER_CYCLE_WILD, HatchConfig.CALORIES_BURNED_PER_CYCLE_TAME, HatchConfig.PEN_SPACE_REQUIRED_PER_CREATURE);
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<Capturable>();
		gameObject.AddOrGet<FloorSwitchActivator>();
		gameObject.AddOrGetDef<TrappedMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<BurrowMonitor.Def>();
		WorldSpawnableMonitor.Def def = gameObject.AddOrGetDef<WorldSpawnableMonitor.Def>();
		def.adjustSpawnLocationCb = new Func<int, int>(this.AdjustSpawnLocationCB);
		ThreatMonitor.Def def2 = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def2.fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		ElementEmitter elementEmitter = gameObject.AddElementEmitter(SimHashes.Carbon, 0f, 0f, SimUtil.DiseaseInfo.Invalid);
		elementEmitter.showDescriptor = false;
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "Hatch_footstep", NOISE_POLLUTION.CREATURES.TIER1);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_chew", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_emerge", NOISE_POLLUTION.CREATURES.TIER6);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_hide", NOISE_POLLUTION.CREATURES.TIER6);
		EntityTemplates.CreateAndRegisterPreview("Hatch_Preview", Assets.GetAnim("hatch_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, global::STRINGS.CREATURES.SPECIES.HATCH.NAME), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, global::STRINGS.CREATURES.SPECIES.HATCH.NAME), Assets.GetAnim("creature_sack_kanim"), "object", new Tag("Hatch_Preview"));
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new ExitBurrowStates.Def()).Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", global::STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, global::STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP))
			.Add(new TrappedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def())
			.PushInterruptGroup()
			.Add(new RanchedStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", global::STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, global::STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP))
			.Add(new LayEggStates.Def())
			.Add(new EatStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
			.PopInterruptGroup()
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(gameObject, builder);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private void SetupDiet(GameObject prefab)
	{
		TagBits tagBits = default(TagBits);
		Tag tag = new Tag("HatchEgg");
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			if (!(kprefabID.PrefabTag == tag))
			{
				if (!(kprefabID.GetComponent<ElementChunk>() == null))
				{
					PrimaryElement component = kprefabID.GetComponent<PrimaryElement>();
					if (!(component == null))
					{
						if (component.ElementID != SimHashes.Carbon)
						{
							if (component.ElementID != SimHashes.Phosphorite)
							{
								if (ElementLoader.FindElementByHash(component.ElementID).IsSolid)
								{
									tagBits.SetTag(kprefabID.PrefabTag);
								}
							}
						}
					}
				}
			}
		}
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(tagBits, SimHashes.Carbon, HatchConfig.CALORIES_PER_KG_OF_ORE, HatchConfig.COAL_PER_KG_OF_ORE, null, 0f));
		foreach (EdiblesManager.FoodInfo foodInfo in FOOD.FOOD_TYPES_LIST)
		{
			list.Add(new Diet.Info(new TagBits(new Tag(foodInfo.Id)), SimHashes.Carbon, foodInfo.CaloriesPerUnit, HatchConfig.COAL_PER_KG_OF_FOOD, null, 0f));
		}
		Diet diet = new Diet(list.ToArray());
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = HatchConfig.MIN_POOP_SIZE_IN_CALORIES;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
	}

	private int AdjustSpawnLocationCB(int cell)
	{
		while (!Grid.Solid[cell])
		{
			int num = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(cell))
			{
				break;
			}
			cell = num;
		}
		return cell;
	}

	public const string ID = "Hatch";

	public const string PREVIEW_ID = "Hatch_Preview";

	public const string BASE_TRAIT_ID = "HatchBaseTrait";

	private static float KG_ORE_EATEN_PER_CYCLE = 140f;

	private static float CALORIES_BURNED_PER_CYCLE_TAME = 700000f;

	private static float CALORIES_BURNED_PER_CYCLE_WILD = 175000f;

	private static float CALORIE_BATTERY = 10f * HatchConfig.CALORIES_BURNED_PER_CYCLE_TAME;

	private static float CALORIES_PER_KG_OF_ORE = HatchConfig.CALORIES_BURNED_PER_CYCLE_TAME / HatchConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float COAL_PER_KG_OF_ORE = 0.5f;

	private static float COAL_PER_KG_OF_FOOD = 0.75f;

	private static float MIN_POOP_SIZE_IN_CALORIES = HatchConfig.CALORIES_PER_KG_OF_ORE * HatchConfig.COAL_PER_KG_OF_ORE * 25f;

	private static int PEN_SPACE_REQUIRED_PER_CREATURE = 12;

	public const SimHashes poopElement = SimHashes.Carbon;
}
