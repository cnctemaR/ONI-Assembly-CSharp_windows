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
		if (this.hatSlot.IsSome())
		{
			clothingItems.Add(this.hatSlot.Unwrap());
		}
		if (this.topSlot.IsSome())
		{
			clothingItems.Add(this.topSlot.Unwrap());
		}
		if (this.glovesSlot.IsSome())
		{
			clothingItems.Add(this.glovesSlot.Unwrap());
		}
		if (this.bottomSlot.IsSome())
		{
			clothingItems.Add(this.bottomSlot.Unwrap());
		}
		if (this.shoesSlot.IsSome())
		{
			clothingItems.Add(this.shoesSlot.Unwrap());
		}
		if (this.accessorySlot.IsSome())
		{
			clothingItems.Add(this.accessorySlot.Unwrap());
		}
	}

	public void AddItemsTo(ICollection<string> itemIds)
	{
		if (this.hatSlot.IsSome())
		{
			itemIds.Add(this.hatSlot.Unwrap().Id);
		}
		if (this.topSlot.IsSome())
		{
			itemIds.Add(this.topSlot.Unwrap().Id);
		}
		if (this.glovesSlot.IsSome())
		{
			itemIds.Add(this.glovesSlot.Unwrap().Id);
		}
		if (this.bottomSlot.IsSome())
		{
			itemIds.Add(this.bottomSlot.Unwrap().Id);
		}
		if (this.shoesSlot.IsSome())
		{
			itemIds.Add(this.shoesSlot.Unwrap().Id);
		}
		if (this.accessorySlot.IsSome())
		{
			itemIds.Add(this.accessorySlot.Unwrap().Id);
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
