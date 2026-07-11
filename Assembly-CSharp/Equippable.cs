using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor, IQuality, IHasSortOrder
{
	public global::QualityLevel GetQuality()
	{
		return this.quality;
	}

	public void SetQuality(global::QualityLevel level)
	{
		this.quality = level;
	}

	public EquipmentDef def
	{
		get
		{
			return this.defHandle.Get<EquipmentDef>();
		}
		set
		{
			this.defHandle.Set<EquipmentDef>(value);
		}
	}

	public int sortOrder { get; set; }

	protected override void OnPrefabInit()
	{
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
		base.Subscribe<Equippable>(1969584890, Equippable.SetDestroyedTrueDelegate);
	}

	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee == this.assignee)
		{
			return;
		}
		if (base.slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity))
		{
			Equipment component = new_assignee.GetSoleOwner().GetComponent<Equipment>();
			AssignableSlotInstance slot = component.GetSlot(base.slot);
			if (slot.assignable != null)
			{
				slot.Unassign(true);
			}
		}
		base.Assign(new_assignee);
	}

	public override void Unassign()
	{
		if (this.isEquipped)
		{
			(this.assignee as KMonoBehaviour).GetComponent<Equipment>().Unequip(this);
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
		if (component != null)
		{
			foreach (Effect effect in this.def.EffectImmunites)
			{
				component.AddImmunity(effect);
			}
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
		if (this.assignee != null)
		{
			Effects component = this.assignee.GetSoleOwner().GetComponent<Effects>();
			if (component != null)
			{
				foreach (Effect effect in this.def.EffectImmunites)
				{
					component.RemoveImmunity(effect);
				}
			}
			base.gameObject.transform.SetPosition(this.assignee.GetSoleOwner().gameObject.transform.GetPosition() + Vector3.up / 2f);
		}
		base.transform.parent = null;
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

	public DefHandle defHandle;

	[Serialize]
	public bool isEquipped;

	private bool destroyed;

	private static readonly EventSystem.IntraObjectHandler<Equippable> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equippable>(delegate(Equippable component, object data)
	{
		component.destroyed = true;
	});
}
