using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public class EquippableBalloonConfig : IEquipmentConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public EquipmentDef CreateEquipmentDef()
	{
		List<AttributeModifier> list = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("EquippableBalloon", EQUIPMENT.TOYS.SLOT, SimHashes.Carbon, EQUIPMENT.TOYS.BALLOON_MASS, EQUIPMENT.VESTS.WARM_VEST_ICON0, null, null, 0, list, null, false, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f, null, null);
		equipmentDef.OnEquipCallBack = new Action<Equippable>(this.OnEquipBalloon);
		equipmentDef.OnUnequipCallBack = new Action<Equippable>(this.OnUnequipBalloon);
		return equipmentDef;
	}

	private void OnEquipBalloon(Equippable eq)
	{
		if (!eq.IsNullOrDestroyed() && !eq.assignee.IsNullOrDestroyed())
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner.IsNullOrDestroyed())
			{
				return;
			}
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)soleOwner.GetComponent<MinionAssignablesProxy>().target;
			Effects component = kmonoBehaviour.GetComponent<Effects>();
			KSelectable component2 = kmonoBehaviour.GetComponent<KSelectable>();
			if (!component.IsNullOrDestroyed())
			{
				component.Add("HasBalloon", false);
				EquippableBalloon component3 = eq.GetComponent<EquippableBalloon>();
				EquippableBalloon.StatesInstance statesInstance = (EquippableBalloon.StatesInstance)component3.GetSMI();
				component2.AddStatusItem(Db.Get().DuplicantStatusItems.JoyResponse_HasBalloon, statesInstance);
				this.SpawnFxInstanceFor(kmonoBehaviour);
				component3.ApplyBalloonOverrideToBalloonFx();
			}
		}
	}

	private void OnUnequipBalloon(Equippable eq)
	{
		if (!eq.IsNullOrDestroyed() && !eq.assignee.IsNullOrDestroyed())
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner.IsNullOrDestroyed())
			{
				return;
			}
			MinionAssignablesProxy component = soleOwner.GetComponent<MinionAssignablesProxy>();
			if (!component.target.IsNullOrDestroyed())
			{
				KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)component.target;
				Effects component2 = kmonoBehaviour.GetComponent<Effects>();
				KSelectable component3 = kmonoBehaviour.GetComponent<KSelectable>();
				if (!component2.IsNullOrDestroyed())
				{
					component2.Remove("HasBalloon");
					component3.RemoveStatusItem(Db.Get().DuplicantStatusItems.JoyResponse_HasBalloon, false);
					this.DestroyFxInstanceFor(kmonoBehaviour);
				}
			}
		}
		Util.KDestroyGameObject(eq.gameObject);
	}

	public void DoPostConfigure(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
		Equippable equippable = go.GetComponent<Equippable>();
		if (equippable.IsNullOrDestroyed())
		{
			equippable = go.AddComponent<Equippable>();
		}
		equippable.hideInCodex = true;
		equippable.unequippable = false;
		go.AddOrGet<EquippableBalloon>();
	}

	private void SpawnFxInstanceFor(KMonoBehaviour target)
	{
		new BalloonFX.Instance(target.GetComponent<KMonoBehaviour>()).StartSM();
	}

	private void DestroyFxInstanceFor(KMonoBehaviour target)
	{
		target.GetSMI<BalloonFX.Instance>().StopSM("Unequipped");
	}

	public const string ID = "EquippableBalloon";
}
