using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeoTunerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GeoTuner";
		int num = 4;
		int num2 = 3;
		string text2 = "geoTuner_kanim";
		int num3 = 30;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Floodable = true;
		buildingDef.Entombable = true;
		buildingDef.Overheatable = false;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "medium";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.UseStructureTemperature = true;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort("GEYSER_ERUPTION_STATUS_PORT", new CellOffset(-1, 1), global::STRINGS.BUILDINGS.PREFABS.GEOTUNER.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.GEOTUNER.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.GEOTUNER.LOGIC_PORT_INACTIVE, false, false) };
		buildingDef.RequiresPowerInput = true;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.AllowGeyserTuning.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.ScienceBuilding, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 0f;
		List<Storage.StoredItemModifier> list = new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Preserve
		};
		storage.SetDefaultStoredItemModifiers(list);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;
		manualDeliveryKG.capacity = 0f;
		manualDeliveryKG.refillMass = 0f;
		manualDeliveryKG.SetStorage(storage);
		go.AddOrGet<GeoTunerWorkable>();
		go.AddOrGet<GeoTunerSwitchGeyserWorkable>();
		go.AddOrGet<CopyBuildingSettings>();
		GeoTuner.Def def = go.AddOrGetDef<GeoTuner.Def>();
		def.OUTPUT_LOGIC_PORT_ID = "GEYSER_ERUPTION_STATUS_PORT";
		def.geotunedGeyserSettings = GeoTunerConfig.geotunerGeyserSettings;
		def.defaultSetting = GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.DEFAULT_CATEGORY];
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Laboratory.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static GeoTunerConfig()
	{
		Dictionary<GeoTunerConfig.Category, GeoTunerConfig.GeotunedGeyserSettings> dictionary = new Dictionary<GeoTunerConfig.Category, GeoTunerConfig.GeotunedGeyserSettings>();
		dictionary[GeoTunerConfig.Category.DEFAULT_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.Dirt.CreateTag(),
			quantity = 50f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.1f,
				temperatureModifier = 10f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		dictionary[GeoTunerConfig.Category.WATER_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.BleachStone.CreateTag(),
			quantity = 50f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.2f,
				temperatureModifier = 20f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		dictionary[GeoTunerConfig.Category.ORGANIC_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.Salt.CreateTag(),
			quantity = 50f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.2f,
				temperatureModifier = 15f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		dictionary[GeoTunerConfig.Category.HYDROCARBON_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.Katairite.CreateTag(),
			quantity = 100f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.2f,
				temperatureModifier = 15f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		dictionary[GeoTunerConfig.Category.VOLCANO_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.Katairite.CreateTag(),
			quantity = 100f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.2f,
				temperatureModifier = 150f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		dictionary[GeoTunerConfig.Category.METALS_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.Phosphorus.CreateTag(),
			quantity = 80f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.2f,
				temperatureModifier = 50f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		dictionary[GeoTunerConfig.Category.CO2_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings
		{
			material = SimHashes.ToxicSand.CreateTag(),
			quantity = 50f,
			duration = 600f,
			template = new Geyser.GeyserModification
			{
				massPerCycleModifier = 0.2f,
				temperatureModifier = 5f,
				iterationDurationModifier = 0f,
				iterationPercentageModifier = 0f,
				yearDurationModifier = 0f,
				yearPercentageModifier = 0f,
				maxPressureModifier = 0f
			}
		};
		GeoTunerConfig.CategorySettings = dictionary;
		GeoTunerConfig.geotunerGeyserSettings = new Dictionary<HashedString, GeoTunerConfig.GeotunedGeyserSettings>
		{
			{
				"steam",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"hot_steam",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"slimy_po2",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]
			},
			{
				"hot_po2",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]
			},
			{
				"methane",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]
			},
			{
				"chlorine_gas",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]
			},
			{
				"chlorine_gas_cool",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]
			},
			{
				"hot_co2",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.CO2_CATEGORY]
			},
			{
				"hot_hydrogen",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]
			},
			{
				"hot_water",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"salt_water",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"slush_salt_water",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"filthy_water",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"slush_water",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]
			},
			{
				"liquid_sulfur",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]
			},
			{
				"liquid_co2",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.CO2_CATEGORY]
			},
			{
				"oil_drip",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]
			},
			{
				"small_volcano",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.VOLCANO_CATEGORY]
			},
			{
				"big_volcano",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.VOLCANO_CATEGORY]
			},
			{
				"molten_copper",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			},
			{
				"molten_gold",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			},
			{
				"molten_iron",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			},
			{
				"molten_aluminum",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			},
			{
				"molten_cobalt",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			},
			{
				"molten_niobium",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			},
			{
				"molten_tungsten",
				GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]
			}
		};
	}

	public const int MAX_GEOTUNED = 5;

	public static Dictionary<GeoTunerConfig.Category, GeoTunerConfig.GeotunedGeyserSettings> CategorySettings;

	public static Dictionary<HashedString, GeoTunerConfig.GeotunedGeyserSettings> geotunerGeyserSettings;

	public const string ID = "GeoTuner";

	public const string OUTPUT_LOGIC_PORT_ID = "GEYSER_ERUPTION_STATUS_PORT";

	public const string GeyserAnimationModelTarget = "geyser_target";

	public const string GeyserAnimation_GeyserSymbols_LogicLightSymbol = "light_bloom";

	public struct GeotunedGeyserSettings
	{
		public GeotunedGeyserSettings(Tag material, float quantity, float duration, Geyser.GeyserModification template)
		{
			this.quantity = quantity;
			this.material = material;
			this.template = template;
			this.duration = duration;
		}

		public Tag material;

		public float quantity;

		public Geyser.GeyserModification template;

		public float duration;
	}

	public enum Category
	{
		DEFAULT_CATEGORY,
		WATER_CATEGORY,
		ORGANIC_CATEGORY,
		HYDROCARBON_CATEGORY,
		VOLCANO_CATEGORY,
		METALS_CATEGORY,
		CO2_CATEGORY
	}
}
