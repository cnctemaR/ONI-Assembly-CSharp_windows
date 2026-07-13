using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public static class BaseLightBugConfig
{
	public static GameObject BaseLightBug(string id, string name, string desc, string anim_file, string traitId, Color lightColor, EffectorValues decor, bool is_baby, string symbolOverridePrefix = null, string onDeathDropID = "", float onDeathDropCount = 0f)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 5f, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "lightbug_build_kanim", symbolOverridePrefix, FactionManager.FactionID.Prey, traitId, "FlyerNavGrid1x1", NavType.Hover, 32, 2f, onDeathDropID, onDeathDropCount, true, true, 283.15f, 313.15f, 173.15f, 373.15f);
		EggConfig.CUSTOM_EGG_OUTPUTS.Add(id + "Baby", new List<global::Tuple<Tag, float>>
		{
			new global::Tuple<Tag, float>(SimHashes.NaturalResin.CreateTag(), 5f)
		});
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num = CREATURES.SORTING.CRITTER_ORDER["LightBug"];
		pickupable.sortOrder = num;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Flyer, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[]
		{
			GameTags.Phosphorite,
			GameTags.Creatures.FlyersLure
		};
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		if (DlcManager.FeatureRadiationEnabled())
		{
			RadiationEmitter radiationEmitter = gameObject.AddOrGet<RadiationEmitter>();
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRadiusX = 6;
			radiationEmitter.emitRadiusY = radiationEmitter.emitRadiusX;
			radiationEmitter.emitRads = 60f;
			radiationEmitter.emissionOffset = new Vector3(0f, 0f, 0f);
			component.prefabSpawnFn += delegate(GameObject inst)
			{
				inst.GetComponent<RadiationEmitter>().SetEmitting(true);
			};
		}
		if (lightColor != Color.black)
		{
			Light2D light2D = gameObject.AddOrGet<Light2D>();
			light2D.Color = lightColor;
			light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
			light2D.Range = 5f;
			light2D.Angle = 0f;
			light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
			light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
			light2D.shape = global::LightShape.Circle;
			light2D.drawOverlay = true;
			light2D.Lux = 1800;
			gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
			gameObject.AddOrGetDef<CreatureLightToggleController.Def>();
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = true
			}, true, -1)
			.Add(new MoveToLureStates.Def(), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def
			{
				working_anim = "cc_working_shinebug"
			}, !is_baby, -1)
			.Add(new CritterEmoteStates.Def(Assets.GetAnim("lightbug_emotes_kanim")), !is_baby, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.LightBugSpecies, symbolOverridePrefix);
		gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>().condoPrefabTag = "AirBorneCritterCondo";
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, HashSet<Tag> consumed_tags, Tag producedTag, float caloriesPerKg)
	{
		Diet diet = new Diet(new Diet.Info[]
		{
			new Diet.Info(consumed_tags, producedTag, caloriesPerKg, 1f, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		});
		prefab.AddOrGetDef<CreatureCalorieMonitor.Def>().diet = diet;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}

	public static void SetupLoopingSounds(GameObject inst)
	{
		inst.GetComponent<LoopingSounds>().StartSound(GlobalAssets.GetSound("ShineBug_wings_LP", false));
	}
}
