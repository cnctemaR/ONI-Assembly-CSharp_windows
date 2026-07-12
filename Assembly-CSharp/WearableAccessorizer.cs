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
	public Dictionary<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>> GetCustomClothingItems()
	{
		return this.customOutfitItems;
	}

	public Dictionary<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> Wearables
	{
		get
		{
			return this.wearables;
		}
	}

	public string[] GetClothingItemsIds(ClothingOutfitUtility.OutfitType outfitType)
	{
		if (this.customOutfitItems.ContainsKey(outfitType))
		{
			string[] array = new string[this.customOutfitItems[outfitType].Count];
			for (int i = 0; i < this.customOutfitItems[outfitType].Count; i++)
			{
				array[i] = this.customOutfitItems[outfitType][i].Get().Id;
			}
			return array;
		}
		return new string[0];
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
	[Obsolete]
	private void OnDeserialized()
	{
		List<WearableAccessorizer.WearableType> list = new List<WearableAccessorizer.WearableType>();
		foreach (KeyValuePair<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> keyValuePair in this.wearables)
		{
			keyValuePair.Value.Deserialize();
			if (keyValuePair.Value.BuildAnims == null)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (WearableAccessorizer.WearableType wearableType in list)
		{
			this.wearables.Remove(wearableType);
		}
		if (this.clothingItems.Count > 0)
		{
			this.customOutfitItems[ClothingOutfitUtility.OutfitType.Clothing] = new List<ResourceRef<ClothingItemResource>>(this.clothingItems);
			this.clothingItems.Clear();
			if (!this.wearables.ContainsKey(WearableAccessorizer.WearableType.CustomClothing))
			{
				foreach (ResourceRef<ClothingItemResource> resourceRef in this.customOutfitItems[ClothingOutfitUtility.OutfitType.Clothing])
				{
					this.ApplyClothingItem(ClothingOutfitUtility.OutfitType.Clothing, resourceRef.Get());
				}
			}
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
				this.RemoveAnimBuild(this.wearables[wearableType].BuildAnims[0], this.wearables[wearableType].buildOverridePriority);
			}
			ClothingOutfitUtility.OutfitType outfitType;
			if (this.TryGetEquippableClothingType(equippable.def, out outfitType) && this.customOutfitItems.ContainsKey(outfitType))
			{
				this.wearables[WearableAccessorizer.WearableType.CustomSuit] = new WearableAccessorizer.Wearable(animFile, equippable.def.BuildOverridePriority);
				this.wearables[WearableAccessorizer.WearableType.CustomSuit].AddCustomItems(this.customOutfitItems[outfitType]);
			}
			else
			{
				this.wearables[wearableType] = new WearableAccessorizer.Wearable(animFile, equippable.def.BuildOverridePriority);
			}
			this.ApplyWearable();
		}
	}

	private bool TryGetEquippableClothingType(EquipmentDef equipment, out ClothingOutfitUtility.OutfitType outfitType)
	{
		if (equipment.Id == "Atmo_Suit")
		{
			outfitType = ClothingOutfitUtility.OutfitType.AtmoSuit;
			return true;
		}
		outfitType = ClothingOutfitUtility.OutfitType.LENGTH;
		return false;
	}

	private bool IsWearingSuitType(ClothingOutfitUtility.OutfitType outfitType)
	{
		Equippable suitEquippable = this.GetSuitEquippable();
		if (suitEquippable != null)
		{
			ClothingOutfitUtility.OutfitType outfitType2;
			this.TryGetEquippableClothingType(suitEquippable.def, out outfitType2);
			return outfitType2 == outfitType;
		}
		return false;
	}

	private Equippable GetSuitEquippable()
	{
		MinionIdentity component = base.GetComponent<MinionIdentity>();
		if (component != null && component.assignableProxy.Get() != null)
		{
			Equipment equipment = component.GetEquipment();
			Assignable assignable = ((equipment != null) ? equipment.GetAssignable(Db.Get().AssignableSlots.Suit) : null);
			if (assignable != null)
			{
				return assignable.GetComponent<Equippable>();
			}
		}
		return null;
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
		foreach (object obj in Enum.GetValues(typeof(WearableAccessorizer.WearableType)))
		{
			WearableAccessorizer.WearableType wearableType = (WearableAccessorizer.WearableType)obj;
			if (this.wearables.ContainsKey(wearableType))
			{
				WearableAccessorizer.Wearable wearable = this.wearables[wearableType];
				int buildOverridePriority = wearable.buildOverridePriority;
				foreach (KAnimFile kanimFile in wearable.BuildAnims)
				{
					KAnim.Build build = kanimFile.GetData().build;
					if (build != null)
					{
						for (int i = 0; i < build.symbols.Length; i++)
						{
							string text = HashCache.Get().Get(build.symbols[i].hash);
							if (wearableType == highestAccessory)
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
			List<KAnimHashedString> list = this.wearables[wearableType].BuildAnims.SelectMany<KAnimFile, KAnimHashedString>((KAnimFile x) => x.GetData().build.symbols.Select<KAnim.Build.Symbol, KAnimHashedString>((KAnim.Build.Symbol s) => s.hash)).ToList<KAnimHashedString>();
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
			this.RemoveEquipment(component);
		}
	}

	public void RemoveEquipment(Equippable equippable)
	{
		WearableAccessorizer.WearableType wearableType;
		if (equippable != null && Enum.TryParse<WearableAccessorizer.WearableType>(equippable.def.Slot, out wearableType))
		{
			ClothingOutfitUtility.OutfitType outfitType;
			if (this.TryGetEquippableClothingType(equippable.def, out outfitType) && this.customOutfitItems.ContainsKey(outfitType) && this.wearables.ContainsKey(WearableAccessorizer.WearableType.CustomSuit))
			{
				foreach (ResourceRef<ClothingItemResource> resourceRef in this.customOutfitItems[outfitType])
				{
					this.RemoveAnimBuild(resourceRef.Get().AnimFile, this.wearables[WearableAccessorizer.WearableType.CustomSuit].buildOverridePriority);
				}
				this.RemoveAnimBuild(equippable.GetBuildOverride(), this.wearables[WearableAccessorizer.WearableType.CustomSuit].buildOverridePriority);
				this.wearables.Remove(WearableAccessorizer.WearableType.CustomSuit);
			}
			if (this.wearables.ContainsKey(wearableType))
			{
				this.RemoveAnimBuild(equippable.GetBuildOverride(), this.wearables[wearableType].buildOverridePriority);
				this.wearables.Remove(wearableType);
			}
			this.ApplyWearable();
		}
	}

	public void AddCustomClothingOutfit(ClothingOutfitUtility.OutfitType outfitType, IEnumerable<ClothingItemResource> items)
	{
		if (outfitType == ClothingOutfitUtility.OutfitType.Clothing)
		{
			this.ApplyClothingItems(outfitType, items);
			return;
		}
		if (this.IsWearingSuitType(outfitType))
		{
			this.ApplyClothingItems(outfitType, items);
			Equippable suitEquippable = this.GetSuitEquippable();
			if (suitEquippable != null)
			{
				this.ApplyEquipment(suitEquippable, suitEquippable.GetBuildOverride());
				return;
			}
		}
		else
		{
			if (!this.customOutfitItems.ContainsKey(outfitType))
			{
				this.customOutfitItems.Add(outfitType, new List<ResourceRef<ClothingItemResource>>());
			}
			using (IEnumerator<ClothingItemResource> enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ClothingItemResource clothingItem = enumerator.Current;
					if (!this.customOutfitItems[outfitType].Exists((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothingItem.IdHash))
					{
						foreach (ResourceRef<ClothingItemResource> resourceRef in this.customOutfitItems[outfitType].FindAll((ResourceRef<ClothingItemResource> x) => x.Get().Category == clothingItem.Category))
						{
							this.RemoveClothingItem(outfitType, resourceRef.Get());
						}
						this.customOutfitItems[outfitType].Add(new ResourceRef<ClothingItemResource>(clothingItem));
					}
				}
			}
		}
	}

	public void ApplyClothingItem(ClothingOutfitUtility.OutfitType outfitType, ClothingItemResource clothingItem)
	{
		WearableAccessorizer.WearableType wearableType = this.ConvertOutfitTypeToWearableType(outfitType);
		if (!this.customOutfitItems.ContainsKey(outfitType))
		{
			this.customOutfitItems.Add(outfitType, new List<ResourceRef<ClothingItemResource>>());
		}
		if (!this.customOutfitItems[outfitType].Exists((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothingItem.IdHash))
		{
			if (this.wearables.ContainsKey(wearableType))
			{
				foreach (ResourceRef<ClothingItemResource> resourceRef in this.customOutfitItems[outfitType].FindAll((ResourceRef<ClothingItemResource> x) => x.Get().Category == clothingItem.Category))
				{
					this.RemoveClothingItem(outfitType, resourceRef.Get());
				}
			}
			this.customOutfitItems[outfitType].Add(new ResourceRef<ClothingItemResource>(clothingItem));
		}
		if (!this.wearables.ContainsKey(wearableType))
		{
			int num = ((wearableType == WearableAccessorizer.WearableType.CustomClothing) ? 4 : 6);
			this.wearables[wearableType] = new WearableAccessorizer.Wearable(new List<KAnimFile>(), num);
		}
		this.wearables[wearableType].AddAnim(clothingItem.AnimFile);
	}

	public void RemoveClothingItem(ClothingOutfitUtility.OutfitType outfitType, ClothingItemResource clothing_item)
	{
		WearableAccessorizer.WearableType wearableType = this.ConvertOutfitTypeToWearableType(outfitType);
		if (this.customOutfitItems.ContainsKey(outfitType))
		{
			this.customOutfitItems[outfitType].RemoveAll((ResourceRef<ClothingItemResource> x) => x.Get().IdHash == clothing_item.IdHash);
		}
		if (this.wearables.ContainsKey(wearableType))
		{
			if (this.wearables[wearableType].RemoveAnim(clothing_item.AnimFile))
			{
				this.RemoveAnimBuild(clothing_item.AnimFile, this.wearables[wearableType].buildOverridePriority);
			}
			if (this.wearables[wearableType].BuildAnims.Count <= 0)
			{
				this.wearables.Remove(wearableType);
			}
		}
	}

	public void ApplyClothingOutfit(ClothingOutfitResource outfit)
	{
		IEnumerable<ClothingItemResource> enumerable = outfit.itemsInOutfit.Select<string, ClothingItemResource>((string itemId) => Db.Get().Permits.ClothingItems.Get(itemId));
		this.ApplyClothingItems(ClothingOutfitUtility.OutfitType.Clothing, enumerable);
	}

	public void ClearAllOutfitItems(ClothingOutfitUtility.OutfitType? forOutfitType = null)
	{
		foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>> keyValuePair in this.customOutfitItems)
		{
			ClothingOutfitUtility.OutfitType outfitType;
			List<ResourceRef<ClothingItemResource>> list;
			keyValuePair.Deconstruct<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>>(out outfitType, out list);
			ClothingOutfitUtility.OutfitType outfitType2 = outfitType;
			if (forOutfitType != null)
			{
				ClothingOutfitUtility.OutfitType? outfitType3 = forOutfitType;
				outfitType = outfitType2;
				if (!((outfitType3.GetValueOrDefault() == outfitType) & (outfitType3 != null)))
				{
					continue;
				}
			}
			this.ApplyClothingItems(outfitType2, Enumerable.Empty<ClothingItemResource>());
		}
	}

	public void ApplyClothingItems(ClothingOutfitUtility.OutfitType outfitType, IEnumerable<ClothingItemResource> items)
	{
		if (this.customOutfitItems.ContainsKey(outfitType))
		{
			this.customOutfitItems[outfitType].Clear();
		}
		WearableAccessorizer.WearableType wearableType = this.ConvertOutfitTypeToWearableType(outfitType);
		if (this.wearables.ContainsKey(wearableType))
		{
			foreach (KAnimFile kanimFile in this.wearables[wearableType].BuildAnims)
			{
				this.RemoveAnimBuild(kanimFile, this.wearables[wearableType].buildOverridePriority);
			}
			this.wearables[wearableType].ClearAnims();
			if (items.Count<ClothingItemResource>() <= 0)
			{
				this.wearables.Remove(wearableType);
			}
		}
		foreach (ClothingItemResource clothingItemResource in items)
		{
			this.ApplyClothingItem(outfitType, clothingItemResource);
		}
		this.ApplyWearable();
	}

	private WearableAccessorizer.WearableType ConvertOutfitTypeToWearableType(ClothingOutfitUtility.OutfitType outfitType)
	{
		if (outfitType == ClothingOutfitUtility.OutfitType.Clothing)
		{
			return WearableAccessorizer.WearableType.CustomClothing;
		}
		if (outfitType != ClothingOutfitUtility.OutfitType.AtmoSuit)
		{
			global::Debug.LogWarning("Add a wearable type for clothing outfit type " + outfitType.ToString());
			return WearableAccessorizer.WearableType.Basic;
		}
		return WearableAccessorizer.WearableType.CustomSuit;
	}

	public void RestoreWearables(Dictionary<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> stored_wearables, Dictionary<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>> clothing)
	{
		if (stored_wearables != null)
		{
			this.wearables = stored_wearables;
			foreach (KeyValuePair<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> keyValuePair in this.wearables)
			{
				keyValuePair.Value.Deserialize();
			}
		}
		if (clothing != null)
		{
			foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>> keyValuePair2 in clothing)
			{
				this.ApplyClothingItems(keyValuePair2.Key, keyValuePair2.Value.Select<ResourceRef<ClothingItemResource>, ClothingItemResource>((ResourceRef<ClothingItemResource> i) => i.Get()));
			}
		}
		this.ApplyWearable();
	}

	public void AddCustomOutfit(Option<ClothingOutfitTarget> outfit)
	{
		this.customOutfitItems[outfit.Value.OutfitType] = new List<ResourceRef<ClothingItemResource>>();
		foreach (ClothingItemResource clothingItemResource in outfit.Value.ReadItemValues())
		{
			this.customOutfitItems[outfit.Value.OutfitType].Add(new ResourceRef<ClothingItemResource>(clothingItemResource));
		}
	}

	[MyCmpReq]
	private KAnimControllerBase animController;

	[Obsolete("Deprecated, use customOufitItems[ClothingOutfitUtility.OutfitType.Clothing]")]
	[Serialize]
	private List<ResourceRef<ClothingItemResource>> clothingItems = new List<ResourceRef<ClothingItemResource>>();

	[Serialize]
	private string joyResponsePermitId;

	[Serialize]
	private Dictionary<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>> customOutfitItems = new Dictionary<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>>();

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

	[SerializationConfig(MemberSerialization.OptIn)]
	public class Wearable
	{
		public List<KAnimFile> BuildAnims
		{
			get
			{
				return this.buildAnims;
			}
		}

		public List<string> AnimNames
		{
			get
			{
				return this.animNames;
			}
		}

		public Wearable(List<KAnimFile> buildAnims, int buildOverridePriority)
		{
			this.buildAnims = buildAnims;
			this.animNames = buildAnims.Select<KAnimFile, string>((KAnimFile animFile) => animFile.name).ToList<string>();
			this.buildOverridePriority = buildOverridePriority;
		}

		public Wearable(KAnimFile buildAnim, int buildOverridePriority)
		{
			this.buildAnims = new List<KAnimFile> { buildAnim };
			this.animNames = new List<string> { buildAnim.name };
			this.buildOverridePriority = buildOverridePriority;
		}

		public Wearable(List<ResourceRef<ClothingItemResource>> items, int buildOverridePriority)
		{
			this.buildAnims = new List<KAnimFile>();
			this.animNames = new List<string>();
			this.buildOverridePriority = buildOverridePriority;
			foreach (ResourceRef<ClothingItemResource> resourceRef in items)
			{
				ClothingItemResource clothingItemResource = resourceRef.Get();
				this.buildAnims.Add(clothingItemResource.AnimFile);
				this.animNames.Add(clothingItemResource.animFilename);
			}
		}

		public void AddCustomItems(List<ResourceRef<ClothingItemResource>> items)
		{
			foreach (ResourceRef<ClothingItemResource> resourceRef in items)
			{
				ClothingItemResource clothingItemResource = resourceRef.Get();
				this.buildAnims.Add(clothingItemResource.AnimFile);
				this.animNames.Add(clothingItemResource.animFilename);
			}
		}

		public void Deserialize()
		{
			if (this.animNames != null)
			{
				this.buildAnims = new List<KAnimFile>();
				for (int i = 0; i < this.animNames.Count; i++)
				{
					this.buildAnims.Add(Assets.GetAnim(this.animNames[i]));
				}
			}
		}

		public void AddAnim(KAnimFile animFile)
		{
			this.buildAnims.Add(animFile);
			this.animNames.Add(animFile.name);
		}

		public bool RemoveAnim(KAnimFile animFile)
		{
			return this.buildAnims.Remove(animFile) | this.animNames.Remove(animFile.name);
		}

		public void ClearAnims()
		{
			this.buildAnims.Clear();
			this.animNames.Clear();
		}

		private List<KAnimFile> buildAnims;

		[Serialize]
		private List<string> animNames;

		[Serialize]
		public int buildOverridePriority;
	}
}
