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

	private void Apply(KBatchedAnimController minon, ref UIDupeRandomizer.AnimChoice anim)
	{
		int num = global::UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
		Personality personality = Db.Get().Personalities[num];
		if (anim.curHair.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHair);
		}
		anim.curHair = UIDupeRandomizer.AddAccessory(minon, this.slots.Hair.accessories[personality.hair]);
		if (anim.curEyes.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curEyes);
		}
		anim.curEyes = UIDupeRandomizer.AddAccessory(minon, this.slots.Eyes.accessories[personality.eyes]);
		if (anim.curHeadShape.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHeadShape);
		}
		anim.curHeadShape = UIDupeRandomizer.AddAccessory(minon, this.slots.HeadShape.accessories[personality.headShape]);
		if (anim.curMouth.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curMouth);
		}
		anim.curMouth = UIDupeRandomizer.AddAccessory(minon, this.slots.Mouth.accessories[personality.mouth]);
		if (anim.curTorso.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curTorso);
			minon.RemoveSymbolOverride(anim.curArm);
		}
		int body = personality.body;
		anim.curTorso = UIDupeRandomizer.AddAccessory(minon, this.slots.Body.accessories[body]);
		anim.curArm = UIDupeRandomizer.AddAccessory(minon, this.slots.Arm.accessories[body]);
		if (global::UnityEngine.Random.value < 0.1f)
		{
			KAnimFile anim2 = Assets.GetAnim("body_oxygen_kanim");
			minon.AddBuildOverride(anim2, true, false);
			KAnimFile anim3 = Assets.GetAnim("helm_oxygen_kanim");
			minon.AddBuildOverride(anim3, true, false);
		}
		if (!anim.overrideSet)
		{
			minon.AddAnimOverrides(anim.target_minion_anim, 0f);
			anim.overrideSet = true;
		}
		minon.UpdateSymbolLookups();
	}

	public static KAnimHashedString AddAccessory(KBatchedAnimController minon, Accessory accessory)
	{
		minon.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol.build.batchTag, accessory.symbol, false);
		minon.ShowSymbol(accessory.slot.targetSymbolId);
		return accessory.slot.targetSymbolId;
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

		public KAnimHashedString curEyes;

		public KAnimHashedString curHeadShape;

		public KAnimHashedString curMouth;

		public KAnimHashedString curTorso;

		public KAnimHashedString curArm;
	}
}
