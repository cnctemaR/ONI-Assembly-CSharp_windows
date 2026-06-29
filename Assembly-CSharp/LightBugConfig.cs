using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME;
		string text2 = "LightBug";
		string text3 = text;
		string text4 = global::STRINGS.CREATURES.SPECIES.LIGHTBUG.DESC;
		float num = 50f;
		KAnimFile anim = Assets.GetAnim("lightbug_kanim");
		string text5 = "idle";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text2, text3, text4, num, anim, text5, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		Trait trait = Db.Get().CreateTrait("LightBugBaseTrait", text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugConfig.CALORIE_BATTERY, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, text, false, false, true));
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Prey;
		text5 = "LightBugBaseTrait";
		text4 = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		num = 2f;
		text3 = "Meat";
		int num2 = 0;
		float freezing_ = global::TUNING.CREATURES.TEMPERATURE.FREEZING_1;
		float hot_ = global::TUNING.CREATURES.TEMPERATURE.HOT_1;
		float hot_2 = global::TUNING.CREATURES.TEMPERATURE.HOT_2;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, text5, text4, navType, 32, num, text3, num2, true, true, 30f, freezing_, hot_, global::TUNING.CREATURES.TEMPERATURE.FREEZING_2, hot_2);
		this.SetupDiet(gameObject);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "egg_lightbug_kanim", LightBugConfig.CALORIES_BURNED_PER_CYCLE_WILD, LightBugConfig.CALORIES_BURNED_PER_CYCLE_TAME, LightBugConfig.PEN_SPACE_REQUIRED_PER_CREATURE);
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.Flying);
		gameObject.AddOrGet<NotCapturable>();
		gameObject.AddOrGet<LoopingSounds>();
		LureableMonitor.Def def = gameObject.AddOrGetDef<LureableMonitor.Def>();
		def.ActiveBaitTag = GameTags.Phosphorite;
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		Light2D light2D = gameObject.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.LIGHTBUG_COLOR;
		light2D.Range = 5f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
		light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new TrappedStates.Def()).Add(new StunnedStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new DrowningStates.Def())
			.PushInterruptGroup()
			.Add(new RanchedStates.Def())
			.Add(new LayEggStates.Def())
			.Add(new EatStates.Def())
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
		LoopingSounds component = inst.GetComponent<LoopingSounds>();
		component.AddLoopingSoundUpdater();
		component.StartSound(GlobalAssets.GetSound("ShineBug_wings_LP", false), component.transform.GetPosition());
	}

	private void SetupDiet(GameObject prefab)
	{
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(new TagBits(PrickleFruitConfig.ID), SimHashes.Void, LightBugConfig.CALORIES_PER_KG_OF_ORE, 1f, null, 0f),
			new Diet.Info(GameTags.Phosphorite, SimHashes.Void, LightBugConfig.CALORIES_PER_KG_OF_ORE, 1f, null, 0f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
	}

	public const string ID = "LightBug";

	public const string BASE_TRAIT_ID = "LightBugBaseTrait";

	private static float KG_ORE_EATEN_PER_CYCLE = 0.25f;

	private static float CALORIES_BURNED_PER_CYCLE_TAME = 40000f;

	private static float CALORIES_BURNED_PER_CYCLE_WILD = 10000f;

	private static float CALORIE_BATTERY = 8f * LightBugConfig.CALORIES_BURNED_PER_CYCLE_TAME;

	private static float CALORIES_PER_KG_OF_ORE = LightBugConfig.CALORIES_BURNED_PER_CYCLE_TAME / LightBugConfig.KG_ORE_EATEN_PER_CYCLE;

	private static int PEN_SPACE_REQUIRED_PER_CREATURE = 12;
}
