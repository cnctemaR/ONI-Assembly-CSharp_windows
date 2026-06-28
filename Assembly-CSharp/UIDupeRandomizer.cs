using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class UIDupeRandomizer : MonoBehaviour
{
	protected virtual void Start()
	{
		this.slots = new AccessorySlots(null, this.head_default_anim, this.head_swap_anim, this.body_swap_anim);
		for (int i = 0; i < this.anims.Length; i++)
		{
			this.anims[i].curBody = null;
			this.anims[i].overrideSet = false;
			this.GetNewBody(i);
		}
	}

	protected void GetNewBody(int minonIdx)
	{
		this.Apply(this.anims[minonIdx].minon, ref this.anims[minonIdx]);
	}

	private void Apply(KBatchedAnimController dupe, ref UIDupeRandomizer.AnimChoice anim)
	{
		int num = global::UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
		Personality personality = Db.Get().Personalities[num];
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
		if (anim.curHair.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curHair);
		}
		anim.curHair = UIDupeRandomizer.AddAccessory(dupe, this.slots.Hair.Lookup(bodyData.hair));
		if (anim.curHatHair.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curHatHair);
		}
		anim.curHatHair = UIDupeRandomizer.AddAccessory(dupe, this.slots.HatHair.Lookup(bodyData.hatHair));
		if (anim.curEyes.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curEyes);
		}
		anim.curEyes = UIDupeRandomizer.AddAccessory(dupe, this.slots.Eyes.Lookup(bodyData.eyes));
		if (anim.curHeadShape.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curHeadShape);
		}
		anim.curHeadShape = UIDupeRandomizer.AddAccessory(dupe, this.slots.HeadShape.Lookup(bodyData.headShape));
		if (anim.curMouth.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curMouth);
		}
		anim.curMouth = UIDupeRandomizer.AddAccessory(dupe, this.slots.Mouth.Lookup(bodyData.mouth));
		if (anim.curTorso.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curTorso);
			dupe.RemoveSymbolOverride(anim.curArm);
		}
		anim.curTorso = UIDupeRandomizer.AddAccessory(dupe, this.slots.Body.Lookup(bodyData.body));
		anim.curArm = UIDupeRandomizer.AddAccessory(dupe, this.slots.Arm.Lookup(bodyData.arms));
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> keyValuePair in RoleManager.roleHatIndex)
		{
			list.Add(keyValuePair.Value);
		}
		string text = list[global::UnityEngine.Random.Range(0, list.Count)];
		if (anim.curHat.IsValid())
		{
			dupe.RemoveSymbolOverride(anim.curHat);
		}
		anim.curHat = UIDupeRandomizer.AddAccessory(dupe, this.slots.Hat.Lookup(text));
		dupe.RemoveVisibleSymbol(Db.Get().AccessorySlots.Hair.targetSymbolId);
		dupe.HideSymbol(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
		dupe.ShowSymbol(Db.Get().AccessorySlots.HatHair.targetSymbolId);
		dupe.StopHidingSymbol(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
		if (global::UnityEngine.Random.value < 0.1f)
		{
			KAnimFile anim2 = Assets.GetAnim("body_oxygen_kanim");
			dupe.AddBuildOverride(anim2, true, false);
			KAnimFile anim3 = Assets.GetAnim("helm_oxygen_kanim");
			dupe.AddBuildOverride(anim3, true, false);
		}
		else
		{
			KAnimFile anim4 = Assets.GetAnim("body_oxygen_kanim");
			dupe.ClearBuildOverride(anim4, false);
			KAnimFile anim5 = Assets.GetAnim("helm_oxygen_kanim");
			dupe.ClearBuildOverride(anim5, true);
		}
		if (!anim.overrideSet)
		{
			dupe.AddAnimOverrides(anim.target_minion_anim, 0f);
			anim.overrideSet = true;
		}
		dupe.UpdateSymbolLookups();
	}

	public static KAnimHashedString AddAccessory(KBatchedAnimController minon, Accessory accessory)
	{
		if (accessory != null)
		{
			minon.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol.build.batchTag, accessory.symbol, false);
			minon.ShowSymbol(accessory.slot.targetSymbolId);
			return accessory.slot.targetSymbolId;
		}
		return HashedString.Invalid;
	}

	public KAnimHashedString AddRandomAccessory(KBatchedAnimController minon, List<Accessory> choices)
	{
		Accessory accessory = choices[global::UnityEngine.Random.Range(1, choices.Count)];
		return UIDupeRandomizer.AddAccessory(minon, accessory);
	}

	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	public UIDupeRandomizer.AnimChoice[] anims;

	private AccessorySlots slots;

	[Serializable]
	public struct AnimChoice
	{
		public string anim_name;

		public KBatchedAnimController minon;

		public float minSecondsBetweenAction;

		public float maxSecondsBetweenAction;

		public float lastWaitTime;

		public KAnimFile curBody;

		public KAnimFile target_minion_anim;

		public bool overrideSet;

		public KAnimHashedString curHair;

		public KAnimHashedString curHatHair;

		public KAnimHashedString curEyes;

		public KAnimHashedString curHeadShape;

		public KAnimHashedString curMouth;

		public KAnimHashedString curTorso;

		public KAnimHashedString curArm;

		public KAnimHashedString curHat;
	}
}
