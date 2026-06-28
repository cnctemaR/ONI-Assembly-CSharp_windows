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
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.DIGGING, (float)global::TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_DIG, global::STRINGS.EQUIPMENT.PREFABS.QUARRYING_GUN.NAME, false, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("QuarryingGun", global::TUNING.EQUIPMENT.TOOLS.TOOLSLOT, global::TUNING.EQUIPMENT.TOOLS.TOOLFABRICATOR, (float)global::TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_FABTIME, SimHashes.IronOre, dictionary, (float)global::TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_MASS, "constructor_gun_kanim", string.Empty, string.Empty, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, null);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.QUARRYING_GUN.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
	}
}
