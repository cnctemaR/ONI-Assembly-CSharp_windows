using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = global::STRINGS.CREATURES.SPECIES.PUFT.NAME;
		string text2 = "Puft";
		string text3 = text;
		string text4 = global::STRINGS.CREATURES.SPECIES.PUFT.DESC;
		float num = 50f;
		KAnimFile anim = Assets.GetAnim("puft_kanim");
		string text5 = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text2, text3, text4, num, anim, text5, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		Trait trait = Db.Get().CreateTrait("PuftBaseTrait", text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftConfig.CALORIE_BATTERY, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, text, false, false, true));
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, "PuftBaseTrait", "FlyerNavGrid1x1", NavType.Hover, 32, 2f, "Meat", 1, true, true, 30f, 302f, 318f, 243f, 343f);
		this.SetupDiet(gameObject);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "egg_puft_kanim", PuftConfig.CALORIES_BURNED_PER_CYCLE_WILD, PuftConfig.CALORIES_BURNED_PER_CYCLE_TAME, PuftConfig.PEN_SPACE_REQUIRED_PER_CREATURE);
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.Flying);
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<LureableMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		DiseaseSourceVisualizer diseaseSourceVisualizer = gameObject.AddOrGet<DiseaseSourceVisualizer>();
		diseaseSourceVisualizer.alwaysShowDisease = "SlimeLung";
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_intake", NOISE_POLLUTION.CREATURES.TIER4);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_toot", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_inflated", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new TrappedStates.Def()).Add(new StunnedStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new DrowningStates.Def())
			.PushInterruptGroup()
			.Add(new RanchedStates.Def())
			.Add(new InhaleStates.Def
			{
				inhaleSound = "Puft_air_intake"
			})
			.Add(new LayEggStates.Def())
			.Add(new MoveToLureStates.Def())
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
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(new Tag(SimHashes.ContaminatedOxygen.ToString()), SimHashes.SlimeMold, PuftConfig.CALORIES_PER_KG_OF_ORE, PuftConfig.POOP_PER_KG_OF_ORE, "SlimeLung", 1000f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = PuftConfig.MIN_POOP_SIZE_IN_CALORIES;
		GasAndLiquidConsumerMonitor.Def def2 = prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
		def2.diet = diet;
	}

	public const string ID = "Puft";

	public const string BASE_TRAIT_ID = "PuftBaseTrait";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ContaminatedOxygen;

	public const SimHashes EMIT_ELEMENT = SimHashes.SlimeMold;

	public const string EMIT_DISEASE = "SlimeLung";

	public const int EMIT_DISEASE_PER_KG = 1000;

	private static float KG_ORE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_BURNED_PER_CYCLE_TAME = 200000f;

	private static float CALORIES_BURNED_PER_CYCLE_WILD = 50000f;

	private static float CALORIE_BATTERY = 6f * PuftConfig.CALORIES_BURNED_PER_CYCLE_TAME;

	private static float CALORIES_PER_KG_OF_ORE = PuftConfig.CALORIES_BURNED_PER_CYCLE_TAME / PuftConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float POOP_PER_KG_OF_ORE = 0.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = PuftConfig.CALORIES_PER_KG_OF_ORE / PuftConfig.POOP_PER_KG_OF_ORE * 2f;

	private static int PEN_SPACE_REQUIRED_PER_CREATURE = 24;
}
