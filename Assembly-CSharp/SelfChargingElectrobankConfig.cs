using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SelfChargingElectrobankConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1.Append<string>(DlcManager.DLC3);
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("SelfChargingElectrobank", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_SELFCHARGING.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_SELFCHARGING.DESC, 20f, true, Assets.GetAnim("electrobank_large_uranium_kanim"), "idle1", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.8f, true, 0, SimHashes.EnrichedUranium, new List<Tag>
		{
			GameTags.ChargedPortableBattery,
			GameTags.PedestalDisplayable
		});
		RadiationEmitter radiationEmitter = gameObject.AddOrGet<RadiationEmitter>();
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
		radiationEmitter.radiusProportionalToRads = false;
		radiationEmitter.emitRadiusX = 5;
		radiationEmitter.emitRadiusY = radiationEmitter.emitRadiusX;
		radiationEmitter.emitRads = 120f;
		radiationEmitter.emissionOffset = new Vector3(0f, 0f, 0f);
		if (!Assets.IsTagCountable(GameTags.ChargedPortableBattery))
		{
			Assets.AddCountableTag(GameTags.ChargedPortableBattery);
		}
		gameObject.GetComponent<KCollider2D>();
		gameObject.AddTag(GameTags.IndustrialProduct);
		SelfChargingElectrobank selfChargingElectrobank = gameObject.AddComponent<SelfChargingElectrobank>();
		selfChargingElectrobank.rechargeable = false;
		selfChargingElectrobank.keepEmpty = true;
		selfChargingElectrobank.radioactivityTuning = radiationEmitter.emitRads;
		gameObject.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(1, 1));
		gameObject.AddOrGet<DecorProvider>().SetValues(DECOR.PENALTY.TIER0);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SelfChargingElectrobank";

	public const float MASS = 20f;

	public const float POWER_DURATION = 90000f;

	public const float SELF_CHARGE_WATTAGE = 60f;
}
