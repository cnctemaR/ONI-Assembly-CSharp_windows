using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class EquipmentTemplates
{
	public static EquipmentDef CreateEquipmentDef(string Id, string Slot, string FabricatorId, float FabricationTime, SimHashes OutputElement, Dictionary<string, float> InputElementMassMap, float Mass, string Anim, string SnapOn, string BuildOverride, PathFinderFlags PathFinderFlags, List<AttributeModifier> AttributeModifiers, string SnapOn1 = null, bool IsBody = false, EntityTemplates.CollisionShape CollisionShape = EntityTemplates.CollisionShape.CIRCLE, float width = 0.325f, float height = 0.325f)
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
		equipmentDef.SnapOn1 = SnapOn1;
		equipmentDef.BuildOverride = ((BuildOverride == null || BuildOverride.Length <= 0) ? null : Assets.GetAnim(BuildOverride));
		equipmentDef.IsBody = IsBody;
		equipmentDef.PathFinderFlags = PathFinderFlags;
		equipmentDef.AttributeModifiers = AttributeModifiers;
		equipmentDef.CollisionShape = CollisionShape;
		equipmentDef.width = width;
		equipmentDef.height = height;
		return equipmentDef;
	}
}
