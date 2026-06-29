using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor, IQuality
{
	public global::QualityLevel GetQuality()
	{
		return this.quality;
	}

	public void SetQuality(global::QualityLevel level)
	{
		this.quality = level;
	}

	protected override void OnPrefabInit()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		KPrefabID originalPrefab = component.GetOriginalPrefab();
		Equippable component2 = originalPrefab.GetComponent<Equippable>();
		this.def = component2.def;
		base.OnPrefabInit();
		if (this.def.AdditionalTags != null)
		{
			foreach (Tag tag in this.def.AdditionalTags)
			{
				base.GetComponent<KPrefabID>().AddTag(tag);
			}
		}
	}

	protected override void OnSpawn()
	{
		if (this.isEquipped)
		{
			if (this.assignee != null)
			{
				this.assignee.GetSoleOwner().GetComponent<Equipment>().Equip(this);
			}
			else
			{
				global::Debug.LogWarning("Equippable trying to be equipped to missing prefab", null);
				this.isEquipped = false;
			}
		}
		base.Subscribe(1969584890, delegate(object o)
		{
			this.destroyed = true;
		});
	}

	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == this.assignee)
		{
			return;
		}
		if (new_assignee is MinionIdentity && base.slot != null && new_assignee.GetSoleOwner().GetComponent<Equipment>().GetSlot(base.slot)
			.assignable != null)
		{
			new_assignee.GetSoleOwner().GetComponent<Equipment>().GetSlot(base.slot)
				.assignable.Unassign();
		}
		base.Assign(new_assignee);
	}

	public override void Unassign()
	{
		if (this.isEquipped)
		{
			(this.assignee as MinionIdentity).GetComponent<Equipment>().Unequip(this);
			this.OnUnequip();
		}
		base.Unassign();
	}

	public void OnEquip(AssignableSlotInstance slot)
	{
		this.isEquipped = true;
		if (SelectTool.Instance.selected == this.selectable)
		{
			SelectTool.Instance.Select(null, false);
		}
		base.GetComponent<KBatchedAnimController>().enabled = false;
		base.GetComponent<KSelectable>().IsSelectable = false;
		base.transform.parent = slot.gameObject.transform;
		base.transform.SetLocalPosition(Vector3.zero);
		Effects component = slot.gameObject.GetComponent<Effects>();
		foreach (Effect effect in this.def.EffectImmunites)
		{
			component.AddImmunity(effect);
		}
		if (this.def.OnEquipCallBack != null)
		{
			this.def.OnEquipCallBack(this);
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Equipped);
	}

	public void OnUnequip()
	{
		this.isEquipped = false;
		if (this.destroyed)
		{
			return;
		}
		base.GetComponent<KPrefabID>().RemoveTag(GameTags.Equipped);
		base.GetComponent<KBatchedAnimController>().enabled = true;
		base.GetComponent<KSelectable>().IsSelectable = true;
		Effects component = this.assignee.GetSoleOwner().GetComponent<Effects>();
		foreach (Effect effect in this.def.EffectImmunites)
		{
			component.RemoveImmunity(effect);
		}
		base.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Misc).transform;
		base.gameObject.transform.SetPosition(this.assignee.GetSoleOwner().gameObject.transform.GetPosition() + Vector3.up / 2f);
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

	private global::QualityLevel quality;

	[MyCmpAdd]
	private EquippableWorkable equippableWorkable;

	[MyCmpReq]
	private KSelectable selectable;

	public EquipmentDef def;

	[Serialize]
	public bool isEquipped;

	private bool destroyed;
}
