using System;
using Klei;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilFloaterConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Oilfloater", global::STRINGS.CREATURES.SPECIES.OIL_FLOATER.NAME, global::STRINGS.CREATURES.SPECIES.OIL_FLOATER.DESC, 400f, Assets.GetAnim("oilfloater_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 348.15f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, 25f, "FloaterNavGrid", NavType.Hover, 2f, "Meat", 2, false, false, 30f, 323.15f, 413.15f, 308.15f, 433.15f);
		OilFloater oilFloater = gameObject.UpdateComponentRequirement<OilFloater>(true);
		oilFloater.consumedElement = SimHashes.CarbonDioxide;
		oilFloater.consumptionRate = 0.25f;
		oilFloater.minimumApproachMass = 0.1f;
		gameObject.UpdateComponentRequirement<Trappable>(true);
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		gameObject.UpdateComponentRequirement<SimpleMover>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		Storage storage = gameObject.UpdateComponentRequirement<Storage>(true);
		storage.capacityKg = 50f;
		ElementEmitter elementEmitter = gameObject.AddElementEmitter(SimHashes.CrudeOil, 0f, 0f, SimUtil.DiseaseInfo.Invalid);
		elementEmitter.showDescriptor = false;
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterPreview("Oilfloater_Preview", Assets.GetAnim("oilfloater_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, global::STRINGS.CREATURES.SPECIES.OIL_FLOATER.NAME), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, global::STRINGS.CREATURES.SPECIES.OIL_FLOATER.NAME), Assets.GetAnim("creature_interacts_trap_oilfloater_kanim"), "working_pre", new Tag("Oilfloater_Preview"));
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Oilfloater";

	public const string PREVIEW_ID = "Oilfloater_Preview";

	public const SimHashes consumeElement = SimHashes.CarbonDioxide;

	public const float consumptionRate = 0.25f;

	public const float minimumApproachMass = 0.1f;

	public const SimHashes emitElement = SimHashes.CrudeOil;
}
