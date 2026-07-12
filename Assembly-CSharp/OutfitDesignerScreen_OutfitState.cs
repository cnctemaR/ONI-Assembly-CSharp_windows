using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class OutfitDesignerScreen_OutfitState
{
	private OutfitDesignerScreen_OutfitState(ClothingOutfitTarget sourceTarget, ClothingOutfitTarget destinationTarget)
	{
		this.destinationTarget = destinationTarget;
		this.sourceTarget = sourceTarget;
		this.name = sourceTarget.ReadName();
		foreach (ClothingItemResource clothingItemResource in sourceTarget.ReadItemValues())
		{
			this.ApplyItem(clothingItemResource);
		}
	}

	public static OutfitDesignerScreen_OutfitState ForTemplateOutfit(ClothingOutfitTarget outfitTemplate)
	{
		global::Debug.Assert(outfitTemplate.IsTemplateOutfit());
		return new OutfitDesignerScreen_OutfitState(outfitTemplate, outfitTemplate);
	}

	public static OutfitDesignerScreen_OutfitState ForMinionInstance(ClothingOutfitTarget sourceTarget, GameObject minionInstance)
	{
		return new OutfitDesignerScreen_OutfitState(sourceTarget, ClothingOutfitTarget.FromMinion(minionInstance));
	}

	public unsafe void ApplyItem(ClothingItemResource item)
	{
		*this.GetItemSlotForCategory(item.Category) = item;
	}

	public ref Option<ClothingItemResource> GetItemSlotForCategory(PermitCategory category)
	{
		if (category == PermitCategory.DupeHats)
		{
			return ref this.hatSlot;
		}
		if (category == PermitCategory.DupeTops)
		{
			return ref this.topSlot;
		}
		if (category == PermitCategory.DupeGloves)
		{
			return ref this.glovesSlot;
		}
		if (category == PermitCategory.DupeBottoms)
		{
			return ref this.bottomSlot;
		}
		if (category == PermitCategory.DupeShoes)
		{
			return ref this.shoesSlot;
		}
		if (category == PermitCategory.DupeAccessories)
		{
			return ref this.accessorySlot;
		}
		DebugUtil.DevAssert(false, string.Format("Couldn't get a {0}<{1}> for {2} \"{3}\" on {4} \"{5}\".", new object[] { "Option", "ClothingItemResource", "PermitCategory", category, "OutfitDesignerScreen_OutfitState", this.name }), null);
		return ref OutfitDesignerScreen_OutfitState.dummySlot;
	}

	public void AddItemValuesTo(ICollection<ClothingItemResource> clothingItems)
	{
		if (this.hatSlot.HasValue)
		{
			clothingItems.Add(this.hatSlot);
		}
		if (this.topSlot.HasValue)
		{
			clothingItems.Add(this.topSlot);
		}
		if (this.glovesSlot.HasValue)
		{
			clothingItems.Add(this.glovesSlot);
		}
		if (this.bottomSlot.HasValue)
		{
			clothingItems.Add(this.bottomSlot);
		}
		if (this.shoesSlot.HasValue)
		{
			clothingItems.Add(this.shoesSlot);
		}
		if (this.accessorySlot.HasValue)
		{
			clothingItems.Add(this.accessorySlot);
		}
	}

	public void AddItemsTo(ICollection<string> itemIds)
	{
		if (this.hatSlot.HasValue)
		{
			itemIds.Add(this.hatSlot.Value.Id);
		}
		if (this.topSlot.HasValue)
		{
			itemIds.Add(this.topSlot.Value.Id);
		}
		if (this.glovesSlot.HasValue)
		{
			itemIds.Add(this.glovesSlot.Value.Id);
		}
		if (this.bottomSlot.HasValue)
		{
			itemIds.Add(this.bottomSlot.Value.Id);
		}
		if (this.shoesSlot.HasValue)
		{
			itemIds.Add(this.shoesSlot.Value.Id);
		}
		if (this.accessorySlot.HasValue)
		{
			itemIds.Add(this.accessorySlot.Value.Id);
		}
	}

	public string[] GetItems()
	{
		List<string> list = new List<string>();
		this.AddItemsTo(list);
		return list.ToArray();
	}

	public bool DoesContainNonOwnedItems()
	{
		bool flag;
		using (ListPool<string, OutfitDesignerScreen_OutfitState>.PooledList pooledList = PoolsFor<OutfitDesignerScreen_OutfitState>.AllocateList<string>())
		{
			this.AddItemsTo(pooledList);
			flag = ClothingOutfitTarget.DoesContainNonOwnedItems(pooledList);
		}
		return flag;
	}

	public bool IsDirty()
	{
		using (HashSetPool<string, OutfitDesignerScreen>.PooledHashSet pooledHashSet = PoolsFor<OutfitDesignerScreen>.AllocateHashSet<string>())
		{
			this.AddItemsTo(pooledHashSet);
			string[] array = this.destinationTarget.ReadItems();
			if (pooledHashSet.Count != array.Length)
			{
				return true;
			}
			foreach (string text in array)
			{
				if (!pooledHashSet.Contains(text))
				{
					return true;
				}
			}
		}
		return false;
	}

	public string name;

	public Option<ClothingItemResource> hatSlot;

	public Option<ClothingItemResource> topSlot;

	public Option<ClothingItemResource> glovesSlot;

	public Option<ClothingItemResource> bottomSlot;

	public Option<ClothingItemResource> shoesSlot;

	public Option<ClothingItemResource> accessorySlot;

	public ClothingOutfitUtility.OutfitType outfitType;

	public ClothingOutfitTarget sourceTarget;

	public ClothingOutfitTarget destinationTarget;

	private static Option<ClothingItemResource> dummySlot;
}
