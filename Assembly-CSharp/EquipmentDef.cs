using System;
using System.Collections.Generic;
using Klei.AI;

public class EquipmentDef : Def
{
	public override string Name
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".NAME");
		}
	}

	public string Desc
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".DESC");
		}
	}

	public string Effect
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".EFFECT");
		}
	}

	public string GenericName
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".GENERICNAME");
		}
	}

	public string WornName
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".WORN_NAME");
		}
	}

	public string WornDesc
	{
		get
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".WORN_DESC");
		}
	}

	public string Id;

	public string Slot;

	public string FabricatorId;

	public float FabricationTime;

	public string RecipeTechUnlock;

	public SimHashes OutputElement;

	public Dictionary<string, float> InputElementMassMap;

	public float Mass;

	public KAnimFile Anim;

	public string SnapOn;

	public string SnapOn1;

	public KAnimFile BuildOverride;

	public int BuildOverridePriority;

	public bool IsBody;

	public List<AttributeModifier> AttributeModifiers;

	public string RecipeDescription;

	public List<Effect> EffectImmunites = new List<Effect>();

	public Action<Equippable> OnEquipCallBack;

	public Action<Equippable> OnUnequipCallBack;

	public EntityTemplates.CollisionShape CollisionShape;

	public float width;

	public float height = 0.325f;

	public Tag[] AdditionalTags;

	public string wornID;

	public List<Descriptor> additionalDescriptors = new List<Descriptor>();
}
