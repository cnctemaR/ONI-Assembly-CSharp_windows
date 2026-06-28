using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor, IQuality
{
	protected override Assignables GetAssignables()
	{
		return this.assignablesRef.Get();
	}

	protected override void SetAssignables(Assignables assignables)
	{
		this.assignablesRef.Set((Equipment)assignables);
	}

	protected override void OnPrefabInit()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		KPrefabID originalPrefab = component.GetOriginalPrefab();
		Equippable component2 = originalPrefab.GetComponent<Equippable>();
		this.def = component2.def;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip_clothing_kanim") };
		this.forcePlayPst = true;
		base.OnPrefabInit();
		if (this.def.AdditionalTags != null)
		{
			foreach (Tag tag in this.def.AdditionalTags)
			{
				base.GetComponent<KPrefabID>().AddTag(tag);
			}
		}
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
		base.OnAssign += this.RefreshChore;
		base.Subscribe(1969584890, delegate(object o)
		{
			this.destroyed = true;
		});
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

	private void CreateChore()
	{
		this.chore = new WorkChore<Equippable>(Db.Get().ChoreTypes.Equip, this, this.assignee.GetSoleOwner().GetComponent<ChoreProvider>(), true, null, null, null, true, null, true, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
	}

	public override Assignables GetAssignables(GameObject go)
	{
		return go.GetComponent<Equipment>();
	}

	public void CancelChore()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Manual equip");
			this.chore = null;
		}
	}

	private void RefreshChore(IAssignableIdentity target)
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Equipment Reassigned");
			this.chore = null;
		}
		if (target != null && !target.GetSoleOwner().GetComponent<Equipment>().IsEquipped(this))
		{
			this.CreateChore();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (this.assignee != null)
		{
			this.assignee.GetSoleOwner().GetComponent<Equipment>().Equip(this);
		}
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
		base.GetComponent<Pickupable>().UnregisterListeners();
		base.transform.parent = slot.gameObject.transform;
		base.transform.localPosition = Vector3.zero;
		Effects component = slot.gameObject.GetComponent<Effects>();
		foreach (Effect effect in this.def.EffectImmunites)
		{
			component.AddImmunity(effect);
		}
		if (this.def.OnEquipCallBack != null)
		{
			this.def.OnEquipCallBack(this);
		}
	}

	public void OnUnequip()
	{
		this.isEquipped = false;
		if (this.destroyed)
		{
			return;
		}
		base.GetComponent<KBatchedAnimController>().enabled = true;
		base.GetComponent<KSelectable>().IsSelectable = true;
		base.GetComponent<Pickupable>().RegisterListeners();
		Effects component = this.assignee.GetSoleOwner().GetComponent<Effects>();
		foreach (Effect effect in this.def.EffectImmunites)
		{
			component.RemoveImmunity(effect);
		}
		base.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Misc).transform;
		base.gameObject.transform.SetPosition(this.assignee.GetSoleOwner().gameObject.transform.position + Vector3.up / 2f);
		if (this.def.OnUnequipCallBack != null)
		{
			this.def.OnUnequipCallBack(this);
		}
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
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

	private bool destroyed;

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private Ref<Equipment> assignablesRef = new Ref<Equipment>();

	[Serialize]
	public bool isEquipped;

	private global::QualityLevel quality;
}
