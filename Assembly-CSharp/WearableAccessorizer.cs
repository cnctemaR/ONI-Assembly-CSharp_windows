using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/WearableAccessorizer")]
public class WearableAccessorizer : KMonoBehaviour
{
	public List<ResourceRef<ClothingItemResource>> GetClothingItems()
	{
		return this.clothingItems;
	}

	public string[] GetClothingItemIds()
	{
		string[] array = new string[this.clothingItems.Count];
		for (int i = 0; i < this.clothingItems.Count; i++)
		{
			array[i] = this.clothingItems[i].Get().Id;
		}
		return array;
	}

	public Option<string> GetJoyResponseId()
	{
		return this.joyResponsePermitId;
	}

	public void SetJoyResponseId(Option<string> joyResponsePermitId)
	{
		this.joyResponsePermitId = joyResponsePermitId.UnwrapOr(null, null);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.animController == null)
		{
			this.animController = base.GetComponent<KAnimControllerBase>();
		}
		base.Subscribe(-448952673, new Action<object>(this.EquippedItem));
		base.Subscribe(-1285462312, new Action<object>(this.UnequippedItem));
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		foreach (KeyValuePair<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> keyValuePair in this.wearables)
		{
			keyValuePair.Value.Deserialize();
		}
		this.ApplyWearable();
	}

	public void EquippedItem(object data)
	{
		KPrefabID kprefabID = data as KPrefabID;
		if (kprefabID != null)
		{
			Equippable component = kprefabID.GetComponent<Equippable>();
			this.ApplyEquipment(component, component.GetBuildOverride());
		}
	}

	public void ApplyEquipment(Equippable equippable, KAnimFile animFile)
	{
		WearableAccessorizer.WearableType wearableType;
		if (equippable != null && animFile != null && Enum.TryParse<WearableAccessorizer.WearableType>(equippable.def.Slot, out wearableType))
		{
			if (this.wearables.ContainsKey(wearableType))
			{
				this.RemoveAnimBuild(this.wearables[wearableType].buildAnims[0], this.wearables[wearableType].buildOverridePriority);
			}
			this.wearables[wearableType] = new WearableAccessorizer.Wearable(animFile, equippable.def.BuildOverridePriority);
			this.ApplyWearable();
		}
	}

	private WearableAccessorizer.WearableType GetHighestAccessory()
	{
		WearableAccessorizer.WearableType wearableType = WearableAccessorizer.WearableType.Basic;
		foreach (WearableAccessorizer.WearableType wearableType2 in this.wearables.Keys)
		{
			if (wearableType2 > wearableType)
			{
				wearableType = wearableType2;
			}
		}
		return wearableType;
	}

	private void ApplyWearable()
	{
		if (this.animController == null)
		{
			global::Debug.LogWarning("Missing animcontroller for WearableAccessorizer, bailing early to prevent a crash!");
			return;
		}
		SymbolOverrideController component = base.GetComponent<SymbolOverrideController>();
		WearableAccessorizer.WearableType highestAccessory = this.GetHighestAccessory();
		foreach (KeyValuePair<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> keyValuePair in this.wearables)
		{
			int buildOverridePriority = keyValuePair.Value.buildOverridePriority;
			foreach (KAnimFile kanimFile in keyValuePair.Value.buildAnims)
			{
				KAnim.Build build = kanimFile.GetData().build;
				if (build != null)
				{
					for (int i = 0; i < build.symbols.Length; i++)
					{
						string text = HashCache.Get().Get(build.symbols[i].hash);
						if (keyValuePair.Key == highestAccessory)
						{
							component.AddSymbolOverride(text, build.symbols[i], buildOverridePriority);
							this.animController.SetSymbolVisiblity(text, true);
						}
						else
						{
							component.RemoveSymbolOverride(text, buildOverridePriority);
						}
					}
				}
			}
		}
		this.UpdateVisibleSymbols(highestAccessory);
	}

	private void UpdateVisibleSymbols(WearableAccessorizer.WearableType wearableType)
	{
		bool flag = wearableType == WearableAccessorizer.WearableType.Basic;
		bool flag2 = base.GetComponent<Accessorizer>().GetAccessory(Db.Get().AccessorySlots.Hat) != null;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = true;
		if (this.wearables.ContainsKey(wearableType))
		{
			List<KAnimHashedString> list = this.wearables[wearableType].buildAnims.SelectMany<KAnimFile, KAnimHashedString>((KAnimFile x) => x.GetData().build.symbols.Select<KAnim.Build.Symbol, KAnimHashedString>((KAnim.Build.Symbol s) => s.hash)).ToList<KAnimHashedString>();
			flag = flag || list.Contains(Db.Get().AccessorySlots.Belt.targetSymbolId);
			flag3 = list.Contains(Db.Get().AccessorySlots.Skirt.targetSymbolId);
			flag4 = list.Contains(Db.Get().AccessorySlots.Necklace.targetSymbolId);
			flag5 = list.Contains(Db.Get().AccessorySlots.ArmLower.targetSymbolId);
		}
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Belt.targetSymbolId, flag);
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Necklace.targetSymbolId, flag4);
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.ArmLower.targetSymbolId, flag5);
		WearableAccessorizer.SkirtAccessory(this.animController, flag3);
		WearableAccessorizer.UpdateHairBasedOnHat(this.animController, flag2);
	}

	public static void UpdateHairBasedOnHat(KAnimControllerBase kbac, bool hasHat)
	{
		if (hasHat)
		{
			kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
			kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
			kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, true);
			return;
		}
		kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
		kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
		kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, false);
	}

	public static void SkirtAccessory(KAnimControllerBase kbac, bool show_skirt)
	{
		kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.Skirt.targetSymbolId, show_skirt);
		kbac.SetSymbolVisiblity(Db.Get().AccessorySlots.Leg.targetSymbolId, !show_skirt);
	}

	private void RemoveAnimBuild(KAnimFile animFile, int override_priority)
	{
		SymbolOverrideController component = base.GetComponent<SymbolOverrideController>();
		KAnim.Build build = ((animFile != null) ? animFile.GetData().build : null);
		if (build != null)
		{
			for (int i = 0; i < build.symbols.Length; i++)
			{
				string text = HashCache.Get().Get(build.symbols[i].hash);
				component.RemoveSymbolOverride(text, override_priority);
			}
		}
	}

	private void UnequippedItem(object data)
	{
		KPrefabID kprefabID = data as KPrefabID;
		if (kprefabID != null)
		{
			Equippable component = kprefabID.GetComponent<Equippable>();
			WearableAccessorizer.WearableType wearableType;
			if (component != null && Enum.TryParse<WearableAccessorizer.WearableType>(component.def.Slot, out wearableType))
			{
				if (this.wearables.ContainsKey(wearableType))
				{
					this.RemoveAnimBuild(component.GetBuildOverride(), this.wearables[wearableType].buildOverridePriority);
					this.wearables.Remove(wearableType);
				}
				this.ApplyWearable();
			}
		}
	}

	public void ApplyClothingItem(ClothingItemResource clothingItem)
	{
		if (!this.clothingItems.Exists((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothingItem.IdHash))
		{
			if (this.wearables.ContainsKey(WearableAccessorizer.WearableType.CustomClothing))
			{
				foreach (ResourceRef<ClothingItemResource> resourceRef in this.clothingItems.FindAll((ResourceRef<ClothingItemResource> x) => x.Get().Category == clothingItem.Category))
				{
					this.RemoveClothingItem(resourceRef.Get());
				}
			}
			this.clothingItems.Add(new ResourceRef<ClothingItemResource>(clothingItem));
		}
		if (!this.wearables.ContainsKey(WearableAccessorizer.WearableType.CustomClothing))
		{
			this.wearables[WearableAccessorizer.WearableType.CustomClothing] = new WearableAccessorizer.Wearable(new List<KAnimFile>(), 4);
		}
		this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildAnims.Add(clothingItem.AnimFile);
	}

	public void RemoveClothingItem(ClothingItemResource clothing_item)
	{
		this.clothingItems.RemoveAll((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothing_item.IdHash);
		if (this.wearables.ContainsKey(WearableAccessorizer.WearableType.CustomClothing))
		{
			if (this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildAnims.Remove(clothing_item.AnimFile))
			{
				this.RemoveAnimBuild(clothing_item.AnimFile, this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildOverridePriority);
			}
			if (this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildAnims.Count <= 0)
			{
				this.wearables.Remove(WearableAccessorizer.WearableType.CustomClothing);
			}
		}
	}

	public void ApplyClothingOutfit(ClothingOutfitResource outfit)
	{
		IEnumerable<ClothingItemResource> enumerable = outfit.itemsInOutfit.Select<string, ClothingItemResource>((string itemId) => Db.Get().Permits.ClothingItems.Get(itemId));
		this.ApplyClothingItems(enumerable);
	}

	public void ApplyClothingItems(IEnumerable<ClothingItemResource> items)
	{
		this.clothingItems.Clear();
		if (this.wearables.ContainsKey(WearableAccessorizer.WearableType.CustomClothing))
		{
			foreach (KAnimFile kanimFile in this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildAnims)
			{
				this.RemoveAnimBuild(kanimFile, this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildOverridePriority);
			}
			this.wearables[WearableAccessorizer.WearableType.CustomClothing].buildAnims.Clear();
			if (items.Count<ClothingItemResource>() <= 0)
			{
				this.wearables.Remove(WearableAccessorizer.WearableType.CustomClothing);
			}
		}
		foreach (ClothingItemResource clothingItemResource in items)
		{
			this.ApplyClothingItem(clothingItemResource);
		}
		this.ApplyWearable();
	}

	[MyCmpReq]
	private KAnimControllerBase animController;

	[Serialize]
	private List<ResourceRef<ClothingItemResource>> clothingItems = new List<ResourceRef<ClothingItemResource>>();

	[Serialize]
	private string joyResponsePermitId;

	[Serialize]
	private Dictionary<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> wearables = new Dictionary<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable>();

	public enum WearableType
	{
		Basic,
		CustomClothing,
		Outfit,
		Suit,
		CustomSuit
	}

	private class Wearable
	{
		public Wearable(List<KAnimFile> buildAnims, int buildOverridePriority)
		{
			this.buildAnims = buildAnims;
			this.buildOverridePriority = buildOverridePriority;
		}

		public Wearable(KAnimFile buildAnim, int buildOverridePriority)
		{
			this.buildAnims = new List<KAnimFile> { buildAnim };
			this.buildOverridePriority = buildOverridePriority;
		}

		public void Deserialize()
		{
			for (int i = 0; i < this.buildAnims.Count; i++)
			{
				this.buildAnims[i] = Assets.GetAnim(this.buildAnims[i].name);
			}
		}

		public List<KAnimFile> buildAnims;

		public int buildOverridePriority;
	}
}
