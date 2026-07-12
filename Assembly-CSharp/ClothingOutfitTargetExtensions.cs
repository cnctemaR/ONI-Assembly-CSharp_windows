using System;
using System.Collections.Generic;
using Database;
using STRINGS;

public static class ClothingOutfitTargetExtensions
{
	public static string ReadName(this Option<ClothingOutfitTarget> self)
	{
		if (self.HasValue)
		{
			return self.Value.ReadName();
		}
		return UI.OUTFIT_NAME.NONE;
	}

	public static IEnumerable<string> ReadItems(this Option<ClothingOutfitTarget> self)
	{
		if (self.HasValue)
		{
			return self.Value.ReadItems();
		}
		return ClothingOutfitTargetExtensions.NO_ITEMS;
	}

	public static IEnumerable<ClothingItemResource> ReadItemValues(this Option<ClothingOutfitTarget> self)
	{
		if (self.HasValue)
		{
			return self.Value.ReadItemValues();
		}
		return ClothingOutfitTargetExtensions.NO_ITEM_VALUES;
	}

	public static Option<string> GetId(this Option<ClothingOutfitTarget> self)
	{
		if (self.HasValue)
		{
			return self.Value.Id;
		}
		return Option.None;
	}

	public static readonly string[] NO_ITEMS = new string[0];

	public static readonly ClothingItemResource[] NO_ITEM_VALUES = new ClothingItemResource[0];
}
