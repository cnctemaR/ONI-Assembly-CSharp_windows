using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FlowerVaseWallConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FlowerVaseWall";
		int num = 1;
		int num2 = 1;
		string text2 = "flowervase_wall_kanim";
		int num3 = 10;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnWall;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.DecorSeed);
		plantablePlot.occupyingObjectVisualOffset = new Vector3(0f, -0.25f, 0f);
		SituationalAnim situationalAnim = go.AddOrGet<SituationalAnim>();
		situationalAnim.mustSatisfy = SituationalAnim.MustSatisfy.All;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.instantiateFn += delegate(GameObject inst)
		{
			SituationalAnim component2 = inst.GetComponent<SituationalAnim>();
			component2.anims = new List<Tuple<SituationalAnim.Situation, string>>
			{
				new Tuple<SituationalAnim.Situation, string>(SituationalAnim.Situation.Left, "leftwall"),
				new Tuple<SituationalAnim.Situation, string>(SituationalAnim.Situation.Right, "rightwall")
			};
			component2.test = new Func<int, bool>(this.IsSolidOrTile);
		};
	}

	private bool IsSolidOrTile(int cell)
	{
		return Grid.Solid[cell];
	}

	public const string ID = "FlowerVaseWall";
}
