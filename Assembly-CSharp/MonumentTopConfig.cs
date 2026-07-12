using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MonumentTopConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MonumentTop";
		int num = 5;
		int num2 = 5;
		string text2 = "victory_monument_upper_kanim";
		int num3 = 1000;
		float num4 = 60f;
		float[] array = new float[] { 2500f, 2500f, 5000f };
		string[] array2 = new string[]
		{
			SimHashes.Glass.ToString(),
			SimHashes.Diamond.ToString(),
			SimHashes.Steel.ToString()
		};
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE, tier, 0.2f);
		BuildingTemplates.CreateMonumentBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.AttachmentSlotTag = "MonumentTop";
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<MonumentPart>().part = MonumentPart.Part.Top;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<KBatchedAnimController>().initialAnim = "option_a";
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			MonumentPart monumentPart = game_object.AddOrGet<MonumentPart>();
			monumentPart.part = MonumentPart.Part.Top;
			monumentPart.selectableStatesAndSymbols = new List<global::Tuple<string, string>>();
			monumentPart.stateUISymbol = "upper";
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_a", "leira"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_b", "mae"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_c", "puft"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_d", "nikola"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_e", "burt"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_f", "rowan"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_g", "nisbet"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_h", "joshua"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_i", "ren"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_j", "hatch"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_k", "drecko"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_l", "driller"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_m", "gassymoo"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_n", "glom"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_o", "lightbug"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_p", "slickster"));
			monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_q", "pacu"));
			if (DlcManager.IsExpansion1Active())
			{
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_r", "bee"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_s", "critter"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_t", "caterpillar"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_u", "worm"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_v", "scout_bot"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_w", "MiMa"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_x", "Stinky"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_y", "Harold"));
				monumentPart.selectableStatesAndSymbols.Add(new global::Tuple<string, string>("option_z", "Nails"));
			}
		};
	}

	public const string ID = "MonumentTop";
}
