using System;
using System.Collections.Generic;
using Klei.AI;

public class EquipmentDef : Def
{
	public string Name
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".NAME");
		}
	}

	public string Id;

	public string Slot;

	public string FabricatorId;

	public int FabricationTime;

	public string OutputElement;

	public Dictionary<string, float> InputElementMassMap;

	public float Mass;

	public KAnimFile Anim;

	public string SnapOn;

	public KAnimFile BuildOverride;

	public List<AttributeModifier> AttributeModifiers;

	public PathFinderFlags PathFinderFlags;

	public string RecipeDescription;
}
