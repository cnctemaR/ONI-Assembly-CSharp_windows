using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class Accessorizer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.RefreshAccessories();
	}

	public void AddAccessory(Accessory accessory)
	{
		this.animController.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.batchSource, accessory.symbol, false);
		if (!this.HasAccessory(accessory))
		{
			ResourceRef<Accessory> resourceRef = new ResourceRef<Accessory>(accessory);
			if (resourceRef != null)
			{
				this.accessories.Add(resourceRef);
			}
		}
	}

	public void RemoveAccessory(Accessory accessory)
	{
		this.accessories.RemoveAll((ResourceRef<Accessory> x) => x.Get() == accessory);
		this.animController.RemoveSymbolOverride(accessory.slot.targetSymbolId);
	}

	public bool HasAccessory(Accessory accessory)
	{
		return this.accessories.Exists((ResourceRef<Accessory> x) => x.Get() == accessory);
	}

	public Accessory GetAccessory(AccessorySlot slot)
	{
		for (int i = 0; i < this.accessories.Count; i++)
		{
			if (this.accessories[i].Get() != null)
			{
				if (this.accessories[i].Get().slot == slot)
				{
					return this.accessories[i].Get();
				}
			}
		}
		return null;
	}

	public void GetBodySlots(ref KCompBuilder.BodyData fd)
	{
		fd.eyes = HashedString.Invalid;
		fd.hair = HashedString.Invalid;
		fd.headShape = HashedString.Invalid;
		fd.mouth = HashedString.Invalid;
		fd.neck = HashedString.Invalid;
		fd.body = HashedString.Invalid;
		fd.arms = HashedString.Invalid;
		fd.hat = HashedString.Invalid;
		fd.hatHair = HashedString.Invalid;
		for (int i = 0; i < this.accessories.Count; i++)
		{
			Accessory accessory = this.accessories[i].Get();
			if (accessory != null)
			{
				if (accessory.slot.Id == "Eyes")
				{
					fd.eyes = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Hair")
				{
					fd.hair = accessory.IdHash;
					fd.hatHair = "hat_" + accessory.Id;
				}
				else if (accessory.slot.Id == "HeadShape")
				{
					fd.headShape = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Mouth")
				{
					fd.mouth = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Neck")
				{
					fd.neck = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Body")
				{
					fd.body = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Arm")
				{
					fd.arms = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Hat")
				{
					fd.hat = HashedString.Invalid;
				}
			}
		}
	}

	public void ApplyAccessories(KAnimControllerBase controller)
	{
	}

	[ContextMenu("Refresh Accessories")]
	public void RefreshAccessories()
	{
		this.ApplyAccessories(this.animController);
	}

	[Serialize]
	private List<ResourceRef<Accessory>> accessories = new List<ResourceRef<Accessory>>();

	[MyCmpReq]
	private KAnimControllerBase animController;
}
