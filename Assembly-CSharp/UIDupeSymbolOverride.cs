using System;
using Database;
using UnityEngine;

[RequireComponent(typeof(SymbolOverrideController))]
public class UIDupeSymbolOverride : MonoBehaviour
{
	public void Apply(MinionIdentity minionIdentity)
	{
		if (this.slots == null)
		{
			this.slots = new AccessorySlots(null);
		}
		if (this.symbolOverrideController == null)
		{
			this.symbolOverrideController = base.GetComponent<SymbolOverrideController>();
		}
		if (this.animController == null)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
		}
		Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(minionIdentity.nameStringKey);
		DebugUtil.DevAssert(personalityFromNameStringKey != null, "Personality is not found", null);
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personalityFromNameStringKey);
		this.symbolOverrideController.RemoveAllSymbolOverrides(0);
		this.SetAccessory(this.animController, this.slots.Hair.Lookup(bodyData.hair));
		this.SetAccessory(this.animController, this.slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)));
		this.SetAccessory(this.animController, this.slots.Eyes.Lookup(bodyData.eyes));
		this.SetAccessory(this.animController, this.slots.HeadShape.Lookup(bodyData.headShape));
		this.SetAccessory(this.animController, this.slots.Mouth.Lookup(bodyData.mouth));
		this.SetAccessory(this.animController, this.slots.Neck.Lookup(bodyData.neck));
		this.SetAccessory(this.animController, this.slots.Body.Lookup(bodyData.body));
		this.SetAccessory(this.animController, this.slots.Leg.Lookup(bodyData.legs));
		this.SetAccessory(this.animController, this.slots.Arm.Lookup(bodyData.arms));
		this.SetAccessory(this.animController, this.slots.ArmLower.Lookup(bodyData.armslower));
		this.SetAccessory(this.animController, this.slots.Pelvis.Lookup(bodyData.pelvis));
		this.SetAccessory(this.animController, this.slots.Belt.Lookup(bodyData.belt));
		this.SetAccessory(this.animController, this.slots.Foot.Lookup(bodyData.foot));
		this.SetAccessory(this.animController, this.slots.Cuff.Lookup(bodyData.cuff));
		this.SetAccessory(this.animController, this.slots.Hand.Lookup(bodyData.hand));
	}

	private KAnimHashedString SetAccessory(KBatchedAnimController minion, Accessory accessory)
	{
		if (accessory != null)
		{
			this.symbolOverrideController.TryRemoveSymbolOverride(accessory.slot.targetSymbolId, 0);
			this.symbolOverrideController.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol, 0);
			minion.SetSymbolVisiblity(accessory.slot.targetSymbolId, true);
			return accessory.slot.targetSymbolId;
		}
		return HashedString.Invalid;
	}

	private KBatchedAnimController animController;

	private AccessorySlots slots;

	private SymbolOverrideController symbolOverrideController;
}
