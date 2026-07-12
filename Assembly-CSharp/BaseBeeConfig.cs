using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public static class BaseBeeConfig
{
	public static GameObject BaseBee(string id, string name, string desc, string anim_file, string traitId, EffectorValues decor, bool is_baby, string symbolOverridePrefix = null)
	{
		float num = 5f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num2 = 1;
		int num3 = 1;
		float freezing_ = CREATURES.TEMPERATURE.FREEZING_3;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, sceneLayer, num2, num3, decor, default(EffectorValues), SimHashes.Creature, null, freezing_);
		string text2 = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		int num4 = 5;
		if (is_baby)
		{
			text2 = "WalkerBabyNavGrid";
			navType = NavType.Floor;
			num4 = 1;
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Hostile, traitId, text2, navType, 32, (float)num4, "Meat", 0, true, true, 223.15f, 273.15f, 173.15f, 283.15f);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num5 = CREATURES.SORTING.CRITTER_ORDER["Bee"];
		pickupable.sortOrder = num5;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, false, false);
		gameObject.AddOrGetDef<AgeMonitor.Def>();
		Bee bee = gameObject.AddOrGet<Bee>();
		RadiationEmitter radiationEmitter = gameObject.AddComponent<RadiationEmitter>();
		radiationEmitter.emitRate = 0.1f;
		gameObject.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "RadiationSickness";
		if (!is_baby)
		{
			component.AddTag(GameTags.Creatures.Flyer, false);
			bee.radiationOutputAmount = 240f;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRadiusX = 3;
			radiationEmitter.emitRadiusY = 3;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			gameObject.AddOrGetDef<SubmergedMonitor.Def>();
			gameObject.AddWeapon(2f, 3f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		}
		else
		{
			bee.radiationOutputAmount = 120f;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRadiusX = 2;
			radiationEmitter.emitRadiusY = 2;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
			gameObject.AddOrGetDef<BeeHiveMonitor.Def>();
			gameObject.AddOrGet<Trappable>();
			EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		}
		gameObject.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = CREATURES.SPACE_REQUIREMENTS.TIER1;
		gameObject.AddOrGetDef<BeeHappinessMonitor.Def>();
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.1f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = false;
		elementConsumer.showDescriptor = true;
		elementConsumer.EnableConsumption(false);
		gameObject.AddOrGetDef<BeeSleepMonitor.Def>();
		gameObject.AddOrGetDef<BeeForagingMonitor.Def>();
		gameObject.AddOrGet<Storage>();
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), true, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new BeeSleepStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("attack_pre", "attack_pst", new CellOffset[]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1),
				new CellOffset(-1, 1)
			}), !is_baby, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new BeeMakeHiveStates.Def(), true, -1)
			.Add(new BeeForageStates.Def(SimHashes.UraniumOre.CreateTag(), BeeHiveTuning.ORE_DELIVERY_AMOUNT), true, -1)
			.Add(new BuzzStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.BeetaSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static void SetupLoopingSounds(GameObject inst)
	{
		inst.GetComponent<LoopingSounds>().StartSound(GlobalAssets.GetSound("Bee_wings_LP", false));
	}
}
