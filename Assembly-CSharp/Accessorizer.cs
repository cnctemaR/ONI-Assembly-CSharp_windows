using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Accessorizer")]
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

	public KCompBuilder.BodyData bodyData { get; set; }

	public string[] GetClothingItemIds()
	{
		string[] array = new string[this.clothingItems.Count];
		for (int i = 0; i < this.clothingItems.Count; i++)
		{
			array[i] = this.clothingItems[i].Get().Id;
		}
		return array;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 30))
		{
			MinionIdentity component = base.GetComponent<MinionIdentity>();
			if (component != null)
			{
				this.bodyData = Accessorizer.UpdateAccessorySlots(component.nameStringKey, ref this.accessories);
			}
			this.accessories.RemoveAll((ResourceRef<Accessory> x) => x.Get() == null);
		}
		this.ApplyAccessories();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		MinionIdentity component = base.GetComponent<MinionIdentity>();
		if (component != null)
		{
			this.bodyData = MinionStartingStats.CreateBodyData(Db.Get().Personalities.Get(component.personalityResourceId));
		}
		base.Subscribe(-448952673, new Action<object>(this.EquippedItem));
		base.Subscribe(-1285462312, new Action<object>(this.UnequippedItem));
	}

	public void EquippedItem(object data)
	{
		KPrefabID kprefabID = data as KPrefabID;
		if (kprefabID != null && kprefabID.GetComponent<Equippable>().def.BuildOverride != null)
		{
			this.ClothingItemsDisabled = true;
			this.ClearAllItemSlots();
			this.ValidateSlots(false);
		}
	}

	private void UnequippedItem(object data)
	{
		if (this.ClothingItemsDisabled)
		{
			KPrefabID kprefabID = data as KPrefabID;
			if (kprefabID != null && kprefabID.GetComponent<Equippable>().def.BuildOverride != null)
			{
				this.ClearAllItemSlots();
				foreach (ResourceRef<ClothingItemResource> resourceRef in this.clothingItems)
				{
					this.ApplyClothingItem(resourceRef.Get());
				}
				this.ClothingItemsDisabled = false;
				this.ValidateSlots(true);
			}
		}
	}

	public void AddAccessory(Accessory accessory)
	{
		if (accessory != null)
		{
			if (this.animController == null)
			{
				this.animController = base.GetComponent<KAnimControllerBase>();
			}
			this.animController.SetSymbolVisiblity(accessory.slot.targetSymbolId, true);
			this.animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol, 0);
			if (!this.HasAccessory(accessory))
			{
				ResourceRef<Accessory> resourceRef = new ResourceRef<Accessory>(accessory);
				if (resourceRef != null)
				{
					this.accessories.Add(resourceRef);
				}
			}
		}
	}

	public void RemoveAccessory(Accessory accessory)
	{
		this.accessories.RemoveAll((ResourceRef<Accessory> x) => x.Get() == accessory);
		if (this.animController.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(accessory.slot.targetSymbolId, 0))
		{
			this.animController.SetSymbolVisiblity(accessory.slot.targetSymbolId, false);
		}
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
		foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots.resources)
		{
			if (this.GetAccessory(accessorySlot) == null)
			{
				this.animController.SetSymbolVisiblity(accessorySlot.targetSymbolId, false);
			}
		}
	}

	public static KCompBuilder.BodyData UpdateAccessorySlots(string nameString, ref List<ResourceRef<Accessory>> accessories)
	{
		accessories.RemoveAll((ResourceRef<Accessory> acc) => acc.Get() == null);
		Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(nameString);
		if (personalityFromNameStringKey != null)
		{
			KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personalityFromNameStringKey);
			foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots.resources)
			{
				if (accessorySlot.accessories.Count != 0)
				{
					Accessory accessory = null;
					if (accessorySlot == Db.Get().AccessorySlots.Body)
					{
						accessory = accessorySlot.Lookup(bodyData.body);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Arm)
					{
						accessory = accessorySlot.Lookup(bodyData.arms);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.ArmLower)
					{
						accessory = accessorySlot.Lookup(bodyData.armslower);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.ArmLowerSkin)
					{
						accessory = accessorySlot.Lookup(bodyData.armLowerSkin);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.ArmUpperSkin)
					{
						accessory = accessorySlot.Lookup(bodyData.armUpperSkin);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.LegSkin)
					{
						accessory = accessorySlot.Lookup(bodyData.legSkin);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Leg)
					{
						accessory = accessorySlot.Lookup(bodyData.legs);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Belt)
					{
						accessory = accessorySlot.Lookup(bodyData.belt);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Neck)
					{
						accessory = accessorySlot.Lookup("neck");
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Pelvis)
					{
						accessory = accessorySlot.Lookup(bodyData.pelvis);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Foot)
					{
						accessory = accessorySlot.Lookup(bodyData.foot);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Cuff)
					{
						accessory = accessorySlot.Lookup(bodyData.cuff);
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Hand)
					{
						accessory = accessorySlot.Lookup(bodyData.hand);
					}
					if (accessory != null)
					{
						ResourceRef<Accessory> resourceRef = new ResourceRef<Accessory>(accessory);
						accessories.Add(resourceRef);
					}
				}
			}
			return bodyData;
		}
		return default(KCompBuilder.BodyData);
	}

	public bool HasAccessory(Accessory accessory)
	{
		return this.accessories.Exists((ResourceRef<Accessory> x) => x.Get() == accessory);
	}

	public bool HasAccessoryInSlot(AccessorySlot slot)
	{
		return this.accessories.Exists((ResourceRef<Accessory> x) => x.Get().slot == slot);
	}

	public Accessory GetAccessory(AccessorySlot slot)
	{
		for (int i = 0; i < this.accessories.Count; i++)
		{
			if (this.accessories[i].Get() != null && this.accessories[i].Get().slot == slot)
			{
				return this.accessories[i].Get();
			}
		}
		return null;
	}

	public void ApplyClothingItem(ClothingItemResource clothingItem)
	{
		if (!this.clothingItems.Exists((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothingItem.IdHash))
		{
			this.clothingItems.RemoveAll((ResourceRef<ClothingItemResource> x) => x.Get().Category == clothingItem.Category);
			this.clothingItems.Add(new ResourceRef<ClothingItemResource>(clothingItem));
		}
		KAnim.Build build = clothingItem.AnimFile.GetData().build;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			string text = HashCache.Get().Get(build.symbols[i].hash);
			AccessorySlot accessorySlot = Db.Get().AccessorySlots.Find(text);
			if (accessorySlot != null)
			{
				Accessory accessory = this.GetAccessory(accessorySlot);
				if (accessory != null)
				{
					this.RemoveAccessory(accessory);
				}
				Accessory accessory2 = accessorySlot.Lookup(clothingItem.Id + text);
				if (accessory2 != null)
				{
					this.AddAccessory(accessory2);
				}
			}
		}
	}

	public void RemoveClothingItem(ClothingItemResource clothing_item)
	{
		this.clothingItems.RemoveAll((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothing_item.IdHash);
		KAnim.Build build = clothing_item.AnimFile.GetData().build;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			string text = HashCache.Get().Get(build.symbols[i].hash);
			AccessorySlot accessorySlot = Db.Get().AccessorySlots.Find(text);
			if (accessorySlot != null)
			{
				Accessory accessory = accessorySlot.Lookup(clothing_item.Id + text);
				if (accessory != null)
				{
					this.RemoveAccessory(accessory);
				}
			}
		}
		this.ValidateClothingAccessory(clothing_item.Category);
	}

	public void ApplyMinionPersonality(Personality personality)
	{
		this.bodyData = MinionStartingStats.CreateBodyData(personality);
		this.accessories.Clear();
		if (this.animController == null)
		{
			this.animController = base.GetComponent<KAnimControllerBase>();
		}
		foreach (string text in new string[] { "snapTo_hat", "snapTo_hat_hair", "snapTo_goggles", "snapTo_headFX", "snapTo_neck", "snapTo_chest", "snapTo_pivot", "skirt", "necklace" })
		{
			this.animController.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(text, 0);
			this.animController.SetSymbolVisiblity(text, false);
		}
		this.AddAccessory(Db.Get().AccessorySlots.Eyes.Lookup(this.bodyData.eyes));
		this.AddAccessory(Db.Get().AccessorySlots.Hair.Lookup(this.bodyData.hair));
		this.AddAccessory(Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(this.bodyData.hair)));
		this.AddAccessory(Db.Get().AccessorySlots.HeadShape.Lookup(this.bodyData.headShape));
		this.AddAccessory(Db.Get().AccessorySlots.Mouth.Lookup(this.bodyData.mouth));
		this.AddAccessory(Db.Get().AccessorySlots.Body.Lookup(this.bodyData.body));
		this.AddAccessory(Db.Get().AccessorySlots.Arm.Lookup(this.bodyData.arms));
		this.AddAccessory(Db.Get().AccessorySlots.ArmLower.Lookup(this.bodyData.armslower));
		this.AddAccessory(Db.Get().AccessorySlots.Neck.Lookup(this.bodyData.neck));
		this.AddAccessory(Db.Get().AccessorySlots.Pelvis.Lookup(this.bodyData.pelvis));
		this.AddAccessory(Db.Get().AccessorySlots.Leg.Lookup(this.bodyData.legs));
		this.AddAccessory(Db.Get().AccessorySlots.Foot.Lookup(this.bodyData.foot));
		this.AddAccessory(Db.Get().AccessorySlots.Hand.Lookup(this.bodyData.hand));
		this.AddAccessory(Db.Get().AccessorySlots.Cuff.Lookup(this.bodyData.cuff));
		this.AddAccessory(Db.Get().AccessorySlots.Belt.Lookup(this.bodyData.belt));
		this.AddAccessory(Db.Get().AccessorySlots.ArmLowerSkin.Lookup(this.bodyData.armLowerSkin));
		this.AddAccessory(Db.Get().AccessorySlots.ArmUpperSkin.Lookup(this.bodyData.armUpperSkin));
		this.AddAccessory(Db.Get().AccessorySlots.LegSkin.Lookup(this.bodyData.legSkin));
		this.UpdateHairBasedOnHat();
	}

	public void ApplyClothingOutfit(ClothingOutfitResource outfit, bool respectRequiredAccessorySlots = true)
	{
		IEnumerable<ClothingItemResource> enumerable = outfit.itemsInOutfit.Select<string, ClothingItemResource>((string itemId) => Db.Get().Permits.ClothingItems.Get(itemId));
		this.ApplyClothingItems(enumerable, respectRequiredAccessorySlots);
	}

	public void ApplyClothingItems(IEnumerable<ClothingItemResource> items, bool respectRequiredAccessorySlots = true)
	{
		this.clothingItems.Clear();
		this.ClearAllItemSlots();
		foreach (ClothingItemResource clothingItemResource in items)
		{
			this.ApplyClothingItem(clothingItemResource);
		}
		if (respectRequiredAccessorySlots)
		{
			this.ValidateSlots(true);
		}
		this.UpdateHairBasedOnHat();
	}

	public void UpdateHairBasedOnHat()
	{
		if (!this.GetAccessory(Db.Get().AccessorySlots.Hat).IsNullOrDestroyed())
		{
			this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
			this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
			return;
		}
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, false);
	}

	private void ValidateSlots(bool check_accessory = true)
	{
		this.ValidateClothingAccessory(PermitCategory.DupeBottoms);
		this.ValidateClothingAccessory(PermitCategory.DupeTops);
		this.ValidateClothingAccessory(PermitCategory.DupeGloves);
		this.ValidateClothingAccessory(PermitCategory.DupeShoes);
		if (check_accessory)
		{
			this.ValidateClothingAccessory(PermitCategory.DupeAccessories);
		}
		MinionResume component = base.GetComponent<MinionResume>();
		if (component != null && !component.CurrentHat.IsNullOrWhiteSpace())
		{
			MinionResume.AddHat(component.CurrentHat, base.GetComponent<KBatchedAnimController>());
		}
	}

	private void ValidateClothingAccessory(PermitCategory category)
	{
		if (!this.HasClothingAccessory(category))
		{
			if (category == PermitCategory.DupeBottoms && !this.HasBottomItem())
			{
				this.AddAccessory(Db.Get().AccessorySlots.Leg.Lookup(this.bodyData.legs));
				this.AddAccessory(Db.Get().AccessorySlots.Pelvis.Lookup(this.bodyData.pelvis));
				return;
			}
			if (category == PermitCategory.DupeTops && !this.HasTopItem())
			{
				this.AddAccessory(Db.Get().AccessorySlots.Arm.Lookup(this.bodyData.arms));
				this.AddAccessory(Db.Get().AccessorySlots.ArmLower.Lookup(this.bodyData.armslower));
				this.AddAccessory(Db.Get().AccessorySlots.Body.Lookup(this.bodyData.body));
				this.AddAccessory(Db.Get().AccessorySlots.Neck.Lookup(this.bodyData.neck));
				return;
			}
			if (category == PermitCategory.DupeGloves && !this.HasGloveItem())
			{
				this.AddAccessory(Db.Get().AccessorySlots.Cuff.Lookup(this.bodyData.cuff));
				this.AddAccessory(Db.Get().AccessorySlots.Hand.Lookup(this.bodyData.hand));
				return;
			}
			if (category == PermitCategory.DupeShoes && !this.HasFootItem())
			{
				this.AddAccessory(Db.Get().AccessorySlots.Foot.Lookup(this.bodyData.foot));
				return;
			}
			if (category == PermitCategory.DupeAccessories && !this.HasAccessoryItem())
			{
				this.AddAccessory(Db.Get().AccessorySlots.Belt.Lookup(this.bodyData.belt));
			}
		}
	}

	public bool HasClothingAccessory(PermitCategory category)
	{
		return !this.ClothingItemsDisabled && this.clothingItems.Exists((ResourceRef<ClothingItemResource> ci) => ci.Get().Category == category);
	}

	private bool HasBottomItem()
	{
		return this.HasAccessoryInSlot(Db.Get().AccessorySlots.Skirt) || this.HasAccessoryInSlot(Db.Get().AccessorySlots.Pelvis);
	}

	private bool HasTopItem()
	{
		return this.HasAccessoryInSlot(Db.Get().AccessorySlots.Body);
	}

	private bool HasAccessoryItem()
	{
		return this.HasAccessoryInSlot(Db.Get().AccessorySlots.Belt) || this.HasAccessoryInSlot(Db.Get().AccessorySlots.Necklace);
	}

	private bool HasFootItem()
	{
		return this.HasAccessoryInSlot(Db.Get().AccessorySlots.Foot);
	}

	private bool HasGloveItem()
	{
		return this.HasAccessoryInSlot(Db.Get().AccessorySlots.Hand);
	}

	private void ClearAllItemSlots()
	{
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Hat);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Neck);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Body);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Belt);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Arm);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.ArmLower);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Pelvis);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Leg);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Skirt);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Necklace);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Cuff);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Hand);
		this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Foot);
	}

	private void RemoveAccessoryFromSlot(AccessorySlot slot)
	{
		Accessory accessory = this.GetAccessory(slot);
		if (accessory != null)
		{
			this.RemoveAccessory(accessory);
		}
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
		fd.armslower = HashedString.Invalid;
		fd.hat = HashedString.Invalid;
		fd.faceFX = HashedString.Invalid;
		fd.armLowerSkin = HashedString.Invalid;
		fd.armUpperSkin = HashedString.Invalid;
		fd.legSkin = HashedString.Invalid;
		fd.belt = HashedString.Invalid;
		fd.pelvis = HashedString.Invalid;
		fd.foot = HashedString.Invalid;
		fd.skirt = HashedString.Invalid;
		fd.necklace = HashedString.Invalid;
		fd.cuff = HashedString.Invalid;
		fd.hand = HashedString.Invalid;
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
				else if (accessory.slot.Id == "Torso")
				{
					fd.body = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Arm_Sleeve")
				{
					fd.arms = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Arm_Lower_Sleeve")
				{
					fd.armslower = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Hat")
				{
					fd.hat = HashedString.Invalid;
				}
				else if (accessory.slot.Id == "FaceEffect")
				{
					fd.faceFX = HashedString.Invalid;
				}
				else if (accessory.slot.Id == "Arm_Lower")
				{
					fd.armLowerSkin = accessory.Id;
				}
				else if (accessory.slot.Id == "Arm_Upper")
				{
					fd.armUpperSkin = accessory.Id;
				}
				else if (accessory.slot.Id == "Leg_Skin")
				{
					fd.legSkin = accessory.Id;
				}
				else if (accessory.slot.Id == "Leg")
				{
					fd.legs = accessory.Id;
				}
				else if (accessory.slot.Id == "Belt")
				{
					fd.belt = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Pelvis")
				{
					fd.pelvis = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Foot")
				{
					fd.foot = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Cuff")
				{
					fd.cuff = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Skirt")
				{
					fd.skirt = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Hand")
				{
					fd.hand = accessory.IdHash;
				}
			}
		}
	}

	[Serialize]
	private List<ResourceRef<Accessory>> accessories = new List<ResourceRef<Accessory>>();

	[MyCmpReq]
	private KAnimControllerBase animController;

	[Serialize]
	private bool ClothingItemsDisabled;

	[Serialize]
	private List<ResourceRef<ClothingItemResource>> clothingItems = new List<ResourceRef<ClothingItemResource>>();
}
