using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SteamTurbineConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SteamTurbine";
		int num = 5;
		int num2 = 4;
		string text2 = "steamturbine_kanim";
		int num3 = 30;
		float num4 = 60f;
		string[] array = new string[] { "RefinedMetal", "Plastic" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		}, array, 1600f, BuildLocationRule.Anywhere, global::TUNING.BUILDINGS.DECOR.NONE, none, 1f);
		buildingDef.GeneratorWattageRating = 2000f;
		buildingDef.GeneratorBaseCapacity = 2000f;
		buildingDef.Entombable = true;
		buildingDef.IsFoundation = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.OverheatTemperature = 1273.15f;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SteamTurbineConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		GeneratedBuildings.RegisterLogicPorts(go, SteamTurbineConfig.INPUT_PORTS);
		Constructable component = go.GetComponent<Constructable>();
		component.requiredRolePerk = RoleManager.rolePerks.CanPowerTinker.id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SteamTurbineConfig.INPUT_PORTS);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(SteamTurbineConfig.StoredItemModifiers);
		Turbine turbine = go.AddOrGet<Turbine>();
		turbine.srcElem = SimHashes.Steam;
		turbine.pumpKGRate = 10f;
		turbine.requiredMassFlowDifferential = 3f;
		turbine.minEmitMass = 10f;
		turbine.maxRPM = 4000f;
		turbine.rpmAcceleration = turbine.maxRPM / 30f;
		turbine.rpmDeceleration = turbine.maxRPM / 20f;
		turbine.minGenerationRPM = 3000f;
		turbine.minActiveTemperature = 500f;
		turbine.emitTemperature = 425f;
		go.AddOrGet<Generator>();
		go.AddOrGet<LogicOperationalController>();
		Prioritizable.AddRef(go);
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(game_object);
			StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
			Extents extents = game_object.GetComponent<Building>().GetExtents();
			Extents extents2 = new Extents(extents.x, extents.y - 1, extents.width, extents.height + 1);
			data.OverrideExtents(extents2);
			GameComps.StructureTemperatures.SetData(handle, data);
		};
	}

	public const string ID = "SteamTurbine";

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
