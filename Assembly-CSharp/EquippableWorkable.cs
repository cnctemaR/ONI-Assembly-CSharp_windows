using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/EquippableWorkable")]
public class EquippableWorkable : Workable, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip_clothing_kanim") };
		this.synchronizeAnims = false;
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
		this.equippable.OnAssign += this.RefreshChore;
		Prioritizable.AddRef(base.gameObject);
	}

	private void CreateChore()
	{
		global::Debug.Assert(this.chore == null, "chore should be null");
		this.chore = new EquipChore(this);
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
		if (target != null && !target.GetSoleOwner().GetComponent<Equipment>().IsEquipped(this.equippable))
		{
			this.CreateChore();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (this.equippable.assignee != null)
		{
			Ownables soleOwner = this.equippable.assignee.GetSoleOwner();
			if (soleOwner)
			{
				soleOwner.GetComponent<Equipment>().Equip(this.equippable);
			}
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		this.workTimeRemaining = this.GetWorkTime();
		base.OnStopWork(worker);
		Prioritizable.RemoveRef(base.gameObject);
	}

	[MyCmpReq]
	private Equippable equippable;

	private Chore chore;

	private global::QualityLevel quality;
}
