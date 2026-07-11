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
			this.GetNewBody(i);
		}
	}

	protected void GetNewBody(int minion_idx)
	{
		int num = global::UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
		Personality personality = Db.Get().Personalities[num];
		foreach (KBatchedAnimController kbatchedAnimController in this.anims[minion_idx].minions)
		{
			this.Apply(kbatchedAnimController, personality);
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
		if (this.applySuit && global::UnityEngine.Random.value < 0.15f)
		{
			component.AddBuildOverride(Assets.GetAnim("body_oxygen_kanim").GetData(), 6);
			component.AddBuildOverride(Assets.GetAnim("helm_oxygen_kanim").GetData(), 6);
			dupe.SetSymbolVisiblity("snapto_neck", true);
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
				list.Add(skill.hat);
			}
			string text = list[global::UnityEngine.Random.Range(0, list.Count)];
			UIDupeRandomizer.AddAccessory(dupe, this.slots.Hat.Lookup(text));
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
			return;
		}
		dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
		dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
		dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, false);
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

	protected virtual void Update()
	{
	}

	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	public bool applyHat = true;

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
