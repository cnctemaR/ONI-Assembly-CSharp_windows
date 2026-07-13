using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class UIDupeRandomizer : MonoBehaviour
{
	protected virtual void Start()
	{
		this.slots = Db.Get().AccessorySlots;
		for (int i = 0; i < this.anims.Length; i++)
		{
			this.anims[i].curBody = null;
			this.GetNewBody(i);
		}
	}

	protected void GetNewBody(int minion_idx)
	{
		Personality random = Db.Get().Personalities.GetRandom(true, false);
		foreach (KBatchedAnimController kbatchedAnimController in this.anims[minion_idx].minions)
		{
			this.Apply(kbatchedAnimController, random);
		}
	}

	private void Apply(KBatchedAnimController dupe, Personality personality)
	{
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
		SymbolOverrideController component = dupe.GetComponent<SymbolOverrideController>();
		component.RemoveAllSymbolOverrides(0);
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Hair.Lookup(bodyData.hair));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Eyes.Lookup(bodyData.eyes));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.HeadShape.Lookup(bodyData.headShape));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Mouth.Lookup(bodyData.mouth));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Body.Lookup(bodyData.body));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Arm.Lookup(bodyData.arms));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.ArmLower.Lookup(bodyData.armslower));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Belt.Lookup(bodyData.belt));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Hand.Lookup(bodyData.hand));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Neck.Lookup(bodyData.neck));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Cuff.Lookup(bodyData.cuff));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Pelvis.Lookup(bodyData.pelvis));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Leg.Lookup(bodyData.legs));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.Foot.Lookup(bodyData.foot));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.ArmLowerSkin.Lookup(bodyData.armLowerSkin));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.ArmUpperSkin.Lookup(bodyData.armUpperSkin));
		UIDupeRandomizer.AddAccessory(dupe, this.slots.LegSkin.Lookup(bodyData.legSkin));
		if (this.applySuit && global::UnityEngine.Random.value < 0.15f)
		{
			component.AddBuildOverride(Assets.GetAnim("body_oxygen_kanim").GetData(), 6);
			dupe.SetSymbolVisiblity("snapto_neck", true);
			dupe.SetSymbolVisiblity("belt", false);
		}
		else
		{
			dupe.SetSymbolVisiblity("snapto_neck", false);
		}
		if (this.applyHat && global::UnityEngine.Random.value < 0.5f)
		{
			List<string> list = new List<string>();
			foreach (Skill skill in Db.Get().Skills.resources)
			{
				if (skill.requiredDuplicantModel.IsNullOrWhiteSpace() || skill.requiredDuplicantModel == personality.model)
				{
					list.Add(skill.hat);
				}
			}
			string text = list[global::UnityEngine.Random.Range(0, list.Count)];
			UIDupeRandomizer.AddAccessory(dupe, this.slots.Hat.Lookup(text));
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
		}
		else
		{
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, false);
		}
		dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Skirt.targetSymbolId, false);
		dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Necklace.targetSymbolId, false);
	}

	public static KAnimHashedString AddAccessory(KBatchedAnimController minion, Accessory accessory)
	{
		if (accessory != null)
		{
			SymbolOverrideController component = minion.GetComponent<SymbolOverrideController>();
			DebugUtil.Assert(component != null, minion.name + " is missing symbol override controller");
			component.TryRemoveSymbolOverride(accessory.slot.targetSymbolId, 0);
			component.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol, 0);
			minion.SetSymbolVisiblity(accessory.slot.targetSymbolId, true);
			return accessory.slot.targetSymbolId;
		}
		return HashedString.Invalid;
	}

	public KAnimHashedString AddRandomAccessory(KBatchedAnimController minion, List<Accessory> choices)
	{
		Accessory accessory = choices[global::UnityEngine.Random.Range(1, choices.Count)];
		return UIDupeRandomizer.AddAccessory(minion, accessory);
	}

	public void Randomize()
	{
		if (this.slots == null)
		{
			return;
		}
		for (int i = 0; i < this.anims.Length; i++)
		{
			this.GetNewBody(i);
		}
	}

	protected virtual void Update()
	{
	}

	[Tooltip("Enable this to allow for a chance for skill hats to appear")]
	public bool applyHat = true;

	[Tooltip("Enable this to allow for a chance for suit helmets to appear (ie. atmosuit and leadsuit)")]
	public bool applySuit = true;

	public UIDupeRandomizer.AnimChoice[] anims;

	private AccessorySlots slots;

	[Serializable]
	public struct AnimChoice
	{
		public string anim_name;

		public List<KBatchedAnimController> minions;

		public float minSecondsBetweenAction;

		public float maxSecondsBetweenAction;

		public float lastWaitTime;

		public KAnimFile curBody;
	}
}
