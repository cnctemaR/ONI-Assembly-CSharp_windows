using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public static class BaseLightBugConfig
{
	public static GameObject BaseLightBug(string id, string name, string desc, string anim_file, string traitId, Color lightColor, EffectorValues decor, bool is_baby, string symbolOverridePrefix = null)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 5f, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Prey;
		string text = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		int num = 32;
		float num2 = 2f;
		string text2 = "Meat";
		int num3 = 0;
		bool flag = true;
		bool flag2 = true;
		float freezing_ = CREATURES.TEMPERATURE.FREEZING_2;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, traitId, text, navType, num, num2, text2, num3, flag, flag2, CREATURES.TEMPERATURE.FREEZING_1, CREATURES.TEMPERATURE.HOT_1, freezing_, CREATURES.TEMPERATURE.HOT_2);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
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
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, false, false);
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
		if (is_baby)
		{
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			component2.animWidth = 0.5f;
			component2.animHeight = 0.5f;
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
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		gameObject.AddOrGetDef<DrinkMilkMonitor.Def>();
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.LightBugSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, HashSet<Tag> consumed_tags, Tag producedTag, float caloriesPerKg)
	{
		Diet diet = new Diet(new Diet.Info[]
		{
			new Diet.Info(consumed_tags, producedTag, caloriesPerKg, 1f, null, 0f, false, false)
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
