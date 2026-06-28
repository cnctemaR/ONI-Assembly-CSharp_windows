using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class EquipmentTemplates
{
	public static EquipmentDef CreateEquipmentDef(string Id, string Slot, string FabricatorId, int FabricationTime, string OutputElement, Dictionary<string, float> InputElementMassMap, float Mass, string Anim, string SnapOn, string BuildOverride, PathFinderFlags PathFinderFlags, List<AttributeModifier> AttributeModifiers)
	{
		EquipmentDef equipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
		equipmentDef.Id = Id;
		equipmentDef.Slot = Slot;
		equipmentDef.FabricatorId = FabricatorId;
		equipmentDef.FabricationTime = FabricationTime;
		equipmentDef.OutputElement = OutputElement;
		equipmentDef.InputElementMassMap = InputElementMassMap;
		equipmentDef.Mass = Mass;
		equipmentDef.Anim = Assets.GetAnim(Anim);
		equipmentDef.SnapOn = SnapOn;
		equipmentDef.BuildOverride = ((BuildOverride == null || BuildOverride.Length <= 0) ? null : Assets.GetAnim(BuildOverride));
		equipmentDef.PathFinderFlags = PathFinderFlags;
		equipmentDef.AttributeModifiers = AttributeModifiers;
		return equipmentDef;
	}
}
