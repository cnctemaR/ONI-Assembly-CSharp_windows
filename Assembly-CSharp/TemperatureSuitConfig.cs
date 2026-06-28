using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class TemperatureSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("Iron", 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURESUIT.NAME, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURESUIT.NAME, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("TemperatureSuit", global::TUNING.EQUIPMENT.SUITS.SUITSLOT, global::TUNING.EQUIPMENT.SUITS.SUITFABRICATOR, global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_FABTIME, "Iron", dictionary, (float)global::TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_MASS, global::TUNING.EQUIPMENT.SUITS.SUIT_ANIM, global::TUNING.EQUIPMENT.SUITS.SUIT_SNAPON, "body_oxygen", PathFinderFlags.SuitRequired, list);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.TEMPERATURESUIT.RECIPEDESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Water";
		suitTank.amount = 100f;
	}
}
