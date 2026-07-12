using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Database;

public static class UIMinionOrMannequinITargetExtensions
{
	public static void SetOutfit(this UIMinionOrMannequin.ITarget self, ClothingOutfitResource outfit)
	{
		self.SetOutfit(outfit.itemsInOutfit.Select<string, ClothingItemResource>((string itemId) => Db.Get().Permits.ClothingItems.Get(itemId)));
	}

	public static void SetOutfit(this UIMinionOrMannequin.ITarget self, OutfitDesignerScreen_OutfitState outfit)
	{
		self.SetOutfit(from itemId in outfit.GetItems()
			select Db.Get().Permits.ClothingItems.Get(itemId));
	}

	public static void SetOutfit(this UIMinionOrMannequin.ITarget self, ClothingOutfitTarget outfit)
	{
		self.SetOutfit(outfit.ReadItemValues());
	}

	public static void SetOutfit(this UIMinionOrMannequin.ITarget self, Option<ClothingOutfitTarget> outfit)
	{
		if (outfit.HasValue)
		{
			self.SetOutfit(outfit.Value);
			return;
		}
		self.ClearOutfit();
	}

	public static void ClearOutfit(this UIMinionOrMannequin.ITarget self)
	{
		self.SetOutfit(UIMinionOrMannequinITargetExtensions.EMPTY_OUTFIT);
	}

	public static void React(this UIMinionOrMannequin.ITarget self)
	{
		self.React(UIMinionOrMannequinReactSource.None);
	}

	public static void ReactToClothingItemChange(this UIMinionOrMannequin.ITarget self, PermitCategory clothingChangedCategory)
	{
		self.React(UIMinionOrMannequinITargetExtensions.<ReactToClothingItemChange>g__GetSource|7_0(clothingChangedCategory));
	}

	public static void ReactToPersonalityChange(this UIMinionOrMannequin.ITarget self)
	{
		self.React(UIMinionOrMannequinReactSource.OnPersonalityChanged);
	}

	public static void ReactToFullOutfitChange(this UIMinionOrMannequin.ITarget self)
	{
		self.React(UIMinionOrMannequinReactSource.OnWholeOutfitChanged);
	}

	[CompilerGenerated]
	internal static UIMinionOrMannequinReactSource <ReactToClothingItemChange>g__GetSource|7_0(PermitCategory clothingChangedCategory)
	{
		switch (clothingChangedCategory)
		{
		case PermitCategory.DupeTops:
			return UIMinionOrMannequinReactSource.OnTopChanged;
		case PermitCategory.DupeBottoms:
			return UIMinionOrMannequinReactSource.OnBottomChanged;
		case PermitCategory.DupeGloves:
			return UIMinionOrMannequinReactSource.OnGlovesChanged;
		case PermitCategory.DupeShoes:
			return UIMinionOrMannequinReactSource.OnShoesChanged;
		default:
			DebugUtil.DevAssert(false, string.Format("Couldn't find a reaction for \"{0}\" clothing item category being changed", clothingChangedCategory), null);
			return UIMinionOrMannequinReactSource.None;
		}
	}

	public static readonly ClothingItemResource[] EMPTY_OUTFIT = new ClothingItemResource[0];
}
