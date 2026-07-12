using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ElectrobankTracker")]
public class ElectrobankTracker : WorldResourceAmountTracker<ElectrobankTracker>, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ignoredTags = new Tag[GameTags.BionicIncompatibleBatteries.Count];
		GameTags.BionicIncompatibleBatteries.CopyTo(this.ignoredTags, 0);
		this.itemTag = GameTags.ChargedPortableBattery;
	}

	protected override WorldResourceAmountTracker<ElectrobankTracker>.ItemData GetItemData(Pickupable item)
	{
		Electrobank component = item.GetComponent<Electrobank>();
		return new WorldResourceAmountTracker<ElectrobankTracker>.ItemData
		{
			ID = component.ID,
			amountValue = component.Charge * item.PrimaryElement.Units,
			units = item.PrimaryElement.Units
		};
	}
}
