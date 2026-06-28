using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public abstract class BaseWireConfig : IBuildingConfig
{
	public abstract override BuildingDef CreateBuildingDef();

	public BuildingDef CreateBuildingDef(string id, string anim, float mass, float construction_time, float[] construction_mass, float insulation, DecorValues decor, AttributeInfo[] attribute_infos = null)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, anim, 800f, 10, construction_time, construction_mass, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, decor, attribute_infos);
		buildingDef.Insulation = insulation;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.ObjectLayer = ObjectLayer.Wire;
		buildingDef.TileLayer = ObjectLayer.WireTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementWire;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.Wires;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.OverlayAnim = Assets.GetAnim(anim);
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
		go.AddOrGet<Wire>();
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kanimGraphTileVisualizer.isPhysicalBuilding = true;
		kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.GetComponent<Constructable>().isDiggingRequired = false;
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kanimGraphTileVisualizer.isPhysicalBuilding = false;
		kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
	}

	protected void DoPostConfigureComplete(Wire.WattageRating rating, GameObject go)
	{
		Wire component = go.GetComponent<Wire>();
		component.MaxWattageRating = rating;
		float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(rating);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.MAX_WATTAGE, GameUtil.GetFormattedWattage(maxWattageAsFloat, "F1")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MAX_WATTAGE, new object[0]), Descriptor.DescriptorType.Effect);
		Building component2 = go.GetComponent<Building>();
		BuildingDef def = component2.Def;
		if (def.EffectDescription == null)
		{
			def.EffectDescription = new List<Descriptor>();
		}
		def.EffectDescription.Add(descriptor);
		BuildingTemplates.DoPostConfigure(go);
	}
}
