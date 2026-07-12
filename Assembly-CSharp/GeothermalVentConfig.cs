using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeothermalVentConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public virtual GameObject CreatePrefab()
	{
		string text = "GeothermalVentEntity";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALVENT.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALVENT.EFFECT + "\n\n" + global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALVENT.DESC;
		float num = 100f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.PENALTY.TIER4;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_geospout_kanim"), "off", Grid.SceneLayer.BuildingBack, 3, 4, tier, tier2, SimHashes.Unobtanium, new List<Tag> { GameTags.Gravitas }, 293f);
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		gameObject.AddComponent<GeothermalVent>();
		gameObject.AddComponent<GeothermalPlantComponent>();
		gameObject.AddComponent<Operational>();
		gameObject.AddComponent<UserNameable>();
		Storage storage = gameObject.AddComponent<Storage>();
		storage.showCapacityAsMainStatus = false;
		storage.showCapacityStatusItem = false;
		storage.showDescriptor = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		LogicPorts logicPorts = inst.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = new LogicPorts.Port[0];
		logicPorts.outputPortInfo = new LogicPorts.Port[] { LogicPorts.Port.OutputPort("GEOTHERMAL_VENT_STATUS_PORT", new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALVENT.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALVENT.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALVENT.LOGIC_PORT_INACTIVE, false, false) };
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GeothermalVentEntity";

	public const string OUTPUT_LOGIC_PORT_ID = "GEOTHERMAL_VENT_STATUS_PORT";

	private const string ANIM_FILE = "gravitas_geospout_kanim";

	public const string OFFLINE_ANIM = "off";

	public const string QUEST_ENTOMBED_ANIM = "pooped";

	public const string IDLE_ANIM = "on";

	public const string OBSTRUCTED_ANIM = "over_pressure";

	public const int EMISSION_RANGE = 1;

	public const float EMISSION_INTERVAL_SEC = 0.2f;

	public const float EMISSION_MAX_PRESSURE_KG = 120f;

	public const float EMISSION_MAX_RATE_PER_TICK = 3f;

	public static string TOGGLE_ANIMATION = "working_loop";

	public static HashedString TOGGLE_ANIM_OVERRIDE = "anim_interacts_geospout_kanim";

	public const float TOGGLE_CHORE_DURATION_SECONDS = 10f;

	public static MathUtil.MinMax INITIAL_DEBRIS_VELOCIOTY = new MathUtil.MinMax(1f, 5f);

	public static MathUtil.MinMax INITIAL_DEBRIS_ANGLE = new MathUtil.MinMax(200f, 340f);

	public static MathUtil.MinMax DEBRIS_MASS_KG = new MathUtil.MinMax(30f, 34f);

	public const string BAROMETER_ANIM = "meter";

	public const string BAROMETER_TARGET = "meter_target";

	public static string[] BAROMETER_SYMBOLS = new string[] { "meter_target" };

	public const string CONNECTED_ANIM = "meter_connected";

	public const string CONNECTED_TARGET = "meter_connected_target";

	public static string[] CONNECTED_SYMBOLS = new string[] { "meter_connected_target" };

	public const float CONNECTED_PROGRESS = 1f;

	public const float DISCONNECTED_PROGRESS = 0f;
}
