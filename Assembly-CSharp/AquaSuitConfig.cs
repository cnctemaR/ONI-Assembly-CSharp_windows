using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AquaSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("IronOre", 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.AQUASUIT.NAME, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.AQUASUIT.NAME, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.MAX_UNDERWATER_TRAVELCOST, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_UNDERWATER_TRAVELCOST, global::STRINGS.EQUIPMENT.PREFABS.AQUASUIT.NAME, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("AquaSuit", global::TUNING.EQUIPMENT.SUITS.SUITSLOT, global::TUNING.EQUIPMENT.SUITS.SUITFABRICATOR, global::TUNING.EQUIPMENT.SUITS.AQUASUIT_FABTIME, "IronOre", dictionary, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_MASS, "suit_water_slow", global::TUNING.EQUIPMENT.SUITS.SUIT_SNAPON, "body_water_slow", PathFinderFlags.None, list);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.AQUASUIT.RECIPEDESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.underwaterSupport = true;
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
	}
}
