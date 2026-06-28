using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class QuarryingGunConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("IronOre", 200f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTEMOD_IDS.DIGGING, (float)global::TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_DIG, global::STRINGS.EQUIPMENT.PREFABS.QUARRYINGGUN.NAME, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("QuarryingGun", global::TUNING.EQUIPMENT.TOOLS.TOOLSLOT, global::TUNING.EQUIPMENT.TOOLS.TOOLFABRICATOR, global::TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_FABTIME, "IronOre", dictionary, (float)global::TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_MASS, "constructor_gun_kanim", string.Empty, string.Empty, PathFinderFlags.None, list);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.QUARRYINGGUN.RECIPEDESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
	}
}
