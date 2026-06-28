using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadableJson, IGameObjectEffectDescriptor
{
	public Equippable()
	{
		base.SetWorkTime(1f);
	}

	protected override Assignables GetAssignables()
	{
		return this.assignablesRef.Get();
	}

	protected override void SetAssignables(Assignables assignables)
	{
		this.assignablesRef.Set((Equipment)assignables);
	}

	private Equipment equipment
	{
		get
		{
			return this.GetAssignables() as Equipment;
		}
		set
		{
			this.SetAssignables(value);
		}
	}

	protected override void OnPrefabInit()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		KPrefabID originalPrefab = component.GetOriginalPrefab();
		Equippable component2 = originalPrefab.GetComponent<Equippable>();
		this.def = component2.def;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip") };
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		if (this.equipment != null && !this.equipment.IsEquipped(this))
		{
			this.CreateChore();
		}
	}

	private void CreateChore()
	{
		this.chore = new WorkChore<Equippable>(Db.Get().ChoreTypes.Equip, this, this.equipment.GetComponent<ChoreProvider>(), true, null, null, null, true, null, true, default(Tag), null, false, true);
	}

	public bool IsEquipped()
	{
		return this.equipment != null && this.equipment.IsEquipped(this);
	}

	public override Assignables GetAssignables(GameObject go)
	{
		return go.GetComponent<Equipment>();
	}

	public void ClickAssign(Assignables new_assignables)
	{
		this.OnClickAssign(new_assignables);
	}

	protected override void OnClickAssign(Assignables new_assignables)
	{
		base.OnClickAssign(new_assignables);
		if (this.chore != null)
		{
			this.chore.Cancel("Equipment Reassigned");
			this.chore = null;
		}
		if (new_assignables != null)
		{
			this.CreateChore();
		}
	}

	public Equipment GetEquipment()
	{
		return this.equipment;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (this.equipment != null)
		{
			this.equipment.Equip(this);
		}
	}

	public override string[] GetWorkAnims(Worker worker)
	{
		return Equippable.WorkAnims;
	}

	public void OnEquip(EquipmentSlotInstance slot)
	{
		Attributes attributes = slot.gameObject.GetAttributes();
		string name = base.GetComponent<KSelectable>().GetName();
		foreach (AttributeModifier attributeModifier in this.def.AttributeModifiers)
		{
			attributes.Add(name, attributeModifier);
		}
		slot.gameObject.GetComponent<SnapOn>().AttachSnapOnByName(this.def.SnapOn);
		slot.gameObject.GetComponent<Navigator>().SetAbilityFlag(this.def.PathFinderFlags);
		if (this.def.BuildOverride != null)
		{
			slot.gameObject.GetComponent<KAnimControllerBase>().AddBuildOverride(this.def.BuildOverride, true);
		}
	}

	public void OnUnequip(EquipmentSlotInstance slot)
	{
		if (this.def.BuildOverride != null)
		{
			slot.gameObject.GetComponent<KAnimControllerBase>().ClearBuildOverride(this.def.BuildOverride, true);
		}
		Attributes attributes = slot.gameObject.GetAttributes();
		foreach (AttributeModifier attributeModifier in this.def.AttributeModifiers)
		{
			attributes.Remove(attributeModifier);
		}
		slot.gameObject.GetComponent<SnapOn>().DetachSnapOnByName(this.def.SnapOn);
		slot.gameObject.GetComponent<Navigator>().ClearAbilityFlag(this.def.PathFinderFlags);
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(GameObject go)
	{
		return null;
	}

	public List<string> GetEffectDescriptions(GameObject go)
	{
		return GameUtil.GetEquipmentEffects(this.def);
	}

	public EquipmentDef def;

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private Ref<Equipment> assignablesRef = new Ref<Equipment>();

	private static readonly string[] WorkAnims = new string[] { "working_pre", "working_loop" };
}
