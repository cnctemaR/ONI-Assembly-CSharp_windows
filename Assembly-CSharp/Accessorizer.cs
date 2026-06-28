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
		this.animController.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.batchSource, accessory.symbol);
		this.animController.ShowSymbol(accessory.slot.targetSymbolId);
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

	public void GetFaceSlots(ref Accessorizer.FaceData fd)
	{
		fd.eyes = 0;
		fd.hair = 0;
		fd.headShape = 0;
		fd.mouth = 0;
		fd.neck = 0;
		for (int i = 0; i < this.accessories.Count; i++)
		{
			Accessory accessory = this.accessories[i].Get();
			if (accessory != null)
			{
				if (accessory.slot.Id == "Eyes")
				{
					fd.eyes = accessory.subtype;
				}
				else if (accessory.slot.Id == "Hair")
				{
					fd.hair = accessory.subtype;
				}
				else if (accessory.slot.Id == "HeadShape")
				{
					fd.headShape = accessory.subtype;
				}
				else if (accessory.slot.Id == "Mouth")
				{
					fd.mouth = accessory.subtype;
				}
				else if (accessory.slot.Id == "Neck")
				{
					fd.neck = accessory.subtype;
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

	[Serializable]
	public struct FaceData
	{
		public int headShape;

		public int mouth;

		public int neck;

		public int eyes;

		public int hair;
	}
}
