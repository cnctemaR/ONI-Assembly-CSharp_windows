using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

public class Accessorizer : KMonoBehaviour
{
	public List<ResourceRef<Accessory>> GetAccessories()
	{
		return this.accessories;
	}

	public void SetAccessories(List<ResourceRef<Accessory>> data)
	{
		this.accessories = data;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		this.ApplyAccessories();
	}

	public void AddAccessory(Accessory accessory)
	{
		SymbolOverrideController component = this.animController.GetComponent<SymbolOverrideController>();
		component.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol, 0);
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
		SymbolOverrideController component = this.animController.GetComponent<SymbolOverrideController>();
		component.TryRemoveSymbolOverride(accessory.slot.targetSymbolId, 0);
	}

	public void ApplyAccessories()
	{
		foreach (ResourceRef<Accessory> resourceRef in this.accessories)
		{
			Accessory accessory = resourceRef.Get();
			if (accessory != null)
			{
				this.AddAccessory(accessory);
			}
		}
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

	[Serialize]
	private List<ResourceRef<Accessory>> accessories = new List<ResourceRef<Accessory>>();

	[MyCmpReq]
	private KAnimControllerBase animController;
}
