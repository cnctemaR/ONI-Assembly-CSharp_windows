using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicInterasteroidSenderConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LogicInterasteroidSender", 1, 1, "inter_asteroid_automation_signal_sender_kanim", 30, 30f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
		buildingDef.AlwaysOperational = false;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort("InputPort", new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.LOGIC_PORT_INACTIVE, true, false) };
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicInterasteroidSender");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		base.ConfigureBuildingTemplate(go, prefab_tag);
		go.AddOrGet<UserNameable>().savedName = global::STRINGS.BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.DEFAULTNAME;
		go.AddOrGet<LogicBroadcaster>().PORT_ID = "InputPort";
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		LogicInterasteroidSenderConfig.AddVisualizer(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicInterasteroidSenderConfig.AddVisualizer(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		LogicInterasteroidSenderConfig.AddVisualizer(go);
	}

	private static void AddVisualizer(GameObject prefab)
	{
		SkyVisibilityVisualizer skyVisibilityVisualizer = prefab.AddOrGet<SkyVisibilityVisualizer>();
		skyVisibilityVisualizer.RangeMin = 0;
		skyVisibilityVisualizer.RangeMax = 0;
		skyVisibilityVisualizer.SkipOnModuleInteriors = true;
	}

	public const string ID = "LogicInterasteroidSender";

	public const string INPUT_PORT_ID = "InputPort";
}
