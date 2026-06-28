using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilFloaterConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = global::STRINGS.CREATURES.SPECIES.OILFLOATER.NAME;
		string text2 = "Oilfloater";
		string text3 = text;
		string text4 = global::STRINGS.CREATURES.SPECIES.OILFLOATER.DESC;
		float num = 400f;
		KAnimFile anim = Assets.GetAnim("oilfloater_kanim");
		string text5 = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text2, text3, text4, num, anim, text5, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 348.15f);
		Trait trait = Db.Get().CreateTrait("OilfloaterBaseTrait", text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterConfig.CALORIE_BATTERY, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, text, false, false, true));
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, "OilfloaterBaseTrait", "FloaterNavGrid", NavType.Hover, 32, 2f, "Meat", 2, false, false, 30f, 323.15f, 413.15f, 308.15f, 433.15f);
		this.SetupDiet(gameObject);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "egg_oilfloater_kanim", OilFloaterConfig.CALORIES_BURNED_PER_CYCLE_WILD, OilFloaterConfig.CALORIES_BURNED_PER_CYCLE_TAME, OilFloaterConfig.PEN_SPACE_REQUIRED_PER_CREATURE);
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.CreateAndRegisterPreview("Oilfloater_Preview", Assets.GetAnim("oilfloater_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, global::STRINGS.CREATURES.SPECIES.OILFLOATER.NAME), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, global::STRINGS.CREATURES.SPECIES.OILFLOATER.NAME), Assets.GetAnim("creature_interacts_trap_oilfloater_kanim"), "working_pre", new Tag("Oilfloater_Preview"));
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new TrappedStates.Def()).Add(new StunnedStates.Def())
			.Add(new SubmergedStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.ExitSubmerged, false, "idle_loop", global::STRINGS.CREATURES.STATUSITEMS.IDLE.NAME, global::STRINGS.CREATURES.STATUSITEMS.IDLE.TOOLTIP))
			.Add(new DebugGoToStates.Def())
			.PushInterruptGroup()
			.Add(new RanchedStates.Def())
			.Add(new InhaleStates.Def
			{
				inhaleSound = "OilFloater_intake_air"
			})
			.Add(new LayEggStates.Def())
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
		inst.Subscribe(1027377649, delegate(object data)
		{
			this.OnObjectMovementStateChanged(inst, data);
		});
	}

	private void SetupDiet(GameObject prefab)
	{
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(new Tag(SimHashes.CarbonDioxide.ToString()), SimHashes.CrudeOil, OilFloaterConfig.CALORIES_PER_KG_OF_ORE, OilFloaterConfig.POOP_PER_KG_OF_ORE, null, 0f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = OilFloaterConfig.MIN_POOP_SIZE_IN_CALORIES;
		GasAndLiquidConsumerMonitor.Def def2 = prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
		def2.diet = diet;
	}

	private void OnObjectMovementStateChanged(GameObject inst, object data)
	{
		string sound = GlobalAssets.GetSound("OilFloater_move_LP", false);
		GameHashes gameHashes = (GameHashes)data;
		if (gameHashes == GameHashes.ObjectMovementWakeUp)
		{
			LoopingSounds component = inst.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.AddLoopingSoundUpdater();
				component.StartSound(sound, inst.transform.GetPosition());
			}
		}
		else
		{
			LoopingSounds component2 = inst.GetComponent<LoopingSounds>();
			if (component2 != null)
			{
				component2.RemoveLoopingSoundUpdater();
				component2.StopSound(sound);
			}
		}
	}

	public const string ID = "Oilfloater";

	public const string PREVIEW_ID = "Oilfloater_Preview";

	public const string BASE_TRAIT_ID = "OilfloaterBaseTrait";

	public const SimHashes CONSUME_ELEMENT = SimHashes.CarbonDioxide;

	public const SimHashes EMIT_ELEMENT = SimHashes.CrudeOil;

	private static float KG_ORE_EATEN_PER_CYCLE = 20f;

	private static float CALORIES_BURNED_PER_CYCLE_TAME = 120000f;

	private static float CALORIES_BURNED_PER_CYCLE_WILD = 30000f;

	private static float CALORIE_BATTERY = 5f * OilFloaterConfig.CALORIES_BURNED_PER_CYCLE_TAME;

	private static float CALORIES_PER_KG_OF_ORE = OilFloaterConfig.CALORIES_BURNED_PER_CYCLE_TAME / OilFloaterConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float POOP_PER_KG_OF_ORE = 0.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = OilFloaterConfig.CALORIES_PER_KG_OF_ORE * OilFloaterConfig.POOP_PER_KG_OF_ORE * 0.5f;

	private static int PEN_SPACE_REQUIRED_PER_CREATURE = 12;
}
