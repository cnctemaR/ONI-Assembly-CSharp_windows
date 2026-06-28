using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadable, IQuality, IGameObjectEffectDescriptor
{
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
		Debug.Assert(this.def != null, "Cant continue without a def");
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip_clothing_kanim") };
		this.forcePlayPst = true;
		base.OnPrefabInit();
	}

	public global::QualityLevel GetQuality()
	{
		return this.quality;
	}

	public void SetQuality(global::QualityLevel level)
	{
		this.quality = level;
	}

	protected override void OnSpawn()
	{
		base.SetWorkTime(1.5f);
		if (this.equipment != null && !this.equipment.IsEquipped(this))
		{
			this.CreateChore();
		}
	}

	private void CreateChore()
	{
		if (this.equipment == null)
		{
			Debug.LogFormat("Looks like we already assigned this [{0}/{1}]", new object[]
			{
				base.name,
				base.gameObject.GetInstanceID()
			});
			return;
		}
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

	protected override void OnClickAssign(Assignables new_assignables)
	{
		if (this.equipment == null)
		{
			this.equipment = base.GetComponent<Equipment>();
		}
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
			if (SelectTool.Instance.selected == this.selectable)
			{
				SelectTool.Instance.Select(null, false);
			}
			this.equipment.Equip(this);
		}
	}

	public void OnEquip(EquipmentSlotInstance slot)
	{
		KBatchedAnimController component = slot.gameObject.GetComponent<KBatchedAnimController>();
		Attributes attributes = slot.gameObject.GetAttributes();
		string name = base.GetComponent<KSelectable>().GetName();
		foreach (AttributeModifier attributeModifier in this.def.AttributeModifiers)
		{
			attributes.Add(name, attributeModifier);
		}
		SnapOn component2 = slot.gameObject.GetComponent<SnapOn>();
		component2.AttachSnapOnByName(this.def.SnapOn);
		if (this.def.SnapOn1 != null)
		{
			component2.AttachSnapOnByName(this.def.SnapOn1);
		}
		slot.gameObject.GetComponent<Navigator>().SetAbilityFlag(this.def.PathFinderFlags);
		if (this.def.BuildOverride != null)
		{
			component.AddBuildOverride(this.def.BuildOverride, true, false);
		}
		if (this.def.OnEquipCallBack != null)
		{
			this.def.OnEquipCallBack(this);
		}
	}

	public void OnUnequip(EquipmentSlotInstance slot)
	{
		KBatchedAnimController component = slot.gameObject.GetComponent<KBatchedAnimController>();
		if (this.def.BuildOverride != null)
		{
			component.ClearBuildOverride(this.def.BuildOverride, !this.def.IsBody);
		}
		Attributes attributes = slot.gameObject.GetAttributes();
		foreach (AttributeModifier attributeModifier in this.def.AttributeModifiers)
		{
			attributes.Remove(attributeModifier);
		}
		if (!this.def.IsBody)
		{
			SnapOn component2 = slot.gameObject.GetComponent<SnapOn>();
			component2.DetachSnapOnByName(this.def.SnapOn);
			if (this.def.SnapOn1 != null)
			{
				component2.DetachSnapOnByName(this.def.SnapOn1);
			}
		}
		slot.gameObject.GetComponent<Navigator>().ClearAbilityFlag(this.def.PathFinderFlags);
		if (this.def.OnUnequipCallBack != null)
		{
			this.def.OnUnequipCallBack(this);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		if (this.def != null)
		{
			List<Descriptor> equipmentEffects = GameUtil.GetEquipmentEffects(this.def);
			if (this.def.additionalDescriptors != null)
			{
				foreach (Descriptor descriptor in this.def.additionalDescriptors)
				{
					equipmentEffects.Add(descriptor);
				}
			}
			return equipmentEffects;
		}
		return new List<Descriptor>();
	}

	public EquipmentDef def;

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private Ref<Equipment> assignablesRef = new Ref<Equipment>();

	private global::QualityLevel quality;
}
